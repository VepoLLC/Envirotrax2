
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers
{
    public class WaterSupplierDto : IDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string Domain { get; set; } = null!;

        public string ContactName { get; set; } = null!;
        public string? PwsId { get; set; } = null!;

        public string? Address { get; set; } = null!;
        public string? City { get; set; } = null!;
        public string? ZipCode { get; set; } = null!;

        public string? PhoneNumber { get; set; } = null!;
        public string? FaxNumber { get; set; } = null!;
        public string? EmailAddress { get; set; } = null!;
        public ReferencedStateDto? State { get; set; }

        public string? LetterCompanyName { get; set; } = null!;
        public string? LetterContactName { get; set; } = null!;
        public string? LetterAddress { get; set; } = null!;
        public string? LetterCity { get; set; } = null!;
        public string? LetterZipCode { get; set; } = null!;
        public ReferencedStateDto? LetterState { get; set; }

        public string? LetterContactCompanyName { get; set; } = null!;
        public string? LetterContactContactName { get; set; } = null!;
        public string? LetterContactAddress { get; set; } = null!;
        public string? LetterContactCity { get; set; } = null!;
        public string? LetterContactZipCode { get; set; } = null!;
        public string? LetterContactPhoneNumber { get; set; } = null!;
        public string? LetterContactFaxNumber { get; set; } = null!;
        public string? LetterContactEmailAddress { get; set; } = null!;
        public ReferencedStateDto? LetterContactState { get; set; }

        public double? GisCenterLatitude { get; set; }
        public double? GisCenterLongitude { get; set; }
        public double? GisCenterZoom { get; set; }

        public ReferencedWaterSupplierDto? Parent { get; set; }
    }

    public class ReferencedWaterSupplierDto
    {
        [Required]
        public int? Id { get; set; }

        public string? Name { get; set; }

        public string? Domain { get; set; }

        public string? ContactName { get; set; }
        public string? PwsId { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public ReferencedStateDto? State { get; set; }
        public string? ZipCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
    }
}