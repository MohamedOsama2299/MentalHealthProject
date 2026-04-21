using CBTClinic.Application.DTOs.Appointment;
using CBTClinic.Application.DTOs.Auth;
using CBTClinic.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminProfileService _service;

    public AdminController(IAdminProfileService service)
    {
        _service = service;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var profile = await _service.GetProfileAsync();
        return Ok(new
        {
            Success = true,
            Message = "Profile fetched successfully",
            Data = profile
        });
    }



    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var stats = await _service.GetStatisticsAsync();
        return Ok(new
        {
            Success = true,
            Message = "Statistics fetched successfully",
            Data = stats
        });
    }




    [HttpPut("update-name")]
    public async Task<IActionResult> UpdateName([FromBody] UpdateAdminNameDto dto)
    {
        var result = await _service.UpdateNameAsync(dto);
        return Ok(new
        {
            Success = result.Success,
            Message = result.Message
        });
    }



    [HttpPut("update-email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateAdminEmailDto dto)
    {
        var result = await _service.UpdateEmailAsync(dto);
        return Ok(new
        {
            Success = result.Success,
            Message = result.Message
        });
    }



    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangeAdminPasswordDto dto)
    {
        var result = await _service.ChangePasswordAsync(dto);
        return Ok(new
        {
            Success = result.Success,
            Message = result.Message
        });
    }



    [HttpPost("add-image")]
    public async Task<IActionResult> AddOrUpdateImage([FromForm] UpdateAdminImageDto dto)
    {
        var result = await _service.AddOrUpdateImageAsync(dto);
        return Ok(new
        {
            Success = result.Success,
            Message = result.Message
        });
    }




    [HttpDelete("delete-image")]
    public async Task<IActionResult> DeleteImage()
    {
        var result = await _service.DeleteImageAsync();
        return Ok(new
        {
            Success = result.Success,
            Message = result.Message
        });
    }


   
}

