namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers
{
    public class LetterContactDto : IDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string ContactName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string FaxNumber { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public int? StateId { get; set; }
    }
}
