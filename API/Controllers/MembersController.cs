using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

    [Authorize]
public class MembersController(IMemberRepository memberRepository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers()
    {
        return Ok(await memberRepository.GetMembersAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Member>> GetMember(string id )
    {
        var member = await memberRepository.GetMemberByIdAsync(id);
        if(member == null) return NotFound();

        return member;
    }


    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
    {
        var photos = await memberRepository.GetMemberByPhotosAsync(id);
        if(photos == null) return NotFound();

        return Ok(photos);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateMember(MemberUpdateDTO memberUpdateDTO)
    {
        var memberId = User.GetMemberId();

        if(memberId == null) return BadRequest("No member id found in token");

        var member = await memberRepository.GetMemberForUpdate(memberId);

        if(member == null )return BadRequest("Could not get member");

        member.DisplayName = memberUpdateDTO.DisplayName ?? member.DisplayName;
        member.Description = memberUpdateDTO.Discription ?? member.Description;
        member.City = memberUpdateDTO.City ?? member.City;
        member.Country = memberUpdateDTO.Country ?? member.Country;

        member.User.DisplayName = memberUpdateDTO.DisplayName ?? member.User.DisplayName;

        memberRepository.Update(member); 


        if(await memberRepository.SaveAllAsync()) return NoContent();


        return BadRequest("Faild to update member");
    }

}
