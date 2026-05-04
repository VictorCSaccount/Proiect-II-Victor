using AutoMapper;
using ProiectII.DTO.FoxManagement;
using ProiectII.Interfaces;
using ProiectII.Models;
using ProiectII.Repositories;

namespace ProiectII.Services.CoreDomain;

public class FoxService(
    IFoxRepository foxRepository,
    ILocationRepository locationRepository,
    IFileStorageService fileStorageService,
    IMapper mapper) : IFoxService
{
    public async Task<IEnumerable<FoxSummaryDto>> GetAllFoxesAsync()
    {
        var foxes = await foxRepository.GetFoxesWithDetailsAsync();
        // Filtrăm vulpile șterse (Soft Delete)
        return foxes.Where(f => !f.IsDeleted).Select(f => new FoxSummaryDto
        {
            Id = f.Id,
            Name = f.Name,
            ImageUrl = f.ImageUrl,
            StatusName = f.Status?.Name ?? "Necunoscut"
        });
    }

    public async Task<FoxDetailsDto?> GetFoxByIdAsync(uint id)
    {
        var fox = await foxRepository.GetFoxByIdWithDetailsAsync(id);
        if (fox == null || fox.IsDeleted) return null;

        return new FoxDetailsDto
        {
            Id = fox.Id,
            Name = fox.Name,
            Description = fox.Description,
            ImageUrl = fox.ImageUrl,
            StatusName = fox.Status?.Name ?? "Necunoscut",
            LastSeenLatitude = fox.FirstSeenLocation?.Coordinate != null ? (double)fox.FirstSeenLocation.Coordinate.Latitude : 0,
            LastSeenLongitude = fox.FirstSeenLocation?.Coordinate != null ? (double)fox.FirstSeenLocation.Coordinate.Longitude : 0
        };
    }

    public async Task<FoxDetailsDto> CreateFoxAsync(CreateFoxDto dto)
    {
        string? savedImageUrl = null;
        if (dto.Image != null && dto.Image.Length > 0)
        {
            savedImageUrl = await fileStorageService.SaveFileAsync(dto.Image, "foxes");
        }

        var newLocation = new Location
        {
            Name = $"Initial sighting: {dto.Name}",
            Coordinate = new Coordinate
            {
                Latitude = (decimal)dto.FirstSeenLatitude,
                Longitude = (decimal)dto.FirstSeenLongitude
            }
        };

        await locationRepository.AddAsync(newLocation);
        await locationRepository.SaveChangesAsync(); // Salvare locație pentru a obține ID

        var newFox = new Fox
        {
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = savedImageUrl,
            StatusId = dto.StatusId,
            EnclosureId = dto.EnclosureId,
            FirstSeenLocationId = newLocation.Id,
            IsDeleted = false
        };

        await foxRepository.AddAsync(newFox);
        await foxRepository.SaveChangesAsync(); // Salvare vulpe

        return new FoxDetailsDto
        {
            Id = newFox.Id,
            Name = newFox.Name,
            Description = newFox.Description,
            ImageUrl = newFox.ImageUrl,
            LastSeenLatitude = dto.FirstSeenLatitude,
            LastSeenLongitude = dto.FirstSeenLongitude
        };
    }

    public async Task<bool> UpdateFoxAsync(uint foxId, UpdateFoxDto dto)
    {
        var fox = await foxRepository.GetFoxByIdWithDetailsAsync(foxId);
        if (fox == null || fox.IsDeleted) return false;

        fox.Name = dto.Name;
        fox.Description = dto.Description;
        fox.EnclosureId = dto.EnclosureId;

        if (fox.FirstSeenLocation != null)
        {
            fox.FirstSeenLocation.Coordinate.Latitude = (decimal)dto.Latitude;
            fox.FirstSeenLocation.Coordinate.Longitude = (decimal)dto.Longitude;
        }

        foxRepository.Update(fox);
        return await foxRepository.SaveChangesAsync();
    }

    public async Task<bool> UpdateFoxStatusAsync(uint foxId, UpdateFoxStatusDto dto)
    {
        var fox = await foxRepository.GetByIdAsync(foxId);
        if (fox == null || fox.IsDeleted) return false;

        fox.StatusId = dto.NewStatusId;

        foxRepository.Update(fox);
        return await foxRepository.SaveChangesAsync();
    }

    public async Task<bool> ArchiveFoxAsync(uint foxId)
    {
        var fox = await foxRepository.GetByIdAsync(foxId);
        if (fox == null) return false;

        fox.IsDeleted = true; 

        foxRepository.Update(fox);
        return await foxRepository.SaveChangesAsync();
    }


    public async Task<IEnumerable<FoxMapMarkerDto>> GetMapMarkersAsync()
    {
        var foxes = await foxRepository.GetFoxesWithDetailsAsync();

        var markers = mapper.Map<IEnumerable<FoxMapMarkerDto>>(foxes);

        return markers.Where(m => m.Latitude != 0 && m.Longitude != 0);
    }

}