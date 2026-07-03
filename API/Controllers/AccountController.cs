using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        var user = new AppUser
        {
            DisplayName = registerDTO.DisplayName,
            Email = registerDTO.Email , 
            UserName = registerDTO.Email,
            Member = new Member
            {
                DisplayName = registerDTO.DisplayName,
                Gender = registerDTO.Gender,
                City = registerDTO.City,
                Country =registerDTO.Country,
                DateOfBirth =registerDTO.DateOfBirth
            }
        };

        var result = await userManager.CreateAsync(user, registerDTO.Password);

        if(!result.Succeeded)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("identity", error.Description);
            }

            return ValidationProblem();
        }

        await userManager.AddToRoleAsync(user, "Member");
        
        await SetRefreshTokenCookie(user);

        return await user.ToDTO(tokenService);
    }


    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(loginDTO login)
    {
        var user = await userManager.FindByEmailAsync(login.Email);

        if(user == null) return Unauthorized("Invaild email address");
   
        var result = await userManager.CheckPasswordAsync(user, login.Password);

        if(!result)
            return Unauthorized("Invalid password");

        await SetRefreshTokenCookie(user);

        return await user.ToDTO(tokenService);
    }


    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDTO>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if(refreshToken == null) return NoContent();

        var user = await userManager.Users
        .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken  
        && x.RefreshTokenExpiry > DateTime.UtcNow);

        if(user == null) return Unauthorized();

        await SetRefreshTokenCookie(user);

        return  await user.ToDTO(tokenService);
    }

    private async Task SetRefreshTokenCookie(AppUser user)
    {
        var refreshToken= tokenService.GenerateRefreshToken();
        user.RefreshToken =refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure =true,
            SameSite =SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }



    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await userManager.Users
            .Where(x => x.Id == User.GetMemberId())
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.RefreshToken, _ => null)
                .SetProperty(u => u.RefreshTokenExpiry, _ => null)
                );

        Response.Cookies.Delete("refreshToken");

        return Ok();
    }
}
