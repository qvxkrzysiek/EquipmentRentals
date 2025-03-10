using EquipmentRentalAPI.DTO;
using EquipmentRentalAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EquipmentRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : Controller
    {
        private readonly IModelsService _service;

        public ModelsController(IModelsService service)
        {
            _service = service;
        }

        // GET: api/models/stock
        [HttpGet("stock")]
        public async Task<ActionResult<IEnumerable<ModelsInStockDTO>>> GetModelsInStock()
        {
            Log.Information("Fetching models in stock");
            var equipments = await _service.GetModelsInStockAsync();
            return Ok(equipments);
        }

        // GET: api/models
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ModelDTO>> GetModels()
        {
            Log.Information("Fetching all models");
            var models = await _service.GetModelsAsync();

            if (models == null || !models.Any())
            {
                Log.Warning("No models found");
                return NotFound("Brak dostępnych modeli.");
            }

            return Ok(models);
        }

        // POST: api/models
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateModel([FromBody] ModelDTO newEquipment)
        {
            Log.Information("Creating a new model");
            if (await _service.CreateModelAsync(newEquipment))
            {
                Log.Information("Model created successfully");
                return NoContent();
            }
            else
            {
                Log.Error("Error occurred while creating a model");
                return StatusCode(500, "Wystąpił problem podczas tworzenia nowego sprzętu.");
            }
        }

        // PUT: api/models/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateModel(int id, [FromBody] ModelDTO updatedEquipment)
        {
            if (id != updatedEquipment.ModelId)
            {
                Log.Warning("ID mismatch in update request. URL ID: {Id}, Body ID: {BodyId}", id, updatedEquipment.ModelId);
                return BadRequest("ID w URL nie pasuje do ID w ciele żądania.");
            }

            Log.Information("Updating model with ID {Id}", id);
            var success = await _service.UpdateModelAsync(updatedEquipment);

            if (success)
            {
                Log.Information("Model {Id} updated successfully", id);
                return NoContent();
            }
            else
            {
                Log.Warning("Model {Id} not found", id);
                return NotFound("Nie znaleziono modelu o podanym ID.");
            }
        }

        // DELETE: api/models/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteModel(int id)
        {
            Log.Information("Attempting to delete model with ID {Id}", id);
            if (!await _service.ModelExistsAsync(id))
            {
                Log.Warning("Model {Id} does not exist", id);
                return NotFound($"Model o ID {id} nie istnieje.");
            }

            if (await _service.HasEquipmentAsync(id))
            {
                Log.Warning("Model {Id} has associated equipment and cannot be deleted", id);
                return Conflict("Nie można usunąć modelu, ponieważ są do niego przypisane urządzenia");
            }

            if (await _service.DeleteModelAsync(id))
            {
                Log.Information("Model {Id} deleted successfully", id);
                return NoContent();
            }
            else
            {
                Log.Error("Error occurred while deleting model {Id}", id);
                return StatusCode(500, "Wystąpił problem podczas usuwania modelu.");
            }
        }
    }
}