using CBTClinic.Application.DTOs.Doctor;
using CBTClinic.Application.DTOs.Patient;
using CBTClinic.Application.Interfaces.Repositories;
using CBTClinic.Domain.Entities;
using CBTClinic.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CBTClinic.API.Controllers
{
    public class PatientController : ControllerBase
    {

        private readonly IPatientRepository _patientRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientController(IPatientRepository patientRepository, UserManager<ApplicationUser> userManager)
        {
            _patientRepository = patientRepository;
            _userManager = userManager;
        }


        [HttpPut("update-name")]
        [Authorize]
        public async Task<IActionResult> UpdateName([FromBody] UpdatePatientNameDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var success = await _patientRepository.UpdateNameAsync(userId, dto.FullName);
            if (!success) return NotFound();
            return Ok(new { message = "Name updated successfully" });
        }



        [HttpPut("update-email")]
        [Authorize]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdatePatientEmailDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var success = await _patientRepository.UpdateEmailAsync(userId,dto.Email, _userManager);

            if (!success) return NotFound(new { message = "Patient not found or email not updated." });

            return Ok(new { message = "Email updated successfully" });
        }





        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePatientPasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var passwordValidator = new PasswordValidator<ApplicationUser>();
            var validationResult = await passwordValidator.ValidateAsync(_userManager, user, dto.NewPassword);
            if (!validationResult.Succeeded)
            {
                var errors = validationResult.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { error = "Password does not meet requirements.", details = errors });
            }

            var success = await _patientRepository.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword, _userManager);
            if (!success)
                return BadRequest(new { error = "Failed to change password. Current password may be incorrect." });

            return Ok(new { message = "Password changed successfully" });
        }




        [HttpDelete("deactivate")]
        [Authorize]
        public async Task<IActionResult> DeactivateAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var success = await _patientRepository.DeactivateAccountAsync(userId);
            if (!success) return NotFound();
            return Ok(new { message = "Account Deleted successfully" });
        }



        // SEARCH: api/doctor/search
        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> Search([FromQuery] DoctorSearchDto searchDto)
        {
            var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(patientId))
                return Unauthorized(new { error = "User must be logged in to search doctors." });

            var doctors = await _patientRepository.SearchDoctorsAsync(patientId, searchDto);
            if (!doctors.Any())
                return NotFound(new { error = "No doctors matched your search criteria." });

            return Ok(new { message = "Doctors fetched successfully", data = doctors });
        }




        // POST: api/doctor/favorite/{doctorId}
        [HttpPost("favorite/{doctorId}")]
        [Authorize]
        public async Task<IActionResult> AddFavorite(int doctorId)
        {
            var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(patientId))
                return Unauthorized(new { error = "User must be logged in to add favorite doctor." });

            try
            {
                await _patientRepository.AddFavoriteDoctorAsync(patientId, doctorId);
                return Ok(new { message = $"Doctor {doctorId} added to favorites successfully." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { error = "Unexpected error", details = ex.Message }); }
        }




        // DELETE: api/doctor/favorite/{doctorId}
        [HttpDelete("favorite/{doctorId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFavorite(int doctorId)
        {
            var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(patientId))
                return Unauthorized(new { error = "User must be logged in to remove favorite doctor." });

            try
            {
                await _patientRepository.RemoveFavoriteDoctorAsync(patientId, doctorId);
                return Ok(new { message = $"Doctor {doctorId} removed from favorites successfully." });
            }
            catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, new { error = "Unexpected error", details = ex.Message }); }
        }



        // GET: api/doctor/favorites
        [HttpGet("favorites")]
        [Authorize]
        public async Task<IActionResult> GetFavorites()
        {
            var patientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(patientId))
                return Unauthorized(new { error = "User must be logged in to view favorite doctors." });

            try
            {
                var favorites = await _patientRepository.GetFavoriteDoctorsAsync(patientId);
                if (!favorites.Any())
                    return NotFound(new { error = "No favorite doctors found for this user." });

                return Ok(new { message = "Favorite doctors fetched successfully.", data = favorites });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Unexpected error", details = ex.Message });
            }
        }



        [HttpGet("my-notifications")]
        [Authorize]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var patientId = await _patientRepository.GetPatientIdByUserIdAsync(userId);
            if (patientId == null)
                return NotFound(new { message = "Patient not found" });

            var notifications = await _patientRepository
                .GetPatientNotificationsAsync(patientId.Value);

            return Ok(notifications);
        }



        [HttpPut("mark-as-read/{id}")]
        [Authorize]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _patientRepository.MarkMessageAsReadAsync(id);
            return Ok(new { message = "Marked as read" });
        }




        [HttpGet("my-messages")]
        [Authorize]
        public async Task<IActionResult> GetMyMessages()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var patientId = await _patientRepository.GetPatientIdByUserIdAsync(userId);
            if (patientId == null)
                return NotFound(new { message = "Patient not found" });

            var messages = await _patientRepository.GetPatientMessagesAsync(patientId.Value);

            return Ok(messages);
        }





    }
}
