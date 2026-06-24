using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{
    void AddMessages(Message message);

    void DeleteMessge(Message message);
    Task<Message?> GetMessage(string messageId);
    Task<PaginatedResult<MessageDTO>> GetMessagesForMembers(MessageParams messageParams);
    Task<IReadOnlyList<MessageDTO>> GetMessageThread(string currentMemeberId , string recipientId);
    Task<bool> SaveAllAsync();
}
