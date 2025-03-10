using EquipmentRentalAPI.DTO;
using EquipmentRentalAPI.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EquipmentRentalAPI.Services
{
    public interface IEquipmentsService
    {
        Task<bool> EquipmentExistsAsync(int id);
        Task<EquipmentDTO?> GetEquipmentByIdAsync(int id);
        Task<bool> UpdateEquipmentAsync(EquipmentDTO equipmentDto);
        Task<bool> DeleteEquipmentAsync(int id);
        Task<bool> CreateEquipmentAsync(EquipmentDTO equipmentDto);
        Task<IEnumerable<EquipmentDTO>> GetEquipmentsByModelIdAsync(int modelId);
        Task<bool> HasRentalAsync(int equipmentId);
    }

    public class EquipmentsService : IEquipmentsService
    {
        private readonly EquipmentRentalsContext _context;

        public EquipmentsService(EquipmentRentalsContext context)
        {
            _context = context;
        }

        public async Task<bool> EquipmentExistsAsync(int id)
        {
            Log.Information("Checking if equipment with ID {EquipmentId} exists.", id);
            var exists = await _context.Equipments.AnyAsync(e => e.EquipmentId == id);
            Log.Information("Equipment with ID {EquipmentId} exists: {Exists}.", id, exists);
            return exists;
        }

        public async Task<EquipmentDTO?> GetEquipmentByIdAsync(int id)
        {
            Log.Information("Fetching equipment with ID {EquipmentId}.", id);
            var equipment = await _context.Equipments
                .FirstOrDefaultAsync(e => e.EquipmentId == id);

            if (equipment == null)
            {
                Log.Warning("Equipment with ID {EquipmentId} not found.", id);
                return null;
            }

            Log.Information("Equipment with ID {EquipmentId} found.", id);
            return new EquipmentDTO
            {
                EquipmentId = equipment.EquipmentId,
                SerialNumber = equipment.SerialNumber
            };
        }

        public async Task<bool> UpdateEquipmentAsync(EquipmentDTO equipmentDto)
        {
            Log.Information("Updating equipment with ID {EquipmentId}.", equipmentDto.EquipmentId);
            var equipment = await _context.Equipments
                .FirstOrDefaultAsync(e => e.EquipmentId == equipmentDto.EquipmentId);

            if (equipment == null)
            {
                Log.Warning("Equipment with ID {EquipmentId} not found for update.", equipmentDto.EquipmentId);
                return false;
            }

            equipment.SerialNumber = equipmentDto.SerialNumber;

            try
            {
                _context.Equipments.Update(equipment);
                await _context.SaveChangesAsync();
                Log.Information("Equipment with ID {EquipmentId} updated successfully.", equipmentDto.EquipmentId);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating equipment with ID {EquipmentId}.", equipmentDto.EquipmentId);
                return false;
            }
        }

        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            Log.Information("Attempting to delete equipment with ID {EquipmentId}.", id);
            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null)
            {
                Log.Warning("Equipment with ID {EquipmentId} not found for deletion.", id);
                return false;
            }

            try
            {
                _context.Equipments.Remove(equipment);
                await _context.SaveChangesAsync();
                Log.Information("Equipment with ID {EquipmentId} deleted successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting equipment with ID {EquipmentId}.", id);
                return false;
            }
        }

        public async Task<bool> CreateEquipmentAsync(EquipmentDTO equipmentDto)
        {
            Log.Information("Creating new equipment with serial number {SerialNumber}.", equipmentDto.SerialNumber);
            var newEquipment = new Equipment
            {
                SerialNumber = equipmentDto.SerialNumber,
                ModelId = equipmentDto.ModelId
            };

            try
            {
                _context.Equipments.Add(newEquipment);
                await _context.SaveChangesAsync();
                Log.Information("New equipment with serial number {SerialNumber} created successfully.", equipmentDto.SerialNumber);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating new equipment with serial number {SerialNumber}.", equipmentDto.SerialNumber);
                return false;
            }
        }

        public async Task<IEnumerable<EquipmentDTO>> GetEquipmentsByModelIdAsync(int modelId)
        {
            Log.Information("Fetching equipment by model ID {ModelId}.", modelId);
            var equipments = await _context.Equipments
                .Where(e => e.ModelId == modelId)
                .ToListAsync();

            Log.Information("Fetched {EquipmentsCount} equipment(s) for model ID {ModelId}.", equipments.Count, modelId);

            return equipments.Select(e => new EquipmentDTO
            {
                EquipmentId = e.EquipmentId,
                SerialNumber = e.SerialNumber,
                ModelId = e.ModelId
            });
        }

        public async Task<bool> HasRentalAsync(int equipmentId)
        {
            Log.Information("Checking if equipment with ID {EquipmentId} has an active rental.", equipmentId);
            var hasRental = await _context.Rentals.AnyAsync(e => e.EquipmentId == equipmentId);
            Log.Information("Equipment with ID {EquipmentId} has rental: {HasRental}.", equipmentId, hasRental);
            return hasRental;
        }
    }
}
