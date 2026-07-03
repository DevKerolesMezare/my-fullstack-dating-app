using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController(IUnitOfWork uow) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
    {
        var sender= await uow.MemberRepository.GetMemberByIdAsync(User.GetMemberId());
        var recipient  = await uow.MemberRepository.GetMemberByIdAsync(createMessageDTO.RecipientId);

        if(recipient == null|| sender == null ||sender.Id == createMessageDTO.RecipientId)
             return BadRequest("Connot send this message");

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMessageDTO.Content
        };

        uow.MessageRepository.AddMessages(message);
        if(await uow.Complete()) return message.ToDto();

        return BadRequest("Faild to send message");
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MessageDTO>>> GetMessagesByContainer
    ([FromQuery] MessageParams messageParams)
    {
        messageParams.MemberId = User.GetMemberId();

        return await uow.MessageRepository.GetMessagesForMembers(messageParams);
    }
    
    [HttpGet("thread/{recipientId}")]
    public async Task<ActionResult<IReadOnlyList<MessageDTO>>> GetMessageThread(string recipientId)
    {
        return  Ok(await uow.MessageRepository.GetMessageThread(User.GetMemberId(),recipientId));
    }


    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(string id)
    {
        var memberId =User.GetMemberId();

        var message = await uow.MessageRepository.GetMessage(id);

        if(message == null) return BadRequest("connot delete this message");

        if(message.SenderId != memberId && message.RecipientId != memberId)
            return BadRequest("connot delete this message");

        if(message.SenderId == memberId) message.SenderDeleted = true;
        if(message.RecipientId == memberId) message.RecipentDeleted = true;

        if(message is {SenderDeleted: true, RecipentDeleted: true })
        {
            uow.MessageRepository.DeleteMessge(message);
        }

        if(await uow.Complete()) return Ok();

        return BadRequest("problem deleting the message");
    }

}


