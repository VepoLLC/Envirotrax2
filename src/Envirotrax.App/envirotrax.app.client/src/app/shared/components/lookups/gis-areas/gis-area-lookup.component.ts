import { Component, forwardRef, Input, OnInit, Optional, SkipSelf } from "@angular/core";
import { ControlContainer, NgForm } from "@angular/forms";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { FilterPanelComponent } from "../../data-components/sorting-filtering/filter-panel/filter-panel.component";
import { FilterPanelFieldComponent } from "../../data-components/sorting-filtering/filter-panel/filter-panel-field.component";
import { InputOption } from "../../input/input.component";
import { GisAreaService } from "../../../services/gis-areas/gis-area.service";
import { ModalHelperService } from "../../../services/helpers/modal-helper.service";
import { GisAreaSelectionModalComponent, GisAreaSelectionModel } from "./gis-area-selection-modal.component";

@Component({
    selector: 'app-gis-area-lookup',
    standalone: false,
    templateUrl: './gis-area-lookup.component.html',
    providers: [
        { provide: FilterPanelFieldComponent, useExisting: forwardRef(() => GisAreaLookupComponent) }
    ],
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class GisAreaLookupComponent implements OnInit {
    @Input() public fieldName: string = 'gisAreaId';
    @Input() public label: string = 'GIS Area';
    @Input() public form?: NgForm;

    public readonly type: 'select' = 'select';
    public value: string = '';
    public options: InputOption[] = [];

    constructor(
        private readonly _gisAreaService: GisAreaService,
        private readonly _modalHelper: ModalHelperService,
        @Optional() private readonly _parent: FilterPanelComponent
    ) { }

    public async ngOnInit(): Promise<void> {
        this.options = await this._gisAreaService.getAllAsOptions();
    }

    public onChange(): void {
        if (this._parent) {
            this._parent.onFilterChanged();
        }
    }

    public selectArea(): void {
        this._modalHelper.show<GisAreaSelectionModel, string>(
            GisAreaSelectionModalComponent,
            { title: 'GIS Area Selection', size: ModalSize.extraLarge, model: { currentAreaId: this.value } }
        ).result().subscribe(selectedId => {
            this.value = selectedId;
            this.onChange();
        });
    }
}
