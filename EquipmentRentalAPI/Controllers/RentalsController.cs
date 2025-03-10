using EquipmentRentalAPI.DTO;
using EquipmentRentalAPI.Models;
using EquipmentRentalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Serilog;

namespace EquipmentRentals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IRentalService _service;

        public RentalsController(IRentalService rentalService)
        {
            _service = rentalService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllRentals()
        {
            Log.Information("Fetching all rentals by admin.");
            var rentals = await _service.GetAllRentalsAsync();
            Log.Information($"Fetched {rentals.Count} rentals.");
            return Ok(rentals);
        }

        [HttpPut("{rentalId}/return")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ReturnRental(int rentalId)
        {
            Log.Information($"Admin is returning rental with ID {rentalId}.");
            var result = await _service.ReturnRentalAsync(rentalId);

            if (!result)
            {
                Log.Warning($"Rental with ID {rentalId} not found or not owned by user.");
                return NotFound("Rental not found or not owned by user");
            }

            Log.Information($"Rental with ID {rentalId} returned successfully.");
            return NoContent();
        }

        [HttpPost]
        [Authorize]  // Używamy JWT do autoryzacji
        public async Task<IActionResult> CreateRental([FromBody] RentalRequestDTO rentalRequest)
        {
            Log.Information("Creating rental request.");

            if (rentalRequest.StartDate.Date != DateTime.UtcNow.Date)
            {
                Log.Warning("Start date is not today.");
                return BadRequest("Start date must be today.");
            }

            if (rentalRequest.EndDate < rentalRequest.StartDate.AddDays(7))
            {
                Log.Warning("End date is less than 7 days from start date.");
                return BadRequest("End date cannot be less than 7 days from start date.");
            }

            if (rentalRequest.EndDate > rentalRequest.StartDate.AddDays(30))
            {
                Log.Warning("End date is more than 30 days from start date.");
                return BadRequest("End date cannot be more than 30 days from start date.");
            }

            if (rentalRequest.EndDate < rentalRequest.StartDate)
            {
                Log.Warning("End date is earlier than start date.");
                return BadRequest("End date cannot be earlier than start date.");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Log.Information($"User {userId} is creating a rental.");

            if (!await _service.EquipmentModelExists(rentalRequest.EquipmentModelId))
            {
                Log.Warning($"Equipment model with ID {rentalRequest.EquipmentModelId} does not exist.");
                return NotFound("Specified model does not exist");
            }

            var equipment = await _service.GetAvailableEquipment(rentalRequest.EquipmentModelId);

            if (equipment == null)
            {
                Log.Warning("No available equipment for the requested model.");
                return NotFound("Not available equipment");
            }

            var rentalResult = await _service.CreateRentalAsync(rentalRequest, userId, equipment);
            Log.Information($"Rental created successfully for user {userId}.");
            return rentalResult;
        }
    }
}
