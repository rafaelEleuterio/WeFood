namespace Infraestructure.Identity;
public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }   // apenas o Id
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }

    public ApplicationUser User { get; set; }
}