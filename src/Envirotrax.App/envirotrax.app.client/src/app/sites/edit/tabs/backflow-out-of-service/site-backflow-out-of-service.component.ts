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

@Component({
    selector: 'app-site-backflow-out-of-service',
    standalone: false,
    templateUrl: './site-backflow-out-of-service.component.html'
})
export class SiteBackflowOutOfServiceComponent implements OnInit {
    @Input()
    public siteId?: number;

    @ViewChild('statusTemplate', { static: true })
    public statusTemplate!: TemplateRef<CellTemplateData<BackflowOutOfServiceRequest>>;

    @ViewChild('bpatTemplate', { static: true })
    public bpatTemplate!: TemplateRef<CellTemplateData<BackflowOutOfServiceRequest>>;

    @ViewChild('serialTemplate', { static: true })
    public serialTemplate!: TemplateRef<CellTemplateData<BackflowOutOfServiceRequest>>;

    @ViewChild('assemblyTemplate', { static: true })
    public assemblyTemplate!: TemplateRef<CellTemplateData<BackflowOutOfServiceRequest>>;

    @ViewChild('deviceDescriptionTemplate', { static: true })
    public deviceDescriptionTemplate!: TemplateRef<CellTemplateData<BackflowOutOfServiceRequest>>;

    public readonly BackflowTestResult = BackflowTestResult;

    public canViewTesters: boolean = false;

    public table: TableViewModel<BackflowOutOfServiceRequest> = {
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

    public viewTest(request: BackflowOutOfServiceRequest): void {
        if (!request.testId) {
            return;
        }
        const url = this._router.serializeUrl(
            this._router.createUrlTree(['/backflow', 'tests', request.testId, 'view'])
        );
        window.open(url, '_blank');
    }

    public isBypassDevice(deviceType?: string): boolean {
        return !!deviceType && BYPASS_DEVICE_TYPES.includes(deviceType);
    }

    public hazardLabel(test?: BackflowTest): string {
        if (!test?.hazardType) {
            return 'Unknown';
        }
        if (test.hazardType === 'Other' && test.hazardTypeOtherDescription) {
            return `Other - ${test.hazardTypeOtherDescription}`;
        }
        return test.hazardType;
    }

    public deviceDescription(manufacturer?: string, model?: string, size?: string, deviceType?: string): string {
        const identity = [manufacturer, model, size].filter(part => part).join(' ');
        return deviceType ? `${identity} - ${deviceType}` : identity;
    }

    public async getRequests(): Promise<void> {
        if (!this.siteId) {
            return;
        }
        try {
            this.table.isLoading = true;
            this.table.query.filter = [this.siteFilter()];
            this.table.items = await this._backflowOutOfServiceRequestService.getAllForWaterSupplier(
                this.table.items?.pageInfo || {},
                this.table.query,
                OutOfServiceRequestStatusFilter.All
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    private siteFilter(): QueryProperty {
        return {
            columnName: 'test.site.id',
            value: this.siteId!.toString(),
            comparisonOperator: 'Eq' as ComparisonOperator
        };
    }

    private getColumns(): TableColumn<BackflowOutOfServiceRequest>[] {
        return [
            {
                field: '',
                caption: 'Status',
                type: ColumnType.other,
                queryColumnExcluded: true,
                cellTemplate: this.statusTemplate
            },
            {
                field: 'test.createdTime',
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
