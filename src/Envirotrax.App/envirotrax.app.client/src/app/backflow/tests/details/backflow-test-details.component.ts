import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { NgForm } from "@angular/forms";
import { BackflowTest } from "../../../shared/models/backflow/backflow-test";
import { BackflowTestService } from "../../../shared/services/backflow/backflow-test.service";
import { State } from "../../../shared/models/lookup/state";
import { LookupService } from "../../../shared/services/lookup/lookup.service";
import { AuthService } from "../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../shared/models/permission-type";
import { FeatureType } from "../../../shared/models/feature-type";
import { ToastService } from "../../../shared/services/toast.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { ImageUrlChange } from "./images/backflow-test-images.component";
import { InputOption, ModalHelperService } from "@envirotrax/common-ui";
import { DownloadService } from "../../../shared/services/download.service";
import { BackflowTestingSettingsService } from "../../../shared/services/settings/backflow-testing-settings.service";
import { BackflowSettings } from "../../../shared/models/settings/backflow-settings";
import { ModalSize } from "@developer-partners/ngx-modal-dialog";
import { BackflowTestRejectComponent } from "./reject/backflow-test-reject.component";
import { BackflowTestForceRenewalComponent } from "./force-renewal/backflow-test-force-renewal.component";

@Component({
    selector: 'app-backflow-test-details',
    standalone: false,
    templateUrl: './backflow-test-details.component.html',
})
export class BackflowTestDetailsComponent implements OnInit {
    public id: number = 0;
    public test: BackflowTest | null = null;
    public isLoading: boolean = false;
    public savingSection: string | null = null;
    public canModify: boolean = false;
    public canViewTesters: boolean = false;
    public canForceRenewal: boolean = false;
    public states: InputOption<State>[] = [];
    public validationErrors: string[] = [];
    public settings: BackflowSettings | null = null;

    public readonly scheduleMonthOptions = [
        { id: 0, text: 'Not Applicable' },
        { id: 1, text: '(1) January' },
        { id: 2, text: '(2) February' },
        { id: 3, text: '(3) March' },
        { id: 4, text: '(4) April' },
        { id: 5, text: '(5) May' },
        { id: 6, text: '(6) June' },
        { id: 7, text: '(7) July' },
        { id: 8, text: '(8) August' },
        { id: 9, text: '(9) September' },
        { id: 10, text: '(10) October' },
        { id: 11, text: '(11) November' },
        { id: 12, text: '(12) December' }
    ];

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _testService: BackflowTestService,
        private readonly _lookupService: LookupService,
        private readonly _authService: AuthService,
        private readonly _toastService: ToastService,
        private readonly _helper: HelperService,
        private readonly _downloadService: DownloadService,
        private readonly _settingsService: BackflowTestingSettingsService,
        private readonly _modalHelper: ModalHelperService
    ) { }

    public get isExpired(): boolean {
        if (!this.test?.expirationDate) {
            return false;
        }

        return new Date(this.test.expirationDate) < new Date();
    }

    public async ngOnInit(): Promise<void> {
        await this.initialize();
    }

    private async initialize(): Promise<void> {
        const [states, canModify, canViewTesters, canForceRenewal, settings] = await Promise.all([
            this._lookupService.getAllStatesAsOptions(true),
            this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.BackflowTests),
            this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowTesters),
            this._authService.hasAnyFeatures(FeatureType.BackflowTestForceRenewal),
            this._settingsService.get()
        ]);

        this.states = states;
        this.canModify = canModify;
        this.canViewTesters = canViewTesters;
        this.canForceRenewal = canForceRenewal;
        this.settings = settings;

        this._activatedRoute.paramMap.subscribe(async params => {
            const id = params.get('id');

            if (id) {
                this.id = +id;
                await this.loadTest();
            }
        });
    }

    private async loadTest(): Promise<void> {
        try {
            this.isLoading = true;
            this.test = await this._testService.get(this.id);
        } finally {
            this.isLoading = false;
        }
    }

    public onImageUrlUpdated(change: ImageUrlChange): void {
        if (this.test == null) {
            return;
        }
        (this.test as any)[change.urlKey] = change.value;
    }

    public async exportPdf(): Promise<void> {
        if (this.test == null) {
            return;
        }

        try {
            this.isLoading = true;
            const blob = await this._testService.getPdf(this.test.id);
            this._downloadService.downloadFileFromBlob(blob);
        } finally {
            this.isLoading = false;
        }
    }

    public async save(form: NgForm, entityName: string): Promise<void> {
        if (this.test == null || !this.canModify || !form.valid) {
            return;
        }

        try {
            this.savingSection = entityName;
            this.validationErrors = [];
            await this._testService.update(this.test);
            this.test = await this._testService.get(this.id);
            this._toastService.successfullySaved(entityName);
        } catch (e) {
            if (!this._helper.parseValidationErrors(e, this.validationErrors)) {
                throw e;
            }
            this._toastService.failedToSave(entityName);
        } finally {
            this.savingSection = null;
        }
    }

    public async toggleRenewalRequired(): Promise<void> {
        if (this.test == null) {
            return;
        }

        try {
            this.isLoading = true;
            this.test = await this._testService.updateRenewalRequired(this.test.id, !this.test.renewalRequired);
        } finally {
            this.isLoading = false;
        }
    }

    public async updateScheduleMonth(): Promise<void> {
        if (this.test == null) {
            return;
        }

        try {
            this.isLoading = true;
            this.test = await this._testService.updateScheduleMonth(this.test.id, this.test.backflowScheduleMonth ?? 0);
        } finally {
            this.isLoading = false;
        }
    }

    public async toggleIsCurrent(): Promise<void> {
        if (this.test == null) {
            return;
        }

        try {
            this.isLoading = true;
            this.test = await this._testService.updateIsCurrent(this.test.id, !this.test.isCurrent);
        } finally {
            this.isLoading = false;
        }
    }

    public async toggleOutOfService(): Promise<void> {
        if (this.test == null) {
            return;
        }

        try {
            this.isLoading = true;
            this.test = await this._testService.updateOutOfService(this.test.id, !this.test.outOfService);
        } finally {
            this.isLoading = false;
        }
    }

    public async toggleDisapproval(): Promise<void> {
        if (this.test == null) {
            return;
        }

        try {
            this.isLoading = true;
            this.test = await this._testService.updateDisapproval(this.test.id, !this.test.disapproved);
        } finally {
            this.isLoading = false;
        }
    }

    public toggleRejection(): void {
        if (this.test == null) {
            return;
        }

        if (this.test.rejected) {
            this.unrejectTest();
        } else {
            this._modalHelper.show<BackflowTest>(BackflowTestRejectComponent, {
                title: 'Reject Test',
                size: ModalSize.large,
                model: this.test
            }).result().subscribe(updated => {
                this.test = updated;
            });
        }
    }

    private async unrejectTest(): Promise<void> {
        try {
            this.isLoading = true;
            this.test = await this._testService.updateRejection(this.test!.id, { rejected: false });
        } finally {
            this.isLoading = false;
        }
    }

    public toggleForceRenewal(): void {
        if (this.test == null) {
            return;
        }

        this._modalHelper.show<BackflowTest>(BackflowTestForceRenewalComponent, {
            title: 'Force Renewal',
            size: ModalSize.large,
            model: this.test
        }).result().subscribe(updated => {
            this.test = updated;
        });
    }
}
