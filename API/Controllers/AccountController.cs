using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers;

public class AccountController(AppDbContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if(await EmailExists(registerDTO.Email)) return BadRequest("Email Taken");

        var hmac = new HMACSHA512();

        var user = new AppUser
        {
            DisplayName = registerDTO.DisplayName,
            Email = registerDTO.Email , 
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key,
            Member = new Member
            {
                DisplayName = registerDTO.DisplayName,
                Gender = registerDTO.Gender,
                City = registerDTO.City,
                Country =registerDTO.Country,
                DateOfBirth =registerDTO.DateOfBirth
            }
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user.ToDTO(tokenService);
    }


    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(loginDTO login)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Email == login.Email);

        if(user == null) return Unauthorized("Invaild email address");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHush = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

        for(var i = 0 ; i< computedHush.Length ; i++)
        {
            if(computedHush[i] != user.PasswordHash[i]) return Unauthorized("Invaild password");
        }

     return user.ToDTO(tokenService);
    }


    public async Task<bool> EmailExists(string email) => await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
}
