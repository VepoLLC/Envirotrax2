using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.LegacyDataMigration.Data.Backflow;

// Only the columns the image migration reads and writes. The full model lives in Envirotrax.App.Server;
// rows are inserted by Scripts/BackflowTests/02_BackflowTests.sql, never by EF, so the columns that
// script alone fills in are deliberately absent here.
public class BackflowTest : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }

    [MaxLength(500)]
    public string? AssemblyImagePath { get; set; }

    [MaxLength(500)]
    public string? SerialNumberImagePath { get; set; }

    [MaxLength(500)]
    public string? BypassAssemblyImagePath { get; set; }

    [MaxLength(500)]
    public string? BypassSerialNumberImagePath { get; set; }

    [MaxLength(500)]
    public string? AirGapImagePath { get; set; }

    // Points back to Vepo.dbo.SaveBackflowDeviceTests.ID. It names the blobs.
    public int? LegacyRecordId { get; set; }

    // Where each image sits underneath the legacy file server root, until it is copied into Azure Storage.
    [MaxLength(500)]
    public string? LegacyAssemblyImagePath { get; set; }

    [MaxLength(500)]
    public string? LegacySerialNumberImagePath { get; set; }

    [MaxLength(500)]
    public string? LegacyBypassAssemblyImagePath { get; set; }

    [MaxLength(500)]
    public string? LegacyBypassSerialNumberImagePath { get; set; }

    [MaxLength(500)]
    public string? LegacyAirGapImagePath { get; set; }

    // True when a water supplier saved the test in V1. V1 keeps no record of an image uploaded from its
    // supplier pages, so some of this test's legacy paths are guesses, and finding no file is expected.
    public bool? LegacyImagesUnrecorded { get; set; }
}
