import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { FogInspectionService } from '../../../shared/services/fog/fog-inspection.service';
import { FogInspection } from '../../../shared/models/fog/fog-inspection';

@Component({
    standalone: false,
    templateUrl: './fog-inspection-view.component.html'
})
export class FogInspectionViewComponent implements OnInit {
    private readonly _destroyRef = inject(DestroyRef);

    public isLoading = true;
    public inspection?: FogInspection;

    constructor(
        private readonly _route: ActivatedRoute,
        private readonly _inspectionService: FogInspectionService
    ) {}

    public ngOnInit(): void {
        this._route.paramMap
            .pipe(takeUntilDestroyed(this._destroyRef))
            .subscribe((params: ParamMap) => this.loadInspection(params.get('id')));
    }

    private async loadInspection(idParam: string | null): Promise<void> {
        if (!idParam) {
            this.isLoading = false;
            return;
        }

        try {
            this.isLoading = true;
            this.inspection = await this._inspectionService.getById(Number(idParam));
        } finally {
            this.isLoading = false;
        }
    }
}
