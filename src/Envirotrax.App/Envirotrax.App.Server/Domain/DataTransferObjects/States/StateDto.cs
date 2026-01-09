namespace Envirotrax.App.Server.Domain.DataTransferObjects.States
{
    public class StateDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
