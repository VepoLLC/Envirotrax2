using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Professionals;

[Table("ProfessionalTransactions")]
public class ProfessionalTransaction : IProfessionalModel
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public DateTime TransactionDate { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    public int UserId { get; set; }
    public AppUser? User { get; set; }

    [Required]
    [StringLength(100)]
    public string TransactionId { get; set; } = null!;

    public ProfessionalTransactionType TransactionType { get; set; }

    [StringLength(500)]
    public string? ReferenceDescription { get; set; }

    [Precision(19, 4)]
    public decimal BalanceAdjustment { get; set; }

    [Precision(19, 4)]
    public decimal CcCharge { get; set; }

    [Precision(19, 4)]
    public decimal Amount { get; set; }

    [Precision(19, 4)]
    public decimal AmountShare { get; set; }

    [StringLength(255)]
    public string? CCNameOnCard { get; set; }

    [StringLength(25)]
    public string? CCNumber { get; set; }
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
            .HasForeignKey(transaction => transaction.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(transaction => transaction.TransactionId)
            .IsUnique()
            .HasFilter("[TransactionId] <> ''");
    }
}
