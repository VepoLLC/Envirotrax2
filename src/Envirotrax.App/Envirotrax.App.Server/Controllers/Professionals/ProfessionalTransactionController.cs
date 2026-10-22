using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;

[Route("api/professionals/transactions")]
public class ProfessionalTransactionController : ProfessionalProtectedController
{
    private readonly IProfessionalTransactionService _transactionService;
    private readonly IProfessionalReceiptService _receiptService;

    public ProfessionalTransactionController(
        IProfessionalTransactionService transactionService,
        IProfessionalReceiptService receiptService)
    {
        _transactionService = transactionService;
        _receiptService = receiptService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _transactionService.GetAllAsync(pageInfo, query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{transactionId}/receipt")]
    public async Task<IActionResult> GetReceiptPdfAsync(string transactionId, CancellationToken cancellationToken)
    {
        var pdf = await _receiptService.GeneratePdfAsync(transactionId, cancellationToken);

        if (pdf == null)
        {
            return NotFound();
        }

        return File(pdf, "application/pdf");
    }
}
