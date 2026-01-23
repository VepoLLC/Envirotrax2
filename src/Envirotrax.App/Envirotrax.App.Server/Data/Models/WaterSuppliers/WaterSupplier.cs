using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers;

public class WaterSupplier : TenantBase, IAuditableModel<AppUser>
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Domain { get; set; } = null!;

    [StringLength(100)]
    public string ContactName { get; set; } = null!;

    [StringLength(25)]
    public string? PwsId { get; set; } = null!;

    [StringLength(100)]
    public string? Address { get; set; } = null!;

    [StringLength(50)]
    public string? City { get; set; } = null!;

    [StringLength(50)]
    public string? ZipCode { get; set; } = null!;

    [StringLength(25)]
    public string? PhoneNumber { get; set; } = null!;

    [StringLength(25)]
    public string? FaxNumber { get; set; } = null!;

    [StringLength(100)]
    public string? EmailAddress { get; set; } = null!;
    public int? StateId { get; set; }
    public State? State { get; set; }

    [StringLength(255)]
    public string? LetterCompanyName { get; set; } = null!;

    [StringLength(100)]
    public string? LetterContactName { get; set; } = null!;

    [StringLength(100)]
    public string? LetterAddress { get; set; } = null!;

    [StringLength(50)]
    public string? LetterCity { get; set; } = null!;

    [StringLength(50)]
    public string? LetterZipCode { get; set; } = null!;
    public int? LetterStateId { get; set; }
    public State? LetterState { get; set; }

    [StringLength(100)]
    public string? LetterContactCompanyName { get; set; } = null!;

    [StringLength(100)]
    public string? LetterContactContactName { get; set; } = null!;

    [StringLength(100)]
    public string? LetterContactAddress { get; set; } = null!;

    [StringLength(50)]
    public string? LetterContactCity { get; set; } = null!;

    [StringLength(50)]
    public string? LetterContactZipCode { get; set; } = null!;

    [StringLength(25)]
    public string? LetterContactPhoneNumber { get; set; } = null!;

    [StringLength(25)]
    public string? LetterContactFaxNumber { get; set; } = null!;

    [StringLength(100)]
    public string? LetterContactEmailAddress { get; set; } = null!;
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