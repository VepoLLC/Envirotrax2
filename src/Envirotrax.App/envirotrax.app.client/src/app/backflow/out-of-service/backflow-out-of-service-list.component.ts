import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { InputOption, ModalHelperService } from '@envirotrax/common-ui';
import { BackflowTest } from '../../shared/models/backflow/backflow-test';
import { BackflowTestResult } from '../../shared/models/backflow/backflow-test-enums';
import { BackflowOutOfServiceRequest } from '../../shared/models/backflow/backflow-out-of-service-request';
import { OutOfServiceType } from '../../shared/models/backflow/out-of-service-type.enum';
import { OutOfServiceRequestStatusFilter } from '../../shared/models/backflow/out-of-service-request-status-filter.enum';
import { BackflowOutOfServiceRequestService } from '../../shared/services/backflow/backflow-out-of-service-request.service';
import { ToastService } from '../../shared/services/toast.service';

// One assembly block (submitted or replacement) inside a request card. All display
// values are pre-computed; `test` is kept only for the status-icon field reads + View.
interface AssemblyLineVm {
    label: string;
    test?: BackflowTest;
    submittedDate?: string;
    testDate?: string;
    expirationDate?: string;
    serial: string;
    isBypassDevice: boolean;
    bypassSerial?: string;
    assemblyDescription: string;
    hazard: string;
    location: string;
    property: string;
    bpat: string;
}

interface OutOfServiceRequestVm {
    id: number;
    submittedBy: string;
    cleared: boolean;
    description?: string;
    submitted: AssemblyLineVm;
    replacement?: AssemblyLineVm;
}

@Component({
    standalone: false,
    templateUrl: './backflow-out-of-service-list.component.html'
})
export class BackflowOutOfServiceListComponent implements OnInit {
    public readonly BackflowTestResult = BackflowTestResult;

    public isLoading: boolean = false;
    public status: OutOfServiceRequestStatusFilter = OutOfServiceRequestStatusFilter.AllUncleared;
    public typeFilter: string = '';
    public requests: OutOfServiceRequestVm[] = [];

    public readonly statusOptions: InputOption[] = [
        { id: OutOfServiceRequestStatusFilter.AllUncleared.toString(), text: 'All uncleared requests' },
        { id: OutOfServiceRequestStatusFilter.MarkedOutOfService.toString(), text: 'Requests marked as out of service' },
        { id: OutOfServiceRequestStatusFilter.Cleared.toString(), text: 'Cleared requests' },
        { id: OutOfServiceRequestStatusFilter.All.toString(), text: 'All requests' }
    ];

    public readonly typeOptions: InputOption[] = [
        { id: '', text: 'All request types' },
        { id: OutOfServiceType.Replaced.toString(), text: 'Assembly was replaced' },
        { id: OutOfServiceType.Removed.toString(), text: 'Assembly was removed' }
    ];

    constructor(
        private readonly _service: BackflowOutOfServiceRequestService,
        private readonly _router: Router,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.load();
    }

    public onStatusChange(value: string): void {
        this.status = Number(value) as OutOfServiceRequestStatusFilter;
    }

    public onTypeChange(value: string): void {
        this.typeFilter = value;
    }

    public async load(): Promise<void> {
        try {
            this.isLoading = true;

            const type = this.typeFilter === '' ? undefined : (Number(this.typeFilter) as OutOfServiceType);

            // The uncleared queue is small; show all matching requests (V1 does not page this page).
            const result = await this._service.getAllForWaterSupplier(
                { pageNumber: 1, pageSize: 250 },
                { sort: {}, filter: [] },
                this.status,
                type
            );

            this.requests = result.data.map(request => this.toViewModel(request));
        } finally {
            this.isLoading = false;
        }
    }

    public viewTest(test?: BackflowTest): void {
        if (test?.id == null) {
            return;
        }

        this._router.navigate(['/backflow/tests', test.id, 'view']);
    }

    public clear(request: OutOfServiceRequestVm): void {
        this._modalHelper.confirm({
            title: 'Clear Out of Service Request',
            messages: ['Are you sure you want to clear this out of service request?']
        }).result().subscribe(async () => {
            await this._service.clear(request.id);
            this._toastService.successfullySaved('Out of Service Request');
            await this.load();
        });
    }

    private toViewModel(request: BackflowOutOfServiceRequest): OutOfServiceRequestVm {
        const isReplaced = request.type === OutOfServiceType.Replaced;

        return {
            id: request.id!,
            submittedBy: this.buildSubmittedBy(request.test),
            cleared: !!request.clearedDate,
            description: request.description,
            submitted: this.buildAssemblyLine(request.test, 'Submitted Assembly'),
            replacement: isReplaced && request.replacementAssemblyTest
                ? this.buildAssemblyLine(request.replacementAssemblyTest, 'Replacement Assembly')
                : undefined
        };
    }

    private buildAssemblyLine(test: BackflowTest | undefined, label: string): AssemblyLineVm {
        return {
            label,
            test,
            submittedDate: test?.createdTime,
            testDate: test?.testDate,
            expirationDate: test?.expirationDate,
            serial: test?.serialNumber ?? '',
            isBypassDevice: this.isBypassDevice(test?.deviceType),
            bypassSerial: test?.serialNumber2,
            assemblyDescription: this.buildAssemblyDescription(test),
            hazard: this.buildHazard(test),
            location: test?.locationDescription ?? '',
            property: this.buildPropertyDescription(test),
            bpat: this.buildBpatDescription(test)
        };
    }

    private buildSubmittedBy(test?: BackflowTest): string {
        if (!test) {
            return '';
        }

        return [test.bpatCompanyName, test.bpatContactName, test.bpat?.emailAddress]
            .filter(part => part)
            .join(' - ');
    }

    private buildAssemblyDescription(test?: BackflowTest): string {
        if (!test) {
            return '';
        }

        const identity = [test.manufacturer, test.model, test.size]
            .filter(part => part)
            .join(' ');

        return test.deviceType ? `${identity} - ${test.deviceType}` : identity;
    }

    private isBypassDevice(deviceType?: string): boolean {
        return !!deviceType && ['DCD', 'DCD2', 'RPPD', 'RPPD2'].includes(deviceType);
    }

    private buildHazard(test?: BackflowTest): string {
        if (!test?.hazardType) {
            return '';
        }

        if (test.hazardType === 'Other' && test.hazardTypeOtherDescription) {
            return `Other - ${test.hazardTypeOtherDescription}`;
        }

        return test.hazardType;
    }

    private buildPropertyDescription(test?: BackflowTest): string {
        if (!test) {
            return '';
        }

        const line1 = [test.propertyStreetNumber, test.propertyStreetName]
            .filter(part => part)
            .join(' ');
        const line2 = [test.propertyCity, test.propertyState?.code, test.propertyZip]
            .filter(part => part)
            .join(' ');

        return [line1, line2].filter(part => part).join(', ');
    }

    private buildBpatDescription(test?: BackflowTest): string {
        if (!test) {
            return '';
        }

        const address = [test.bpatAddress, test.bpatCity, test.bpatState?.code, test.bpatZip]
            .filter(part => part)
            .join(' ');

        return [test.bpatCompanyName, test.bpatContactName, address]
            .filter(part => part)
            .join(', ');
    }
}
