import { BackflowTest } from "./backflow-test";
import { SiteLog } from "../sites/site-log";
import { ComplianceOverdueSeverity } from "../../enums/compliance-overdue-severity.enum";

// A Backflow Compliance Management row: an expired assembly (backflow test) plus its site's logs.
// Mirrors the backend BackflowComplianceDto (BackflowTestDto + Logs + the expiry figures).
export interface BackflowCompliance extends BackflowTest {
    logs?: SiteLog[];

    // Computed server-side against the caller's local time.
    daysExpired?: number;
    expiredSeverity?: ComplianceOverdueSeverity;
}
