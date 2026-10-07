import { BackflowTest } from "../backflow/backflow-test";

export interface PublicBackflowTestDetails extends BackflowTest {
    showWaterMeterNumber?: boolean;
    showRainSensor?: boolean;
    showOSSF?: boolean;
    showPermitNumber?: boolean;
}
