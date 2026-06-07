using System.Text.Json.Serialization;

namespace API.Entities;

public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; } = null!;
    public string? PublicId { get; set; } = null!;

  
  
     // Navigation property
     [JsonIgnore]
     public Member Member { get; set; } = null!;
     public string MemberId { get; set; } = null!;
}
