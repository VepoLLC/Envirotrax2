import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { ModalModule } from "@developer-partners/ngx-modal-dialog";
import { NgSelectModule } from "@ng-select/ng-select";
import { PaginationComponent } from "./components/data-components/pagination/pagination.component";
import { FilterInputComponent } from "./components/data-components/sorting-filtering/filter-input.component";
import { SortingFilteringModalComponent } from "./components/data-components/sorting-filtering/sorting-filtering-modal.component";
import { SortingFilteringComponent } from "./components/data-components/sorting-filtering/sorting-filtering.component";
import { FilterPanelComponent } from "./components/data-components/sorting-filtering/filter-panel/filter-panel.component";
import { FilterPanelFieldComponent } from "./components/data-components/sorting-filtering/filter-panel/filter-panel-field.component";
import { TableComponent } from "./components/data-components/table/table.component";
import { TableCellComponent } from "./components/data-components/table/table-cells/table-cell.component";
import { CheckboxCellComponent } from "./components/data-components/table/table-cells/checkbox-cell.component";
import { CurrencyCellComponent } from "./components/data-components/table/table-cells/currency-cell.component";
import { DropdownComponent } from "./components/dropdown/dropdown.component";
import { DropdownOptionComponent } from "./components/dropdown/dropdown-option.component";
import { InputComponent } from "./components/input/input.component";
import { InputOptionComponent } from "./components/input/input-option.component";
import { ConfirmModalComponent } from "./components/modals/confirm-modal.component";
import { MessageModalComponent } from "./components/modals/message-modal.component";
import { ValidationFieldComponent } from "./components/validation/validation-field/validation-field.component";
import { ValidationSummaryComponent } from "./components/validation/validation-summary/validation-summary.component";
import { SectionComponent } from "./components/section/section.component";
import { SectionActionsComponent } from "./components/section/section-actions.component";
import { InfoIconComponent } from "./components/info-icon/info-icon.component";
import { StatusIconComponent } from "./components/status-icon/status-icon.component";
import { FileUploadComponent } from "./components/file-upload/file-upload.component";
import { LookupFieldComponent } from "./components/lookup-field/lookup-field.component";
import { MapComponent } from "./components/map/map.component";
import { MapResultsComponent } from "./components/map/map-results.component";
import { AppLoadingSpinnerModule } from "./components/loading-spinner/app-loading-spinner.module";
import { InputAddOnComponent } from "./components/input/input-add-on.component";

@NgModule({
    declarations: [
        PaginationComponent,
        FilterInputComponent,
        SortingFilteringModalComponent,
        SortingFilteringComponent,
        FilterPanelComponent,
        FilterPanelFieldComponent,
        TableComponent,
        TableCellComponent,
        CheckboxCellComponent,
        CurrencyCellComponent,
        DropdownComponent,
        DropdownOptionComponent,
        InputComponent,
        InputOptionComponent,
        InputAddOnComponent,
        ConfirmModalComponent,
        MessageModalComponent,
        ValidationFieldComponent,
        ValidationSummaryComponent,
        SectionComponent,
        SectionActionsComponent,
        InfoIconComponent,
        StatusIconComponent,
        FileUploadComponent,
        LookupFieldComponent,
        MapComponent,
        MapResultsComponent,
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        ModalModule,
        NgSelectModule,
        AppLoadingSpinnerModule
    ],
    exports: [
        PaginationComponent,
        FilterPanelComponent,
        FilterPanelFieldComponent,
        TableComponent,
        CheckboxCellComponent,
        CurrencyCellComponent,
        DropdownComponent,
        DropdownOptionComponent,
        InputComponent,
        InputOptionComponent,
        InputAddOnComponent,
        ValidationFieldComponent,
        ValidationSummaryComponent,
        SectionComponent,
        SectionActionsComponent,
        InfoIconComponent,
        StatusIconComponent,
        FileUploadComponent,
        LookupFieldComponent,
        MapComponent,
        MapResultsComponent,
        ModalModule,
        AppLoadingSpinnerModule
    ]
})
export class EnvirotraxComponentsModule {
}