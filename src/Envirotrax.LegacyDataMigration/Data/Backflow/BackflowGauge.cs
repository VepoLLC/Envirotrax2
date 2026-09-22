using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.LegacyDataMigration.Data.Backflow;

// Only the columns the file migration reads and writes. The full model lives in Envirotrax.App.Server;
// rows are inserted by Scripts/BackflowGauges/02_BackflowGauges.sql, never by EF, so the columns that
// script alone fills in are deliberately absent here.
public class BackflowGauge
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }

    public string? FilePath { get; set; }

    // Points back to Vepo.dbo.SaveBpatGauges.ID. It names the blob, because it survives a wipe and
    // rebuild of the V2 database while the identity value in Id does not.
    public int? LegacyRecordId { get; set; }

    // Where the Test for Accuracy report sits underneath the legacy file server root, until it is
    // copied into Azure Storage.
    [MaxLength(500)]
    public string? LegacyFilePath { get; set; }
}
