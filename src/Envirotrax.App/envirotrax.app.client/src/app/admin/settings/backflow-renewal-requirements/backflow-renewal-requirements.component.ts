import { Component, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { CellTemplateData, ColumnType, ModalHelperService, TableColumn } from "@envirotrax/common-ui";
import { TableViewModel } from "../../../shared/models/table-view-model";
import { BackflowRenewalRequirement } from "../../../shared/models/settings/backflow-renewal-requirement";
import { BackflowRenewalRequirementService } from "../../../shared/services/settings/backflow-renewal-requirement.service";
import { PropertyType } from "../../../shared/enums/property-type.enum";
import { ToastService } from "../../../shared/services/toast.service";
import { EditBackflowRenewalRequirementComponent } from "./edit/edit-backflow-renewal-requirement.component";

@Component({
    standalone: false,
    selector: 'app-backflow-renewal-requirements',
    templateUrl: './backflow-renewal-requirements.component.html'
})
export class BackflowRenewalRequirementsComponent implements OnInit {
    public table: TableViewModel<BackflowRenewalRequirement> = {
        columns: [],
        query: {
            sort: {},
            filter: []
        }
    };

    public readonly propertyType = PropertyType;

    @ViewChild('propertyTypeCell', { static: true })
    private propertyTypeCellTemplate!: TemplateRef<CellTemplateData<BackflowRenewalRequirement>>;

    @ViewChild('otherCell', { static: true })
    private otherCellTemplate!: TemplateRef<CellTemplateData<BackflowRenewalRequirement>>;

    constructor(
        private readonly _service: BackflowRenewalRequirementService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) { }

    public async ngOnInit(): Promise<void> {
        this.table.columns = this.getColumns();
        await this.getRequirements();
    }

    private getColumns(): TableColumn<BackflowRenewalRequirement>[] {
        return [
            {
                field: 'propertyType',
                caption: 'Property Type',
                cellTemplate: this.propertyTypeCellTemplate,
                type: ColumnType.text
            },
            {
                field: 'deviceType',
                caption: 'Assembly Type',
                type: ColumnType.text
            },
            {
                field: 'hazardType',
                caption: 'Hazard Type',
                type: ColumnType.text
            },
            {
                field: 'other',
                caption: 'Other',
                cellTemplate: this.otherCellTemplate,
                type: ColumnType.other,
                queryColumnExcluded: true
            },
            {
                field: 'renewalYears',
                caption: 'Years',
                type: ColumnType.text
            }
        ];
    }

    public async getRequirements(): Promise<void> {
        try {
            this.table.isLoading = true;
            this.table.items = await this._service.getAll(
                this.table.items?.pageInfo || {},
                this.table.query
            );
        } finally {
            this.table.isLoading = false;
        }
    }

    public add(): void {
        const newRequirement: BackflowRenewalRequirement = {
            propertyType: PropertyType.Residential,
            deviceType: 'All',
            hazardType: 'All',
            renewalYears: 1,
            auxWaterSupply: false,
            hasSiteOssf: false
        };

        this._modalHelper.show<BackflowRenewalRequirement>(EditBackflowRenewalRequirementComponent, {
            title: 'Add Renewal Requirement',
            model: newRequirement,
            size: ModalSize.large
        }).result().subscribe(() => this.getRequirements());
    }

    public edit(requirement: BackflowRenewalRequirement): void {
        this._modalHelper.show<BackflowRenewalRequirement>(EditBackflowRenewalRequirementComponent, {
            title: 'Edit Renewal Requirement',
            model: requirement,
            size: ModalSize.large
        }).result().subscribe(() => this.getRequirements());
    }

    public delete(requirement: BackflowRenewalRequirement): void {
        this._modalHelper.showDeleteConfirmation()
            .result()
            .subscribe(async () => {
                try {
                    this.table.isLoading = true;
                    await this._service.delete(requirement.id!);
                    this._toastService.successFullyDeleted('Renewal Requirement');
                    await this.getRequirements();
                } finally {
                    this.table.isLoading = false;
                }
            });
    }
}
