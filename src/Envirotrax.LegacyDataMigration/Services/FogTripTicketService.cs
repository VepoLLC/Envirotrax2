
using System.Linq.Expressions;
using Envirotrax.LegacyDataMigration.Data;
using Envirotrax.LegacyDataMigration.Data.Fog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Services;

public class FogTripTicketService
{
    private const string ScriptsFolder = "Scripts/FogTripTickets";

    // Azure Storage being down fails every signature the same way. Stopping early keeps a dead host from
    // costing thousands of timeouts; nothing is written, so a later run resumes.
    private const int MaxConsecutiveFailures = 10;

    // How many tickets' signatures are read from the legacy database in one query. A ticket holds at most
    // three small PNGs, so a batch stays at a few megabytes.
    private const int SignatureBatchSize = 200;

    private readonly ILogger<FogTripTicketService> _logger;
    private readonly AppDbContext _appDbContext;
    private readonly BlobStorageService _blobStorageService;

    public FogTripTicketService(
        ILogger<FogTripTicketService> logger,
        AppDbContext appDbContext,
        BlobStorageService blobStorageService)
    {
        _logger = logger;
        _appDbContext = appDbContext;
        _blobStorageService = blobStorageService;
    }

    public async Task MigrateAsync()
    {
        _logger.LogInformation("--------- Starting migration of FOG trip tickets ---------");

        await ExecuteSqlScriptsAsync();
        await MoveSignaturesToAzureAsync();

        _logger.LogInformation("--------- Finished migration of FOG trip tickets ---------");
    }

    private async Task ExecuteSqlScriptsAsync()
    {
        _logger.LogInformation("Executing database scripts from {folderName}.", ScriptsFolder);

        var scripts = Directory.GetFiles(ScriptsFolder, "*.sql").OrderBy(file => file);

        foreach (var file in scripts)
        {
            _logger.LogInformation("Executing script {file}", file);
            var sql = await File.ReadAllTextAsync(file);

            var addedRows = await _appDbContext.Database.ExecuteSqlRawAsync(sql);
            _logger.LogInformation("Imported FOG trip tickets. Count: {count}", addedRows);
        }

        _logger.LogInformation("Completed executing database scripts from {folderName}.", ScriptsFolder);
    }

    // Copies every V1 transporter, generator and receiver signature that has not been copied yet into Azure
    // Storage. Each signature path stays NULL until that signature really is in Azure, so a ticket whose
    // signature did not make it simply shows none instead of a link that goes nowhere.
    private async Task MoveSignaturesToAzureAsync()
    {
        _logger.LogInformation("Moving legacy FOG trip ticket signatures into Azure Storage.");

        try
        {
            await _blobStorageService.VerifyCanWriteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Cannot write to Azure Storage, so no FOG trip ticket signature was moved and every one of them is still pending. Sign in with an account that has the Storage Blob Data Contributor role on the configured storage account, then run the migration again. Error message: {errorMessage}.",
                ex.Message);

            return;
        }

        // The signature paths are the completion flags: each is written only after its upload returned, so
        // anything still NULL has not made it to Azure yet and anything set is never touched again. Read
        // untracked, because tracking every pending row makes each save rescan all of them. Requiring a
        // LegacyRecordId keeps the blob names below from collapsing to a shared one.
        var pendingTickets = await _appDbContext
            .FogTripTickets
            .AsNoTracking()
            .Where(ticket => ticket.LegacyRecordId != null
                && ((ticket.HasLegacyTransporterSignature == true && ticket.TransporterSignaturePath == null)
                    || (ticket.HasLegacyGeneratorSignature == true && ticket.GeneratorSignaturePath == null)
                    || (ticket.HasLegacyReceiverSignature == true && ticket.ReceiverSignaturePath == null)))
            .OrderBy(ticket => ticket.Id)
            .ToListAsync();

        _logger.LogInformation("Moving signatures of {Count} FOG trip tickets.", pendingTickets.Count);

        var movedCount = 0;
        var unavailableCount = 0;
        var failedCount = 0;
        var consecutiveFailureCount = 0;
        var stopped = false;

        foreach (var batch in pendingTickets.Chunk(SignatureBatchSize))
        {
            var legacySignatures = await ReadLegacySignaturesAsync(batch);

            foreach (var signature in GetPendingSignatures(batch, legacySignatures))
            {
                var ticket = signature.Ticket;

                if (signature.Content == null || signature.Content.Length == 0)
                {
                    _logger.LogWarning(
                        "The {kind} signature of FOG trip ticket {ticketId} (V1 ticket {legacyRecordId}) was not moved because the legacy database no longer holds it.",
                        signature.Kind, ticket.Id, ticket.LegacyRecordId);

                    unavailableCount++;

                    continue;
                }

                try
                {
                    // The folders V2's FogTripTicketService uses, with the V1 record id in place of a new GUID,
                    // so a retry overwrites the same blob instead of orphaning one. V2 keeps no per-ticket
                    // transporter signature, but V1 does, so it gets a folder of its own next to the other two.
                    // The folder is the V2 ProfessionalId, which a rebuild of the V2 database reassigns, so the
                    // previous run's blobs are left behind rather than reused after one.
                    var blobPath = $"professionals/{ticket.ProfessionalId}/fog-trip-tickets/{signature.Kind}/legacy-{ticket.LegacyRecordId}.png";

                    string uploadedPath;

                    using (var content = new MemoryStream(signature.Content))
                    {
                        uploadedPath = await _blobStorageService.UploadAsync(blobPath, content);
                    }

                    // Written one signature at a time, after the upload, so a run that dies half way keeps every
                    // signature it already moved and never points a link at a blob that is not there.
                    await _appDbContext
                        .FogTripTickets
                        .Where(pendingTicket => pendingTicket.WaterSupplierId == ticket.WaterSupplierId
                            && pendingTicket.Id == ticket.Id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(signature.PathProperty, uploadedPath));

                    movedCount++;
                    consecutiveFailureCount = 0;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        "Failed moving the {kind} signature of FOG trip ticket {ticketId} (V1 ticket {legacyRecordId}). Error message: {errorMessage}.",
                        signature.Kind, ticket.Id, ticket.LegacyRecordId, ex.Message);

                    failedCount++;
                    consecutiveFailureCount++;

                    if (consecutiveFailureCount >= MaxConsecutiveFailures)
                    {
                        _logger.LogError(
                            "Stopping after {failureCount} FOG trip ticket signatures failed in a row. Azure Storage is most likely unreachable. Everything left is still pending, so run the migration again once it is back.",
                            consecutiveFailureCount);

                        stopped = true;

                        break;
                    }
                }
            }

            if (stopped)
            {
                break;
            }
        }

        _logger.LogInformation(
            "Moved {MovedCount} FOG trip ticket signatures, could not get {UnavailableCount} out of V1, and failed {FailedCount} signatures.",
            movedCount, unavailableCount, failedCount);
    }

    // One query for a whole batch of tickets rather than one per signature. V1 saves each signature as a PNG
    // straight onto the ticket (transporters/signature.aspx.vb, and a copy of the transporter's own signature
    // when the ticket is submitted in transporters/trip_ticket_submit.aspx.vb).
    private async Task<Dictionary<int, LegacyTripTicketSignatures>> ReadLegacySignaturesAsync(IReadOnlyCollection<FogTripTicket> tickets)
    {
        var legacyRecordIds = tickets
            .Select(ticket => ticket.LegacyRecordId!.Value)
            .ToList();

        var signatures = await _appDbContext
            .Database
            .SqlQuery<LegacyTripTicketSignatures>($"""
                SELECT ID AS LegacyRecordId,
                    CAST(TransporterSignature AS VARBINARY(MAX)) AS TransporterSignature,
                    CAST(GeneratorSignature AS VARBINARY(MAX)) AS GeneratorSignature,
                    CAST(ReceiverSignature AS VARBINARY(MAX)) AS ReceiverSignature
                FROM Vepo.dbo.FogTripTickets
                """)
            .Where(signature => legacyRecordIds.Contains(signature.LegacyRecordId))
            .ToListAsync();

        return signatures.ToDictionary(signature => signature.LegacyRecordId);
    }

    // The signatures of a batch of tickets that are still waiting to be copied, with the bytes V1 holds for
    // each. A ticket V1 no longer has comes back with no content, which is reported rather than retried.
    private static IEnumerable<PendingSignature> GetPendingSignatures(
        IEnumerable<FogTripTicket> tickets,
        Dictionary<int, LegacyTripTicketSignatures> legacySignatures)
    {
        foreach (var ticket in tickets)
        {
            legacySignatures.TryGetValue(ticket.LegacyRecordId!.Value, out var legacy);

            if (ticket.HasLegacyTransporterSignature == true && ticket.TransporterSignaturePath == null)
            {
                yield return new PendingSignature(
                    ticket,
                    "transporter",
                    legacy?.TransporterSignature,
                    pendingTicket => pendingTicket.TransporterSignaturePath);
            }

            if (ticket.HasLegacyGeneratorSignature == true && ticket.GeneratorSignaturePath == null)
            {
                yield return new PendingSignature(
                    ticket,
                    "generator",
                    legacy?.GeneratorSignature,
                    pendingTicket => pendingTicket.GeneratorSignaturePath);
            }

            if (ticket.HasLegacyReceiverSignature == true && ticket.ReceiverSignaturePath == null)
            {
                yield return new PendingSignature(
                    ticket,
                    "receiver",
                    legacy?.ReceiverSignature,
                    pendingTicket => pendingTicket.ReceiverSignaturePath);
            }
        }
    }

    private sealed record LegacyTripTicketSignatures(
        int LegacyRecordId,
        byte[]? TransporterSignature,
        byte[]? GeneratorSignature,
        byte[]? ReceiverSignature);

    // Kind is the V2 folder the signature goes into. PathProperty is the column that is set once it is there.
    private sealed record PendingSignature(
        FogTripTicket Ticket,
        string Kind,
        byte[]? Content,
        Expression<Func<FogTripTicket, string?>> PathProperty);
}
