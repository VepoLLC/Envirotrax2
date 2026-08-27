
namespace Envirotrax.TaskRunner;

public class QueueNames
{
    public class Sites
    {
        public const string Geocode = "sites-geocode";
    }

    public class BackflowTests
    {
        public const string ProcessSiteRenewal = "backflow-tests-process-site-renewal";
        public const string ProcessTestRenewal = "backflow-tests-process-test-renewal";
    }

    public class BackflowComplianceSnapshots
    {
        public const string Process = "backflow-compliance-snapshots-process";
        public const string Backfill = "backflow-compliance-snapshots-backfill";
    }
}