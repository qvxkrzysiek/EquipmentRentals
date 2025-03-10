using EquipmentRentalAPI.DTO;
using EquipmentRentalAPI.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EquipmentRentalAPI.Services
{
    public interface IModelsService
    {
        Task<bool> CreateModelAsync(ModelDTO newEquipment);
        Task<bool> UpdateModelAsync(ModelDTO updatedEquipment);
        Task<bool> DeleteModelAsync(int id);
        Task<bool> HasEquipmentAsync(int id);
        Task<IEnumerable<ModelDTO>> GetModelsAsync();
        Task<IEnumerable<ModelsInStockDTO>> GetModelsInStockAsync();
        Task<bool> ModelExistsAsync(int id);
    }

    public class ModelsService : IModelsService
    {
        private readonly EquipmentRentalsContext _context;

        public ModelsService(EquipmentRentalsContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateModelAsync(ModelDTO newEquipment)
        {
            var model = new Models.Models
            {
                Name = newEquipment.Name,
                Description = newEquipment.Description,
                Price = newEquipment.Price
            };

            try
            {
                _context.Models.Add(model);
                await _context.SaveChangesAsync();
                Log.Information("Model created: {ModelName}", newEquipment.Name);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while creating model: {ModelName}", newEquipment.Name);
                return false;
            }
        }

        public async Task<bool> UpdateModelAsync(ModelDTO updatedEquipment)
        {
            var model = await _context.Models
                .FirstOrDefaultAsync(m => m.ModelId == updatedEquipment.ModelId);

            if (model == null)
            {
                Log.Warning("Model with ID {ModelId} not found for update", updatedEquipment.ModelId);
                return false;
            }

            model.Name = updatedEquipment.Name;
            model.Description = updatedEquipment.Description;
            model.Price = updatedEquipment.Price;

            try
            {
                _context.Models.Update(model);
                await _context.SaveChangesAsync();
                Log.Information("Model updated: {ModelName}", updatedEquipment.Name);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating model: {ModelName}", updatedEquipment.Name);
                return false;
            }
        }

        public async Task<bool> DeleteModelAsync(int id)
        {
            var model = await _context.Models
                .FirstOrDefaultAsync(m => m.ModelId == id);

            if (model == null)
            {
                Log.Warning("Model with ID {ModelId} not found for deletion", id);
                return false;
            }

            try
            {
                _context.Models.Remove(model);
                await _context.SaveChangesAsync();
                Log.Information("Model deleted: {ModelId}", id);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while deleting model with ID: {ModelId}", id);
                return false;
            }
        }

        public async Task<bool> HasEquipmentAsync(int modelId)
        {
            var exists = await _context.Equipments.AnyAsync(e => e.ModelId == modelId);
            Log.Information("Checked if model {ModelId} has equipment: {HasEquipment}", modelId, exists);
            return exists;
        }

        public async Task<IEnumerable<ModelDTO>> GetModelsAsync()
        {
            var models = await _context.Models
                .Select(m => new ModelDTO
                {
                    ModelId = m.ModelId,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price
                })
                .ToListAsync();

            Log.Information("Fetched {ModelCount} models", models.Count);
            return models;
        }

        public async Task<IEnumerable<ModelsInStockDTO>> GetModelsInStockAsync()
        {
            var modelsInStock = await _context.Equipments
                .GroupBy(e => e.Model)
                .Select(group => new ModelsInStockDTO
                {
                    ModelId = group.Key.ModelId,
                    Name = group.Key.Name,
                    Description = group.Key.Description,
                    Price = group.Key.Price,
                    Quantity = group.Count(),
                    StockQuantity = group.Count() - _context.Rentals
                        .Where(r => !r.IsReturned && group.Select(e => e.EquipmentId).Contains(r.EquipmentId))
                        .Count()
                })
                .ToListAsync();

            Log.Information("Fetched {ModelsInStockCount} models in stock", modelsInStock.Count);
            return modelsInStock;
        }

        public async Task<bool> ModelExistsAsync(int id)
        {
            var exists = await _context.Models.AnyAsync(e => e.ModelId == id);
            Log.Information("Checked if model {ModelId} exists: {ModelExists}", id, exists);
            return exists;
        }
    }
}
