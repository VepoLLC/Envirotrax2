import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Site } from "../../../shared/models/sites/site";
import { SiteService } from "../../../shared/services/sites/site.service";
import { PropertyType } from "../../../shared/enums/property-type.enum";
import { SiteSchedule } from "../../../shared/models/sites/site-schedule";
import { ModalHelperService } from "@envirotrax/common-ui";
import { EditSiteScheduleComponent, EditSiteScheduleModel } from "../schedule/edit-site-schedule.component";
import { SITE_SCHEDULE_TYPE_NAMES, SiteScheduleService, SiteScheduleType } from "../../../shared/services/sites/site-schedule.service";

@Component({
    standalone: false,
    templateUrl: './site-details.component.html'
})
export class SiteDetailsComponent implements OnInit {
    public site: Site | null = null;
    public schedules: SiteSchedule[] = [];
    public scheduleTypes: SiteScheduleType[] = [];
    public readonly scheduleTypeNames = SITE_SCHEDULE_TYPE_NAMES;
    public isLoading: boolean = false;

    public readonly PropertyType = PropertyType;

    constructor(
        private readonly _siteService: SiteService,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _modalHelper: ModalHelperService,
        private readonly _siteScheduleService: SiteScheduleService
    ) {
    }

    public async ngOnInit(): Promise<void> {
        this._activatedRoute.paramMap.subscribe(async params => {
            const id = params.get('id');
            if (id) {
                await this.getSite(+id);
            }
        });
    }

    private async getSite(id: number): Promise<void> {
        try {
            this.isLoading = true;

            [this.site, this.schedules, this.scheduleTypes] = await Promise.all([
                this._siteService.getForProfessional(id),
                this._siteScheduleService.getMySchedules(id),
                this._siteScheduleService.getMyScheduleTypes()
            ]);
        } finally {
            this.isLoading = false;
        }
    }

    public editSchedule(): void {
        this._modalHelper.show<EditSiteScheduleModel, SiteSchedule[]>(EditSiteScheduleComponent, {
            title: 'Set Schedule',
            mode: 'disableFullScreen',
            model: {
                siteId: this.site!.id!,
                schedules: this.schedules,
                scheduleTypes: this.scheduleTypes
            }
        }).result().subscribe(schedules => this.schedules = schedules);
    }

    public goBack(): void {
        this._router.navigate(['../../'], { relativeTo: this._activatedRoute });
    }
}
