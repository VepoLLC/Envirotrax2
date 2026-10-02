using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.Common.Data.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Professionals;

[Table("ProfessionalTransactions")]
[AppIndex(nameof(ProfessionalId), nameof(TransactionId), IsUnique = true)]
public class ProfessionalTransaction : IProfessionalModel
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public DateTime TransactionDate { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    public int UserId { get; set; }
    public ProfessionalUser? User { get; set; }

    [StringLength(100)]
    public string? TransactionId { get; set; }

    public ProfessionalTransactionType TransactionType { get; set; }

    [StringLength(500)]
    public string? ReferenceDescription { get; set; }

    [Precision(19, 4)]
    public decimal BalanceAdjustment { get; set; }

    [Precision(19, 4)]
    public decimal CardCharge { get; set; }

    [Precision(19, 4)]
    public decimal Amount { get; set; }

    [Precision(19, 4)]
    public decimal AmountShare { get; set; }

    [StringLength(255)]
    public string? NameOnCard { get; set; }

    [StringLength(25)]
    public string? CardNumber { get; set; }
}

public class ProfessionalTransactionConfiguration : IEntityTypeConfiguration<ProfessionalTransaction>
{
    public void Configure(EntityTypeBuilder<ProfessionalTransaction> builder)
    {
        builder.HasOne(transaction => transaction.Professional)
            .WithMany()
            .HasForeignKey(transaction => transaction.ProfessionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(transaction => transaction.User)
            .WithMany()
            .HasForeignKey(transaction => new { transaction.ProfessionalId, transaction.UserId })
            .HasPrincipalKey(user => new { user.ProfessionalId, user.UserId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
