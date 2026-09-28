using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.LegacyDataMigration.Data.Professionals;


public class ProfessionalInsurance
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }

    public string FilePath { get; set; } = null!;

    public int? LegacyRecordId { get; set; }

    // Where the policy file sits underneath the legacy file server root, until it is copied into Azure Storage.
    [MaxLength(500)]
    public string? LegacyFilePath { get; set; }
}
