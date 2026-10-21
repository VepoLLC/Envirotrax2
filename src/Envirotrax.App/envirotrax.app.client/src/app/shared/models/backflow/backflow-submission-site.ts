import { Site } from "../sites/site";
import { BackflowTest } from "./backflow-test";

export interface BackflowSubmissionSite extends Site {
    includeWsAccountNumbers?: boolean;
    assemblies?: BackflowTest[];
}
