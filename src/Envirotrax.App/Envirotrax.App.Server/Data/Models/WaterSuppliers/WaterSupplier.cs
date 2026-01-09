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

    [ForeignKey(nameof(ParentId))]
    public WaterSupplier? Parent { get; set; }

    [ForeignKey(nameof(LetterAddressId))]
    public int? LetterAddressId { get; set; }
    public LetterAddress? LetterAddress { get; set; } = null!;

    [ForeignKey(nameof(LetterContactId))]
    public int? LetterContactId { get; set; }
    public LetterContact? LetterContact { get; set; } = null!;

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