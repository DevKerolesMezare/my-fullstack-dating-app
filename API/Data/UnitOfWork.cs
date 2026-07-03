using API.DTOs;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IMemberRepository? _membersRepository;
    private IMessageRepository? _messagesRepository;
    private ILikesRepository? _likesRepository;  

    public IMemberRepository MemberRepository => _membersRepository ??= new MemberRepository(context);

    public IMessageRepository MessageRepository => _messagesRepository ??= new MessageRepository(context);
    
    public ILikesRepository LikesRepository => _likesRepository ??= new LikesRepository(context);

    public async Task<bool> Complete()
    {
        try
        {
          return await context.SaveChangesAsync() > 0;
        }
        catch(DbUpdateException ex)
        {
            throw new Exception($"An error occurred while saving changes: {ex.Message}", ex);
        }
    }

    public bool HasChanges()
    {
        return context.ChangeTracker.HasChanges();
    }
}
