using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs;

public class PhotoRepository(AppDbContext context) : IPhotoRepository
{
    public Task<Photo?> GetPhotoById(int id)
    {
        return context.Photos.IgnoreQueryFilters()
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<PhotoForApprovalDTO>> GetUnapprovedPhotos()
    {
        return await context.Photos
           .IgnoreQueryFilters()
           .Where(p => p.IsApproved == false)
           .Select(u => new PhotoForApprovalDTO
           {
               Id = u.Id,
               UserId = u.MemberId,
               Url = u.Url,
               IsApproved = u.IsApproved
           }).ToListAsync();
    }

    public void RemovePhoto(Photo photo)
    {
        context.Photos.Remove(photo);
    }
}
