import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { ModalModule } from "@developer-partners/ngx-modal-dialog";
import { PaginationComponent } from "./data-components/pagination/pagination.component";
import { FilterInputComponent } from "./data-components/sorting-filtering/filter-input.component";
import { SortingFilteringModalComponent } from "./data-components/sorting-filtering/sorting-filtering-modal.component";
import { SortingFilteringComponent } from "./data-components/sorting-filtering/sorting-filtering.component";
import { TableComponent } from "./data-components/table/table.component";
import { TableCellComponent } from "./data-components/table/table-cells/table-cell.component";
import { CheckboxCellComponent } from "./data-components/table/table-cells/checkbox-cell.component";
import { CurrencyCellComponent } from "./data-components/table/table-cells/currency-cell.component";
import { DropdownOptionComponent } from "./dropdown/dropdown-option.component";
import { DropdownComponent } from "./dropdown/dropdown.component";
import { InputComponent } from "./input/input.component";
import { OptionDirective } from "./input/option.directive";
import { ConfirmModalComponent } from "./modals/confirm-modal.component";
import { MessageModalComponent } from "./modals/message-modal.component";
import { ValidationFieldComponent } from "./validation/validation-field/validation-field.component";
import { ValidationSummaryComponent } from "./validation/validation-summary/validation-summary.component";
import { SectionComponent } from "./section/section.component";
import { AppLoadingSpinnerModule } from "./loading-spinner/app-loading-spinner.module";
import { FilterPanelFieldComponent } from "./data-components/sorting-filtering/filter-panel/filter-panel-field.component";
import { FilterPanelComponent } from "./data-components/sorting-filtering/filter-panel/filter-panel.component";
import { NgSelectModule } from "@ng-select/ng-select";
import { InputOptionComponent } from "./input/input-option.component";
import { WaterSupplierHierarchyComponent } from "./water-supplier-hierarchy/water-supplier-hierarchy.component";
import { InfoIconComponent } from "./info-icon/info-icon.component";
import { FileUploadComponent } from "./file-upload/file-upload.component";

@NgModule({
    declarations: [
        PaginationComponent,
        FilterInputComponent,
        SortingFilteringModalComponent,
        SortingFilteringComponent,
        TableComponent,
        TableCellComponent,
        CheckboxCellComponent,
        CurrencyCellComponent,
        DropdownOptionComponent,
        DropdownComponent,
        InputComponent,
        OptionDirective,
        InputOptionComponent,
        ConfirmModalComponent,
        MessageModalComponent,
        ValidationFieldComponent,
        ValidationSummaryComponent,
        SectionComponent,
        FilterPanelComponent,
        FilterPanelFieldComponent,
        WaterSupplierHierarchyComponent,
        InfoIconComponent,
        FileUploadComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        ModalModule,
        AppLoadingSpinnerModule,
        NgSelectModule
    ],
    exports: [
        PaginationComponent,
        DropdownComponent,
        DropdownOptionComponent,
        InputComponent,
        InputOptionComponent,
        ValidationFieldComponent,
        ValidationSummaryComponent,
        TableComponent,
        CheckboxCellComponent,
        CurrencyCellComponent,
        SectionComponent,
        AppLoadingSpinnerModule,
        ModalModule,
        FilterPanelComponent,
        FilterPanelFieldComponent,
        WaterSupplierHierarchyComponent,
        InfoIconComponent,
        FileUploadComponent
    ]
})
export class SharedComponentsModule {

}