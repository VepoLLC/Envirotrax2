import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Site } from "../../../shared/models/sites/site";
import { SiteService } from "../../../shared/services/sites/site.service";
import { PropertyType } from "../../../shared/enums/property-type.enum";
import { SiteSchedule } from "../../../shared/models/sites/site-schedule";
import { ModalHelperService } from "@envirotrax/common-ui";
import { EditSiteScheduleComponent, EditSiteScheduleModel } from "../schedule/edit-site-schedule.component";

@Component({
    standalone: false,
    templateUrl: './site-details.component.html'
})
export class SiteDetailsComponent implements OnInit {
    public site: Site | null = null;
    public schedule: SiteSchedule | null = null;
    public isLoading: boolean = false;

    public readonly PropertyType = PropertyType;

    constructor(
        private readonly _siteService: SiteService,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router,
        private readonly _modalHelper: ModalHelperService
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

            [this.site, this.schedule] = await Promise.all([
                this._siteService.getForProfessional(id),
                this._siteService.getScheduleForProfessional(id)
            ]);
        } finally {
            this.isLoading = false;
        }
    }

    public editSchedule(): void {
        this._modalHelper.show<EditSiteScheduleModel, SiteSchedule | null>(EditSiteScheduleComponent, {
            title: 'Set Schedule',
            model: {
                siteId: this.site!.id!,
                schedule: this.schedule
            }
        }).result().subscribe(schedule => this.schedule = schedule);
    }

    public goBack(): void {
        this._router.navigate(['../../'], { relativeTo: this._activatedRoute });
    }
}
