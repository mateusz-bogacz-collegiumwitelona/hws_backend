using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class User : IdentityUser<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdateAt { get; set; }

    public bool IsDeleted { get; set; }
    
    public ICollection<Sensor> Sensors { get; set; } = new List<Sensor>();
}