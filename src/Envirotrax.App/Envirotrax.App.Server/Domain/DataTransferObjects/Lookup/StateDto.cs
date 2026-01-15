namespace Envirotrax.App.Server.Domain.DataTransferObjects.Lookup
{
    public class StateDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
