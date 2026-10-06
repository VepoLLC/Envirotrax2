import { Component } from "@angular/core";
import { formatDate } from "@angular/common";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { SiteSchedule } from "../../../shared/models/sites/site-schedule";
import { SiteService } from "../../../shared/services/sites/site.service";

export interface EditSiteScheduleModel {
    siteId: number;
    schedule: SiteSchedule | null;
}

@Component({
    standalone: false,
    templateUrl: './edit-site-schedule.component.html'
})
export class EditSiteScheduleComponent {
    public schedule: SiteSchedule;
    public isScheduled: boolean;
    public isLoading: boolean = false;

    private readonly _siteId: number;

    constructor(
        private readonly _siteService: SiteService,
        private readonly _modalReference: ModalReference<EditSiteScheduleModel, SiteSchedule | null>
    ) {
        const model = this._modalReference.config.model!;

        this._siteId = model.siteId;
        this.isScheduled = model.schedule != null;
        this.schedule = { ...model.schedule, scheduleDate: model.schedule?.scheduleDate ?? this.getCurrentHour() };
    }

    private getCurrentHour(): string {
        const now = new Date();
        now.setMinutes(0, 0, 0);

        return formatDate(now, 'yyyy-MM-ddTHH:mm', 'en-US');
    }

    public async save(form: NgForm): Promise<void> {
        if (form.valid) {
            try {
                this.isLoading = true;

                const result = await this._siteService.setScheduleForProfessional(this._siteId, this.schedule);
                this._modalReference.closeSuccess(result);
            } finally {
                this.isLoading = false;
            }
        }
    }

    public async clear(): Promise<void> {
        try {
            this.isLoading = true;

            await this._siteService.clearScheduleForProfessional(this._siteId);
            this._modalReference.closeSuccess(null);
        } finally {
            this.isLoading = false;
        }
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
