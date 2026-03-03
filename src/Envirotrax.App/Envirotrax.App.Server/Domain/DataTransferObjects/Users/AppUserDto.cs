namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users
{
    public class AppUserDto : IDto
    {
        public int Id { get; set; }
        
        public string? Email { get; set; }
    }
}
