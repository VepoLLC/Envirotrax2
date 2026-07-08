import { OutOfServiceType } from './out-of-service-type.enum';

export interface BackflowOutOfServiceRequest {
    id?: number;
    bpatId?: number;
    testId: number;
    type?: OutOfServiceType;
    description?: string;
    replacementAssemblyTestId?: number | null;
    outOfServiceDate?: string | null;
    clearedDate?: string | null;
}
