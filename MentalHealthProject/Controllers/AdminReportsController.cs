using CBTClinic.Application.DTOs.Appointment;
using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using CBTClinic.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/admin/reports")]
[Authorize(Roles = "Admin")]
public class AdminReportsController : ControllerBase
{
    private readonly IAdminReportRepository _repository;
    private readonly ApplicationDbContext _context;

    public AdminReportsController(
        IAdminReportRepository repository,
        ApplicationDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    // Add Report
    [HttpPost]
    public async Task<IActionResult> AddReport([FromBody] SubmitQuizDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.FromDate > dto.ToDate)
            return BadRequest(new { message = "FromDate cannot be greater than ToDate." });

        var admin = await _context.Admins
            .FirstOrDefaultAsync(a => a.Email == "admin@system.com");

        if (admin == null)
            return NotFound(new { message = "Admin not found." });

        var report = new AdminReport
        {
            Title = dto.Title.Trim(),
            Content = dto.Content.Trim(),
            FromDate = dto.FromDate,
            ToDate = dto.ToDate,
            ReportDate = dto.ReportDate == default
                            ? DateTime.UtcNow
                            : dto.ReportDate,
            AdminId = admin.Id
        };

        await _repository.AddReportAsync(report);

        return Ok(new
        {
            Success = true,
            Message = "Report added successfully",
            Data = report
        });
    }

    // Get All Reports For Admin
    [HttpGet]
    public async Task<IActionResult> GetReports()
    {
        var admin = await _context.Admins
            .FirstOrDefaultAsync(a => a.Email == "admin@system.com");

        if (admin == null)
            return NotFound(new { message = "Admin not found." });

        var reports = await _repository.GetReportsByAdminAsync(admin.Id);

        if (reports == null || !reports.Any())
            return Ok(new { Success = true, Message = "No reports found.", Data = new List<AdminReport>() });

        return Ok(new { Success = true, Data = reports });
    }

    // Update Report
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReport(int id, [FromBody] SubmitQuizDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.FromDate > dto.ToDate)
            return BadRequest(new { message = "FromDate cannot be greater than ToDate." });

        var report = await _repository.GetByIdAsync(id);

        if (report == null)
            return NotFound(new { message = "Report not found." });

        report.Title = dto.Title.Trim();
        report.Content = dto.Content.Trim();
        report.FromDate = dto.FromDate;
        report.ToDate = dto.ToDate;
        report.ReportDate = dto.ReportDate == default
                                ? report.ReportDate
                                : dto.ReportDate;

        await _repository.UpdateReportAsync(report);

        return Ok(new
        {
            Success = true,
            Message = "Report updated successfully",
            Data = report
        });
    }

    // Delete Report
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReport(int id)
    {
        var report = await _repository.GetByIdAsync(id);

        if (report == null)
            return NotFound(new { message = "Report not found." });

        await _repository.DeleteReportAsync(id);

        return Ok(new
        {
            Success = true,
            Message = "Report deleted successfully"
        });
    }
}