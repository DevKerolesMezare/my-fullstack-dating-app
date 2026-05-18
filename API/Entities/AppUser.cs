namespace API.Entities;

public class AppUser
{
    public string Id {set;get;} = Guid.NewGuid().ToString();    
    public required string DisplayName{ set ;    get;}
    public required string Email{get;set;}
}
