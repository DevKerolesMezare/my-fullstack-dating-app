using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs;

public class MemberRepository(AppDbContext context) : IMemberRepository
{

        
        public void Update(Member member)
        {
            context.Entry(member).State = EntityState.Modified;            
        }
    
        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    
        public async Task<IReadOnlyList<Member>> GetMembersAsync()
        {
         return await context.Members
         .ToListAsync();
        }
    
        public async Task<Member?> GetMemberByIdAsync(string id)
        {
            return await context.Members.FindAsync(id);
        }
    
        public async Task<IReadOnlyList<Photo>> GetMemberByPhotosAsync(string memberId)
        {
            return await context.Members
            .Where(x => x.Id == memberId)
            .SelectMany(x => x.Photos)
            .ToListAsync();
        }

    public async Task<Member?> GetMemberForUpdate(string id)
    {
        return await context.Members.Include(x => x.User)
        .Include(x => x.Photos)
        .SingleOrDefaultAsync(x => x.Id == id);
    }

}
