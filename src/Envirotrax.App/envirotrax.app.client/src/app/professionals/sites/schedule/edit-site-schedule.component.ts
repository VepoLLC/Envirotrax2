import { Component } from "@angular/core";
import { formatDate } from "@angular/common";
import { NgForm } from "@angular/forms";
import { ModalReference } from "@developer-partners/ngx-modal-dialog";
import { InputOption } from "@envirotrax/common-ui";
import { SiteSchedule } from "../../../shared/models/sites/site-schedule";
import { ProfessionalType } from "../../../shared/models/professionals/licenses/professional-user-license";
import { SiteScheduleService, SiteScheduleType } from "../../../shared/services/sites/site-schedule.service";

export interface EditSiteScheduleModel {
    siteId: number;
    schedules: SiteSchedule[];
    scheduleTypes: SiteScheduleType[];
}

@Component({
    standalone: false,
    templateUrl: './edit-site-schedule.component.html'
})
export class EditSiteScheduleComponent {
    public schedule: SiteSchedule = {};
    public isScheduled: boolean = false;
    public isLoading: boolean = false;

    public scheduleTypeOptions: InputOption[];
    public hasMultipleScheduleTypes: boolean;
    public selectedScheduleType: string = '';

    private readonly _siteId: number;
    private readonly _schedules: SiteSchedule[];

    constructor(
        private readonly _siteScheduleService: SiteScheduleService,
        private readonly _modalReference: ModalReference<EditSiteScheduleModel, SiteSchedule[]>
    ) {
        const model = this._modalReference.config.model!;

        this._siteId = model.siteId;
        this._schedules = model.schedules;

        this.scheduleTypeOptions = model.scheduleTypes.map(t => ({ id: String(t.professionalType), text: t.name }));
        this.hasMultipleScheduleTypes = model.scheduleTypes.length > 1;

        const scheduledType = model.scheduleTypes.find(t => model.schedules.some(s => s.professionalType === t.professionalType));

        this.selectScheduleType((scheduledType ?? model.scheduleTypes[0]).professionalType);
    }

    public scheduleTypeChanged(professionalType: string): void {
        this.selectScheduleType(Number(professionalType));
    }

    private selectScheduleType(professionalType: ProfessionalType): void {
        const existing = this._schedules.find(s => s.professionalType === professionalType);

        this.selectedScheduleType = String(professionalType);
        this.isScheduled = existing != null;
        this.schedule = {
            professionalType,
            scheduleDate: existing?.scheduleDate ?? this.getCurrentHour()
        };
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

                const saved = await this._siteScheduleService.set(this._siteId, this.schedule);
                const schedules = [...this.withoutSelectedType(), saved].sort((a, b) => a.scheduleDate!.localeCompare(b.scheduleDate!));

                this._modalReference.closeSuccess(schedules);
            } finally {
                this.isLoading = false;
            }
        }
    }

    public async clear(): Promise<void> {
        try {
            this.isLoading = true;

            await this._siteScheduleService.clear(this._siteId, this.schedule.professionalType!);
            this._modalReference.closeSuccess(this.withoutSelectedType());
        } finally {
            this.isLoading = false;
        }
    }

    private withoutSelectedType(): SiteSchedule[] {
        return this._schedules.filter(s => s.professionalType !== this.schedule.professionalType);
    }

    public cancel(): void {
        this._modalReference.cancel();
    }
}
