import { OutOfServiceType } from './out-of-service-type.enum';
import { BackflowTest } from './backflow-test';

export interface BackflowOutOfServiceRequest {
    id?: number;
    bpatId?: number;
    testId: number;
    type?: OutOfServiceType;
    description?: string;
    replacementAssemblyTestId?: number | null;
    outOfServiceDate?: string | null;
    clearedDate?: string | null;
    test?: BackflowTest;
    replacementAssemblyTest?: BackflowTest;
}
