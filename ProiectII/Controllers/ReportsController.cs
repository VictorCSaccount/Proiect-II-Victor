using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProiectII.DTO.CommentsReport;
using ProiectII.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }
    //[HttpPost]
    //[Authorize] // Obligatoriu logat pentru a raporta
    //[Consumes("multipart/form-data")]
    //public async Task<IActionResult> Create([FromForm] CreateReportDto dto)
    //{
    //    // Extragem ID-ul real din token-ul de securitate
    //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    //    if (string.IsNullOrEmpty(userId))
    //        return Unauthorized("Utilizator invalid.");

    //    try
    //    {
    //        var result = await _reportService.CreateReportAsync(dto, userId);
    //        return Ok(result);
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reports = await _reportService.GetAllActiveReportsAsync();
        return Ok(reports);
    }


    [Authorize(Roles = "Admin,Employee")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(uint id, [FromBody] UpdateReportStatusDto dto)
    {
        var success = await _reportService.UpdateReportStatusAsync(id, dto);
        if (!success) return BadRequest("Status invalid sau raport negăsit.");

        return Ok(new { Message = "Statusul raportului a fost actualizat." });
    }

    [Authorize(Roles = "Admin,Employee")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(uint id)
    {
        var success = await _reportService.DeleteReportAsync(id);
        if (!success) return NotFound();

        return Ok(new { Message = "Raport șters permanent (inclusiv imaginea)." });
    }


    [HttpPost]
    public async Task<IActionResult> CreateReport([FromForm] CreateReportDto dto)
    {
        string? currentUserId = null;

        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        var createdReport = await _reportService.CreateReportAsync(dto, currentUserId);

        return Ok(createdReport);
    }



}