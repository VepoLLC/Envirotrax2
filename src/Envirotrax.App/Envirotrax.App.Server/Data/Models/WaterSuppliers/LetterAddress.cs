using Envirotrax.App.Server.Data.Models.States;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers
{
    /// <summary>
    /// Letter Return Address Information
    /// </summary>
    public class LetterAddress
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string ContactName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public int? StateId { get; set; }
        public State? State { get; set; }
    }
}
