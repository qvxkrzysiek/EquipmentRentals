using Microsoft.AspNetCore.Mvc;
using EquipmentRentalAPI.Services;
using EquipmentRentalAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace EquipmentRentals.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentsService _service;

        public EquipmentController(IEquipmentsService service)
        {
            _service = service;
        }

        [HttpGet("model/{modelId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEquipmentsByModelId(int modelId)
        {
            Log.Information("Fetching equipments for model ID {ModelId}", modelId);
            var equipments = await _service.GetEquipmentsByModelIdAsync(modelId);

            if (equipments == null || !equipments.Any())
            {
                Log.Warning("No equipment found for model ID {ModelId}", modelId);
                return NotFound($"Brak sprzętu dla modelu o ID {modelId}.");
            }

            Log.Information("Returning {Count} equipments for model ID {ModelId}", equipments.Count(), modelId);
            return Ok(equipments);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEquipment([FromBody] EquipmentDTO newEquipment)
        {
            Log.Information("Creating new equipment: {@NewEquipment}", newEquipment);
            if (await _service.CreateEquipmentAsync(newEquipment))
            {
                Log.Information("Equipment created successfully");
                return NoContent();
            }
            else
            {
                Log.Error("Error occurred while creating new equipment");
                return StatusCode(500, "Wystąpił problem podczas tworzenia nowego sprzętu.");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEquipment(int id, [FromBody] EquipmentDTO updatedEquipment)
        {
            Log.Information("Updating equipment ID {Id}: {@UpdatedEquipment}", id, updatedEquipment);
            if (!await _service.EquipmentExistsAsync(id))
            {
                Log.Warning("Attempted to update non-existing equipment ID {Id}", id);
                return NotFound($"Sprzęt o ID {id} nie istnieje.");
            }

            if (await _service.UpdateEquipmentAsync(updatedEquipment))
            {
                Log.Information("Equipment ID {Id} updated successfully", id);
                return NoContent();
            }
            else
            {
                Log.Error("Error occurred while updating equipment ID {Id}", id);
                return StatusCode(500, "Wystąpił problem podczas aktualizacji sprzętu.");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            Log.Information("Attempting to delete equipment ID {Id}", id);
            if (!await _service.EquipmentExistsAsync(id))
            {
                Log.Warning("Attempted to delete non-existing equipment ID {Id}", id);
                return NotFound($"Sprzęt o ID {id} nie istnieje.");
            }

            if (await _service.HasRentalAsync(id))
            {
                Log.Warning("Cannot delete equipment ID {Id} due to active rentals", id);
                return Conflict("Nie można usunąć sprzętu, ponieważ są do niego przypisane wypożyczenia");
            }

            if (await _service.DeleteEquipmentAsync(id))
            {
                Log.Information("Equipment ID {Id} deleted successfully", id);
                return NoContent();
            }
            else
            {
                Log.Error("Error occurred while deleting equipment ID {Id}", id);
                return StatusCode(500, "Wystąpił problem podczas usuwania sprzętu.");
            }
        }
    }
}