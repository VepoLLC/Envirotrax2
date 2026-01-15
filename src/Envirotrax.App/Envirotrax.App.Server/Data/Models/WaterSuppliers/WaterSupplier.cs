using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Pkcs;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers;

public class WaterSupplier : TenantBase, IAuditableModel<AppUser>
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string Domain { get; set; } = null!;

    public string ContactName { get; set; } = null!;
    public string PwsId { get; set; } = null!;
   
    public string Address { get; set; } = null!;
    public string City { get; set; } = null!;
    public string ZipCode { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
    public string FaxNumber { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public int? StateId { get; set; }
    public State? State { get; set; }

    public string LetterCompanyName { get; set; } = null!;
    public string LetterContactName { get; set; } = null!;
    public string LetterAddress { get; set; } = null!;
    public string LetterCity { get; set; } = null!;
    public string LetterZipCode { get; set; } = null!;
    public int? LetterStateId { get; set; }
    public State? LetterState { get; set; }

    public string LetterContactCompanyName { get; set; } = null!;
    public string LetterContactContactName { get; set; } = null!;
    public string LetterContactAddress { get; set; } = null!;
    public string LetterContactCity { get; set; } = null!;
    public string LetterContactZipCode { get; set; } = null!;
    public string LetterContactPhoneNumber { get; set; } = null!;
    public string LetterContactFaxNumber { get; set; } = null!;
    public string LetterContactEmailAddress { get; set; } = null!;
    public int? LetterContactStateId { get; set; }
    public State? LetterContactState { get; set; }

    [ForeignKey(nameof(ParentId))]
    public WaterSupplier? Parent { get; set; }

    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public int? UpdatedById { get; set; }
    public AppUser? UpdatedBy { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public int? DeletedById { get; set; }
    public AppUser? DeletedBy { get; set; }
    public DateTime? DeletedTime { get; set; }
}