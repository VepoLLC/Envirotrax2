import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { PublicSearchService } from "../../shared/services/public-search/public-search.service";
import { DownloadService } from "../../shared/services/download.service";
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
        private readonly _publicSearchService: PublicSearchService,
        private readonly _downloadService: DownloadService
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

    public async exportPdf(): Promise<void> {
        if (!this.test) {
            return;
        }

        try {
            this.isLoading = true;

            const blob = await this._publicSearchService.getBackflowTestPdf(this.test.id);

            this._downloadService.downloadFileFromBlob(blob, `backflow-test-${this.test.id}.pdf`);
        } finally {
            this.isLoading = false;
        }
    }
}
