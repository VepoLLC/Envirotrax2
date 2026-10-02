import { CommonModule } from '@angular/common';
import { Component, Input, Optional, SkipSelf } from '@angular/core';
import { ControlContainer, FormsModule, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { FogInspection, InterceptorType } from '../../../../shared/models/fog/fog-inspection';
import { FogInspectionOptionsService } from '../../../../shared/services/fog/fog-inspection-options.service';

@Component({
    selector: 'vp-fog-inspection-trap',
    templateUrl: './fog-inspection-trap.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule],
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class FogInspectionTrapComponent {
    @Input() public inspection: FogInspection = {};
    @Input() public form?: NgForm;

    public readonly InterceptorType = InterceptorType;

    public readonly interceptorTypeOptions: InputOption[];
    public readonly capacityTypeOptions: InputOption[];

    constructor(private readonly _options: FogInspectionOptionsService) {
        this.interceptorTypeOptions = this._options.interceptorTypeOptions;
        this.capacityTypeOptions = this._options.capacityTypeOptions;
    }
}
