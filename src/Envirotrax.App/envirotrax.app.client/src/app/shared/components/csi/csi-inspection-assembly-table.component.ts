import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { BackflowTestResult, BYPASS_DEVICE_TYPES } from '../../models/backflow/backflow-test-enums';
import { CsiInspectionAssembly } from '../../models/csi/csi-inspection-assembly';

interface AssemblyRowVm {
    assembly: CsiInspectionAssembly;
    isCurrent: boolean;
    isPassing: boolean;
    inService: boolean;
    isPaid: boolean;
    hasBypass: boolean;
    hazardDescription: string;
}

// The "Assemblies at This Location" table. On the inspector's form (`editable`) the Visually Identified
// checkboxes can be changed and unpaid rows deleted; elsewhere it is read-only and paid rows link to their test.
@Component({
    selector: 'vp-csi-inspection-assembly-table',
    standalone: false,
    templateUrl: './csi-inspection-assembly-table.component.html'
})
export class CsiInspectionAssemblyTableComponent implements OnChanges {
    @Input() public assemblies: CsiInspectionAssembly[] = [];
    @Input() public editable = false;

    @Output() public delete = new EventEmitter<CsiInspectionAssembly>();

    public rows: AssemblyRowVm[] = [];

    public ngOnChanges(): void {
        this.rows = this.assemblies.map(assembly => ({
            assembly,
            isCurrent: !!assembly.isCurrent,
            isPassing: assembly.testResult === BackflowTestResult.Pass,
            inService: !assembly.outOfService,
            isPaid: !!assembly.transactionId,
            hasBypass: BYPASS_DEVICE_TYPES.includes(assembly.deviceType ?? ''),
            hazardDescription: this.buildHazardDescription(assembly)
        }));
    }

    private buildHazardDescription(assembly: CsiInspectionAssembly): string {
        if (!assembly.hazardType) {
            return 'Unknown';
        }

        return assembly.hazardType === 'Other' ? `Other - ${assembly.hazardTypeOtherDescription ?? ''}` : assembly.hazardType;
    }
}
