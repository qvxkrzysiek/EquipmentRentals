using EquipmentRentalAPI.DTO;
using EquipmentRentalAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EquipmentRentalAPI.Services
{
    public interface IRentalService
    {
        Task<IActionResult> CreateRentalAsync(RentalRequestDTO rentalRequest, int userId, Equipment availableEquipment);
        Task<Equipment?> GetAvailableEquipment(int modelId);
        Task<bool> EquipmentModelExists(int modelId);
        Task<List<RentalDTO>> GetAllRentalsAsync();
        Task<bool> ReturnRentalAsync(int rentalId);
    }

    public class RentalService : IRentalService
    {
        private readonly EquipmentRentalsContext _context;

        public RentalService(EquipmentRentalsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> CreateRentalAsync(RentalRequestDTO rentalRequest, int userId, Equipment availableEquipment)
        {
            var rental = new Rental
            {
                RentalDate = rentalRequest.StartDate,
                ReturnDate = rentalRequest.EndDate,
                IsReturned = false,
                UserId = userId,
                EquipmentId = availableEquipment.EquipmentId
            };

            try
            {
                _context.Rentals.Add(rental);
                await _context.SaveChangesAsync();
                Log.Information("Rental created: User {UserId} rented equipment {EquipmentId}", userId, availableEquipment.EquipmentId);
                return new CreatedResult(string.Empty, new { equipmentId = rental.EquipmentId });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while creating rental for User {UserId} with equipment {EquipmentId}", userId, availableEquipment.EquipmentId);
                return new BadRequestResult();
            }
        }

        public async Task<Equipment?> GetAvailableEquipment(int modelId)
        {
            var equipment = await _context.Equipments
                .FirstOrDefaultAsync(e => e.ModelId == modelId && !e.Rentals.Any(r => !r.IsReturned));

            if (equipment == null)
                Log.Warning("No available equipment found for model {ModelId}", modelId);
            else
                Log.Information("Available equipment found for model {ModelId}: Equipment {EquipmentId}", modelId, equipment.EquipmentId);

            return equipment;
        }

        public async Task<bool> EquipmentModelExists(int modelId)
        {
            var equipmentModel = await _context.Models
                                               .FirstOrDefaultAsync(em => em.ModelId == modelId);
            var exists = equipmentModel != null;
            Log.Information("Checked if equipment model {ModelId} exists: {Exists}", modelId, exists);
            return exists;
        }

        public async Task<List<RentalDTO>> GetAllRentalsAsync()
        {
            var rentals = await _context.Rentals
                .Include(r => r.User)
                .Include(r => r.Equipment)
                .ThenInclude(e => e.Model)
                .Select(r => new RentalDTO
                {
                    RentalId = r.RentalId,
                    RentalDate = r.RentalDate,
                    ReturnDate = r.ReturnDate,
                    IsReturned = r.IsReturned,
                    UserId = r.UserId,
                    UserEmail = r.User.Email,
                    EquipmentId = r.EquipmentId,
                    EquipmentModelName = r.Equipment.Model.Name,
                    EquipmentSerialNumber = r.Equipment.SerialNumber
                })
                .ToListAsync();

            Log.Information("Fetched {RentalCount} rentals", rentals.Count);
            return rentals;
        }

        public async Task<bool> ReturnRentalAsync(int rentalId)
        {
            var rental = await _context.Rentals.FirstOrDefaultAsync(r => r.RentalId == rentalId);
            if (rental == null || rental.IsReturned)
            {
                Log.Warning("Rental not found or already returned: {RentalId}", rentalId);
                return false;
            }

            rental.IsReturned = true;
            await _context.SaveChangesAsync();
            Log.Information("Rental with ID {RentalId} has been returned", rentalId);
            return true;
        }
    }
}
