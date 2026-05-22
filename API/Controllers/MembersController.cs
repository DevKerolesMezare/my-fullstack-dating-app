using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class MembersController(AppDbContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
    {
        return await context.Users.ToListAsync();
    }

    [Authorize]
    [HttpGet("{Id}")]
    public async Task<ActionResult<AppUser>> GetMember(string id )
    {
        var member = await context.Users.FindAsync(id); 
        if(member == null) return NotFound();

        return member;
    }

}
