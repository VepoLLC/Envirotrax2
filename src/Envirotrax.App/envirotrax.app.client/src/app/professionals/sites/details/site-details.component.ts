import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Site } from "../../../shared/models/sites/site";
import { SiteService } from "../../../shared/services/sites/site.service";
import { PropertyType } from "../../../shared/enums/property-type.enum";

@Component({
    standalone: false,
    templateUrl: './site-details.component.html'
})
export class SiteDetailsComponent implements OnInit {
    public site: Site | null = null;
    public isLoading: boolean = false;

    public readonly PropertyType = PropertyType;

    constructor(
        private readonly _siteService: SiteService,
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _router: Router
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
            this.site = await this._siteService.getForProfessional(id);
        } finally {
            this.isLoading = false;
        }
    }

    public goBack(): void {
        this._router.navigate(['../../'], { relativeTo: this._activatedRoute });
    }
}
