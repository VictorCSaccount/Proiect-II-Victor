using Microsoft.AspNetCore.Mvc;
using ProiectII.DTO.CommentsReport;
using ProiectII.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateReportDto dto)
    {
        // Analiză rece: Hardcodăm un UserId de test până faci Auth-ul
        string testUserId = "b144d7ba-6ab2-4efd-b81c-5eec01ba039f";

        try
        {
            var result = await _reportService.CreateReportAsync(dto, testUserId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reports = await _reportService.GetAllActiveReportsAsync();
        return Ok(reports);
    }
}