import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { LoadingSpinnerModule } from "@developer-partners/ngx-loading-spinner";
import { ModalModule } from "@developer-partners/ngx-modal-dialog";
import { PaginationComponent } from "./data-components/pagination/pagination.component";
import { FilterInputComponent } from "./data-components/sorting-filtering/filter-input.component";
import { SortingFilteringModalComponent } from "./data-components/sorting-filtering/sorting-filtering-modal.component";
import { SortingFilteringComponent } from "./data-components/sorting-filtering/sorting-filtering.component";
import { TableComponent } from "./data-components/table/table.component";
import { TableCellComponent } from "./data-components/table/table-cells/table-cell.component";
import { CheckboxCellComponent } from "./data-components/table/table-cells/checkbox-cell.component";
import { DropdownOptionComponent } from "./dropdown/dropdown-option.component";
import { DropdownComponent } from "./dropdown/dropdown.component";
import { InputComponent } from "./input/input.component";
import { OptionDirective } from "./input/option.directive";
import { ConfirmModalComponent } from "./modals/confirm-modal.component";
import { MessageModalComponent } from "./modals/message-modal.component";
import { ValidationFieldComponent } from "./validation/validation-field/validation-field.component";
import { ValidationSummaryComponent } from "./validation/validation-summary/validation-summary.component";
import { SectionComponent } from "./section/section.component";

@NgModule({
    declarations: [
        PaginationComponent,
        FilterInputComponent,
        SortingFilteringModalComponent,
        SortingFilteringComponent,
        TableComponent,
        TableCellComponent,
        CheckboxCellComponent,
        DropdownOptionComponent,
        DropdownComponent,
        InputComponent,
        OptionDirective,
        ConfirmModalComponent,
        MessageModalComponent,
        ValidationFieldComponent,
        ValidationSummaryComponent,
        SectionComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule,
        ModalModule,
        LoadingSpinnerModule
    ],
    exports: [
        PaginationComponent,
        DropdownComponent,
        DropdownOptionComponent,
        InputComponent,
        ValidationFieldComponent,
        ValidationSummaryComponent,
        TableComponent,
        CheckboxCellComponent,
        SectionComponent,
        LoadingSpinnerModule,
        ModalModule
    ]
})
export class SharedComponentsModule {

}