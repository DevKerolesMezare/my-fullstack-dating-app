using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.DTOs;

public class MessageRepository(AppDbContext context) : IMessageRepository
{
    public void AddMessages(Message message) => context.Messages.Add(message);

    public void DeleteMessge(Message message) => context.Messages.Remove(message);

    public async Task<Message?> GetMessage(string messageId)
    {
    return await context.Messages
        .FirstOrDefaultAsync(x => x.Id == messageId);
    }


    public async Task<PaginatedResult<MessageDTO>> GetMessagesForMembers(MessageParams messageParams)
    {
        var query = context.Messages
        .OrderByDescending(x => x.MessageSent)
        .AsQueryable();

        query = messageParams.Container switch
        {
            "Outbox" => query.Where(x => x.SenderId == messageParams.MemberId 
            && x.SenderDeleted == false),
            _ => query.Where(x => x.RecipientId == messageParams.MemberId && x.RecipentDeleted == false)
        };

        var messageQuery = query.Select(MessageExtensions.ToDtoProjection());
        return await PaginationHelper.CreateAsync
        (messageQuery,messageParams.PageNumber,messageParams.PageSize);
    }

    public async Task<IReadOnlyList<MessageDTO>> GetMessageThread(string currentMemeberId, string recipientId)
    {
        await context.Messages
            .Where(x => x.RecipientId == currentMemeberId
                && x.RecipentDeleted == false
                && x.SenderId == recipientId
                && x.DateRead == null)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(x => x.DateRead, DateTime.UtcNow));

                return await context.Messages
                .Where(x => (x.RecipientId == currentMemeberId 
                && x.SenderId == recipientId)

                    || (x.SenderId == currentMemeberId  &&x.SenderDeleted == false && x.RecipientId == recipientId))
                    .OrderBy(x => x.MessageSent)
                    .Select(MessageExtensions.ToDtoProjection())
                    .ToListAsync();

                
    }

    public async Task<bool> SaveAllAsync() => await context.SaveChangesAsync() > 0;

}
