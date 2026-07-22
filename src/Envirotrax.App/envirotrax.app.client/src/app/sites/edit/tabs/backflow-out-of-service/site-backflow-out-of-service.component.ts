import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { BackflowOutOfServiceRequest } from '../../../../shared/models/backflow/backflow-out-of-service-request';
import { OutOfServiceRequestStatusFilter } from '../../../../shared/models/backflow/out-of-service-request-status-filter.enum';
import { BackflowTest } from '../../../../shared/models/backflow/backflow-test';
import { BackflowTestResult } from '../../../../shared/models/backflow/backflow-test-enums';
import { BackflowOutOfServiceRequestService } from '../../../../shared/services/backflow/backflow-out-of-service-request.service';
import { AuthService } from '../../../../shared/services/auth/auth.service';
import { PermissionAction, PermissionType } from '../../../../shared/models/permission-type';
import { TableViewModel } from '../../../../shared/models/table-view-model';
import { ComparisonOperator, QueryProperty } from '../../../../shared/models/query';
import { CellTemplateData, ColumnType, TableColumn } from '@envirotrax/common-ui';

const BYPASS_DEVICE_TYPES = ['DCD', 'DCD2', 'RPPD', 'RPPD2'];

interface BackflowOutOfServiceRowVm {
    id: number;
    testId?: number;
    date?: string;
    isCurrent: boolean;
    passed: boolean;
    outOfService: boolean;
    showBpatLink: boolean;
    professionalId?: number;
    bpatCompanyName?: string;
    bpatContactName?: string;
    bpatAddress?: string;
    bpatCityStateZip?: string;
    isBypassDevice: boolean;
    serialNumber?: string;
    bypassSerialNumber?: string;
    assemblyDescription: string;
    bypassAssemblyDescription?: string;
    hazardLabel: string;
    locationDescription?: string;
}

@Component({
    selector: 'app-site-backflow-out-of-service',
    standalone: false,
    templateUrl: './site-backflow-out-of-service.component.html'
})
export class SiteBackflowOutOfServiceComponent implements OnInit {
    @Input()
    public siteId?: number;

    @ViewChild('statusTemplate', { static: true })
    public statusTemplate!: TemplateRef<CellTemplateData<BackflowOutOfServiceRowVm>>;

    @ViewChild('bpatTemplate', { static: true })
    public bpatTemplate!: TemplateRef<CellTemplateData<BackflowOutOfServiceRowVm>>;

    @ViewChild('serialTemplate', { static: true })
    public serialTemplate!: TemplateRef<CellTemplateData<BackflowOutOfServiceRowVm>>;

    @ViewChild('assemblyTemplate', { static: true })
    public assemblyTemplate!: TemplateRef<CellTemplateData<BackflowOutOfServiceRowVm>>;

    @ViewChild('deviceDescriptionTemplate', { static: true })
    public deviceDescriptionTemplate!: TemplateRef<CellTemplateData<BackflowOutOfServiceRowVm>>;

    public canViewTesters: boolean = false;

    public table: TableViewModel<BackflowOutOfServiceRowVm> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        }
    };

    constructor(
        private readonly _backflowOutOfServiceRequestService: BackflowOutOfServiceRequestService,
        private readonly _router: Router,
        private readonly _authService: AuthService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.canViewTesters = await this._authService.hasAnyPermisison(
            PermissionAction.CanView, PermissionType.BackflowTesters);
        this.table.columns = this.getColumns();
        await this.getRequests();
    }

    public viewTest(row: BackflowOutOfServiceRowVm): void {
        if (!row.testId) {
            return;
        }
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/backflow', 'tests', row.testId, 'view'])
        );
        window.open(url, '_blank');
    }

    public async getRequests(): Promise<void> {
        if (!this.siteId) {
            return;
        }
        try {
            this.table.isLoading = true;
            this.table.query.filter = [this.siteFilter()];
            const result = await this._backflowOutOfServiceRequestService.getAllForWaterSupplier(
                this.table.items?.pageInfo || {},
                this.table.query,
                OutOfServiceRequestStatusFilter.All
            );
            this.table.items = {
                pageInfo: result.pageInfo,
                data: result.data.map(request => this.toRowVm(request))
            };
        } finally {
            this.table.isLoading = false;
        }
    }

    private toRowVm(request: BackflowOutOfServiceRequest): BackflowOutOfServiceRowVm {
        const test = request.test;
        const isBypass = this.isBypassDeviceType(test?.deviceType);

        return {
            id: request.id!,
            testId: request.testId,
            date: test?.createdTime,
            isCurrent: !!test?.isCurrent,
            passed: test?.testResult !== BackflowTestResult.Fail,
            outOfService: !!test?.outOfService,
            showBpatLink: this.canViewTesters && !!test?.professional?.id,
            professionalId: test?.professional?.id,
            bpatCompanyName: test?.bpatCompanyName,
            bpatContactName: test?.bpatContactName,
            bpatAddress: test?.bpatAddress,
            bpatCityStateZip: this.buildCityStateZip(test?.bpatCity, test?.bpatState?.name, test?.bpatZip),
            isBypassDevice: isBypass,
            serialNumber: test?.serialNumber,
            bypassSerialNumber: isBypass ? test?.serialNumber2 : undefined,
            assemblyDescription: this.buildDeviceDescription(test?.manufacturer, test?.model, test?.size, test?.deviceType),
            bypassAssemblyDescription: isBypass
                ? this.buildDeviceDescription(test?.manufacturer2, test?.model2, test?.size2)
                : undefined,
            hazardLabel: this.buildHazardLabel(test),
            locationDescription: test?.locationDescription
        };
    }

    private isBypassDeviceType(deviceType?: string): boolean {
        return !!deviceType && BYPASS_DEVICE_TYPES.includes(deviceType);
    }

    private buildHazardLabel(test?: BackflowTest): string {
        if (!test?.hazardType) {
            return 'Unknown';
        }
        if (test.hazardType === 'Other' && test.hazardTypeOtherDescription) {
            return `Other - ${test.hazardTypeOtherDescription}`;
        }
        return test.hazardType;
    }

    private buildDeviceDescription(manufacturer?: string, model?: string, size?: string, deviceType?: string): string {
        const identity = [manufacturer, model, size].filter(part => part).join(' ');
        return deviceType ? `${identity} - ${deviceType}` : identity;
    }

    private buildCityStateZip(city?: string, stateName?: string, zip?: string): string {
        const cityPart = city ?? '';
        const stateZipPart = [stateName, zip].filter(part => part).join(' ');

        if (cityPart && stateZipPart) {
            return `${cityPart}, ${stateZipPart}`;
        }

        return cityPart || stateZipPart;
    }

    private siteFilter(): QueryProperty {
        return {
            columnName: 'test.site.id',
            value: this.siteId!.toString(),
            comparisonOperator: 'Eq' as ComparisonOperator
        };
    }

    private getColumns(): TableColumn<BackflowOutOfServiceRowVm>[] {
        return [
            {
                field: '',
                caption: 'Status',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.statusTemplate
            },
            {
                field: 'date',
                caption: 'Date',
                type: ColumnType.date
            },
            {
                field: '',
                caption: 'Submitted By:',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.bpatTemplate
            },
            {
                field: '',
                caption: 'Serial #',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.serialTemplate
            },
            {
                field: '',
                caption: 'Assembly Description',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.assemblyTemplate
            },
            {
                field: '',
                caption: 'Out of Service Information',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.deviceDescriptionTemplate
            }
        ];
    }
}
