using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class loginDTO
{
    [EmailAddress]
    public string Email {get; set;}

    public string Password {get;set;}
}
