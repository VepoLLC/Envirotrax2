
using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.LegacyDataMigration.Data.Fog;

// Only the columns the image migration reads and writes. The full model lives in Envirotrax.App.Server;
// rows are inserted by Scripts/FogInspections/02_FogInspections.sql, never by EF, so the columns that
// script alone fills in are deliberately absent here.
public class FogInspection : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }

    [MaxLength(500)]
    public string? ExteriorImagePath { get; set; }

    [MaxLength(500)]
    public string? InteriorImagePath { get; set; }

    [MaxLength(500)]
    public string? SignatureImagePath { get; set; }

    // Points back to Vepo.dbo.FogInspections.ID. It names the blobs, and it is what the signature is
    // read back from the legacy database by.
    public int? LegacyRecordId { get; set; }

    // Where the exterior and interior photos sit underneath the legacy file server root, until they are
    // copied into Azure Storage.
    [MaxLength(500)]
    public string? LegacyExteriorImagePath { get; set; }

    [MaxLength(500)]
    public string? LegacyInteriorImagePath { get; set; }

    // V1 keeps the signature as bytes in its own table rather than as a file, so all that can be
    // recorded is that there is one to copy.
    public bool? HasLegacySignature { get; set; }
}
