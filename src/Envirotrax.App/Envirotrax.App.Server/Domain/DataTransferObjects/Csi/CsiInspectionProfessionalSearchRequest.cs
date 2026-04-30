namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectionProfessionalSearchRequest
{
    public bool LatestOnly { get; set; } = true;
    public CsiInspectionResultFilter? PassFail { get; set; }
    public CsiInspectionDateType? DateType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public enum CsiInspectionDateType
{
    Inspection = 1,
    Submission = 2
}

public enum CsiInspectionResultFilter
{
    Pass = 1,
    Fail = 2
}
