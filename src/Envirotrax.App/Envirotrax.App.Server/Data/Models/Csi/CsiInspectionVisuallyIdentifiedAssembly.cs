using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Csi;

[Table("CsiInspectionVisuallyIdentifiedAssemblies")]
public class CsiInspectionVisuallyIdentifiedAssembly : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int InspectionId { get; set; }
    public CsiInspection? Inspection { get; set; }

    public int? TestId { get; set; }
    public BackflowTest? Test { get; set; }

    [StringLength(50)]
    public string? SubmissionId { get; set; }

    public bool VisuallyIdentified { get; set; }

    [StringLength(50)]
    public string? DeviceType { get; set; }

    [StringLength(200)]
    public string? AssemblyDescription { get; set; }

    [StringLength(50)]
    public string? SerialNumber { get; set; }

    [StringLength(200)]
    public string? AssemblyDescription2 { get; set; }

    [StringLength(50)]
    public string? SerialNumber2 { get; set; }

    [StringLength(50)]
    public string? HazardType { get; set; }

    [StringLength(200)]
    public string? HazardTypeOtherDescription { get; set; }

    [StringLength(500)]
    public string? LocationDescription { get; set; }

    public bool IsCurrent { get; set; }

    public BackflowTestResult TestResult { get; set; }

    public bool OutOfService { get; set; }

    public DateTime? TestDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [StringLength(100)]
    public string? TransactionId { get; set; }

    public DateTime CreatedTime { get; set; }
}
