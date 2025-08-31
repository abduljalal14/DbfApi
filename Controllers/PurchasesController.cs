using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PurchasesController : ControllerBase
{
    private readonly DbfService _dbfService;

    public PurchasesController(DbfService dbfService)
    {
        _dbfService = dbfService;
    }

    // Endpoint: /purchases
    [HttpGet]
    public ActionResult<IEnumerable<Pembelian>> GetPurchases(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10,
        [FromQuery] string? sort = "ID",
        [FromQuery] string? filter = null)
    {
        var result = _dbfService.GetPembelian(page, limit, sort, filter);
        if (result == null || !result.Any())
        {
            return NotFound("No records found.");
        }
        return Ok(result);
    }

    // Endpoint: /purchases/{id}
    [HttpGet("{id}")]
    public ActionResult<IEnumerable<PembelianDetail>> GetPurchaseDetails(long id)
    {
        var result = _dbfService.GetPembelianWithDetails(id);
        if (result == null)
        {
            return NotFound($"Purchase with ID {id} not found or has no details.");
        }
        return Ok(result);
    }
}