import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { PublicSearchService } from "../../shared/services/public-search/public-search.service";
import { PublicBackflowTestDetails } from "../../shared/models/public-search/public-backflow-test-details";

@Component({
    standalone: false,
    templateUrl: './public-backflow-test-view.component.html'
})
export class PublicBackflowTestViewComponent implements OnInit {
    public test: PublicBackflowTestDetails | null = null;
    public isLoading: boolean = false;
    public isLoaded: boolean = false;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _publicSearchService: PublicSearchService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        const id = Number(this._activatedRoute.snapshot.paramMap.get('id'));

        try {
            this.isLoading = true;

            this.test = id > 0
                ? await this._publicSearchService.getBackflowTest(id)
                : null;
        } finally {
            this.isLoading = false;
            this.isLoaded = true;
        }
    }

    public exportPdf(): void {
        if (!this.test) {
            return;
        }

        window.open(this._publicSearchService.getBackflowTestPdfUrl(this.test.id), '_blank');
    }
}
