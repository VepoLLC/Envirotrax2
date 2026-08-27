import { Component, Input, OnChanges, OnInit, SimpleChanges } from "@angular/core";
import { MySupplierHierarchyDto, WaterSupplier, WaterSupplierHierarchy } from "../../models/water-suppliers/water-supplier";
import { AuthService } from "../../services/auth/auth.service";
import { AppContainerHelperService } from "../../services/helpers/app-contaner-helper.service";

const COLUMN_COUNT = 4;

@Component({
    selector: 'vp-water-supplier-hierarchy',
    standalone: false,
    templateUrl: './water-supplier-hierarchy.component.html'
})
export class WaterSupplierHierarchyComponent implements OnInit, OnChanges {
    public isLoading: boolean = false;

    public columns: WaterSupplierHierarchy[][] = [];

    @Input()
    public mySuppliers?: MySupplierHierarchyDto;

    @Input()
    public returnUrl?: string;

    constructor(
        private readonly _authService: AuthService,
        private readonly _containerHelper: AppContainerHelperService
    ) {

    }

    public ngOnInit(): void {
        this._containerHelper.setContainerVisibility(false);
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if (changes['mySuppliers']) {
            this.columns = this.buildColumns(this.mySuppliers?.hierarchy ?? []);
        }
    }

    // Distributes the alphabetical groups across a fixed number of columns, balancing
    // by item count instead of giving every letter its own column (some letters have
    // only one or two suppliers, which left short columns of wasted space).
    private buildColumns(hierarchy: WaterSupplierHierarchy[]): WaterSupplierHierarchy[][] {
        const columns: WaterSupplierHierarchy[][] = Array.from({ length: COLUMN_COUNT }, () => []);

        const totalItems = hierarchy.reduce((sum, group) => sum + group.waterSuppliers.length, 0);
        const targetPerColumn = Math.ceil(totalItems / COLUMN_COUNT) || 1;

        let columnIndex = 0;
        let columnItemCount = 0;

        for (const group of hierarchy) {
            const isLastColumn = columnIndex === COLUMN_COUNT - 1;

            if (!isLastColumn && columnItemCount > 0 && columnItemCount + group.waterSuppliers.length > targetPerColumn) {
                columnIndex++;
                columnItemCount = 0;
            }

            columns[columnIndex].push(group);
            columnItemCount += group.waterSuppliers.length;
        }

        return columns;
    }

    public async selectSupplier(supplier: WaterSupplier): Promise<void> {
        try {
            this.isLoading = true;
            await this._authService.signIn(supplier.id, undefined, this.returnUrl);
        } finally {
            this.isLoading = false;
        }
    }
}