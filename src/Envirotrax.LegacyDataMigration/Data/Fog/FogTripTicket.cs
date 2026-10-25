
using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.LegacyDataMigration.Data.Fog;

// Only the columns the signature migration reads and writes. The full model lives in Envirotrax.App.Server;
// rows are inserted by Scripts/FogTripTickets/02_FogTripTickets.sql, never by EF, so the columns that script
// alone fills in are deliberately absent here.
public class FogTripTicket : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }

    [MaxLength(500)]
    public string? TransporterSignaturePath { get; set; }

    [MaxLength(500)]
    public string? GeneratorSignaturePath { get; set; }

    [MaxLength(500)]
    public string? ReceiverSignaturePath { get; set; }

    // Points back to Vepo.dbo.FogTripTickets.ID. It names the blobs, and it is what the signatures are read
    // back from the legacy database by.
    public int? LegacyRecordId { get; set; }

    // V1 keeps the signatures as bytes on the ticket itself rather than as files, so all that can be
    // recorded is that there is one to copy.
    public bool? HasLegacyTransporterSignature { get; set; }

    public bool? HasLegacyGeneratorSignature { get; set; }

    public bool? HasLegacyReceiverSignature { get; set; }
}
