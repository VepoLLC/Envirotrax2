import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { PublicSearchService } from "../../shared/services/public-search/public-search.service";
import { DownloadService } from "../../shared/services/download.service";
import { CsiInspection } from "../../shared/models/csi/csi-inspection";
import { CsiInspectionAssembly } from "../../shared/models/csi/csi-inspection-assembly";
import { CsiInspectionImage } from "../../shared/models/csi/csi-inspection-image";
import { CsiInspectionReason, csiInspectionReasonLabels } from "../../shared/enums/csi-inspection-reason.enum";

type PublicCsiInspectionTab = 'main' | 'assemblies' | 'additional' | 'images';

@Component({
    standalone: false,
    templateUrl: './public-csi-inspection-view.component.html',
    styleUrl: './public-csi-inspection-view.component.scss'
})
export class PublicCsiInspectionViewComponent implements OnInit {
    public inspection: CsiInspection | null = null;
    public assemblies: CsiInspectionAssembly[] = [];
    public images: CsiInspectionImage[] = [];

    public reasonLabel: string = '';
    public propertyTypeLabel: string = '';
    public activeTab: PublicCsiInspectionTab = 'main';

    public isLoading: boolean = false;
    public isLoaded: boolean = false;
    public isAssembliesLoading: boolean = false;
    public isImagesLoading: boolean = false;
    public isImagesLoaded: boolean = false;

    private _id: number = 0;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _publicSearchService: PublicSearchService,
        private readonly _downloadService: DownloadService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        this._id = Number(this._activatedRoute.snapshot.paramMap.get('id'));

        try {
            this.isLoading = true;

            this.inspection = this._id > 0
                ? await this._publicSearchService.getCsiInspection(this._id)
                : null;

            if (this.inspection) {
                this.reasonLabel = csiInspectionReasonLabels[this.inspection.reasonForInspection as CsiInspectionReason] ?? '';
                this.propertyTypeLabel = this.inspection.propertyType === 0 ? 'Residential' : 'Commercial';
            }
        } finally {
            this.isLoading = false;
            this.isLoaded = true;
        }

        if (this.inspection) {
            await this.loadAssemblies();
        }
    }

    public async setActiveTab(tab: PublicCsiInspectionTab): Promise<void> {
        this.activeTab = tab;

        if (tab === 'images' && !this.isImagesLoaded) {
            await this.loadImages();
        }
    }

    public async exportPdf(): Promise<void> {
        if (!this.inspection) {
            return;
        }

        try {
            this.isLoading = true;

            const blob = await this._publicSearchService.getCsiInspectionPdf(this._id);

            this._downloadService.downloadFileFromBlob(blob, `csi-inspection-${this._id}.pdf`);
        } finally {
            this.isLoading = false;
        }
    }

    private async loadAssemblies(): Promise<void> {
        try {
            this.isAssembliesLoading = true;
            this.assemblies = await this._publicSearchService.getCsiInspectionAssemblies(this._id);
        } finally {
            this.isAssembliesLoading = false;
        }
    }

    private async loadImages(): Promise<void> {
        try {
            this.isImagesLoading = true;
            this.images = await this._publicSearchService.getCsiInspectionImages(this._id);
            this.isImagesLoaded = true;
        } finally {
            this.isImagesLoading = false;
        }
    }
}
