import { BackflowTest } from "./backflow-test";
import { SiteLog } from "../sites/site-log";

// A Backflow Compliance Management row: an expired assembly (backflow test) plus its site's logs.
// Mirrors the backend BackflowComplianceDto (BackflowTestDto + Logs).
export interface BackflowCompliance extends BackflowTest {
    logs?: SiteLog[];
}
