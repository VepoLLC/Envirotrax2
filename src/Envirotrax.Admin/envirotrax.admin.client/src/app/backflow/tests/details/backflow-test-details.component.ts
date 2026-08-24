import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { InputOption } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import {
    BackflowTestDetails,
    backflowDeviceTypeDescriptions
} from '../../../shared/models/backflow/backflow-test';
import { State } from '../../../shared/models/lookup/state';
import { BackflowTestService } from '../../../shared/services/backflow/backflow-test.service';
import { BackflowTestOptionsService } from '../../../shared/services/backflow/backflow-test-options.service';
import { LookupService } from '../../../shared/services/lookup/lookup.service';
import { WindowService } from '../../../shared/services/window.service';
import { SiteEditComponent } from '../../../sites/edit/site-edit.component';
import { WindowReference } from '../../../window/window-config';
import { BackflowTestAdditionalInformationComponent } from './additional-information/backflow-test-additional-information.component';
import { BackflowTestAssemblyComponent } from './assembly/backflow-test-assembly.component';
import { BackflowAssemblyVisibility, buildAssemblyVisibility } from './backflow-test-visibility';
import { BackflowTestBpatComponent } from './bpat/backflow-test-bpat.component';
import { BackflowTestImagesComponent } from './images/backflow-test-images.component';
import { BackflowTestMailingComponent } from './mailing/backflow-test-mailing.component';
import { BackflowTestPropertyComponent } from './property/backflow-test-property.component';
import { BackflowTestReadingsComponent } from './readings/backflow-test-readings.component';
import { BackflowTestRecordLogComponent } from './record-log/backflow-test-record-log.component';
import { BackflowTestRemarksComponent } from './remarks/backflow-test-remarks.component';
import { BackflowTestResultsComponent } from './results/backflow-test-results.component';
import { BackflowTestTransactionComponent } from './transaction/backflow-test-transaction.component';
import { BackflowTestValidationNotesComponent } from './validation-notes/backflow-test-validation-notes.component';
import { BackflowTestWaterSupplierComponent } from './water-supplier/backflow-test-water-supplier.component';

type BackflowTestTab = 'results' | 'images' | 'logs';

const SaveMessageDurationMs = 5000;

const DaysInMillisecond = 1000 * 60 * 60 * 24;

@Component({
    templateUrl: './backflow-test-details.component.html',
    imports: [
        CommonModule,
        FormsModule,
        SharedComponentsModule,
        BackflowTestValidationNotesComponent,
        BackflowTestTransactionComponent,
        BackflowTestWaterSupplierComponent,
        BackflowTestBpatComponent,
        BackflowTestPropertyComponent,
        BackflowTestMailingComponent,
        BackflowTestAssemblyComponent,
        BackflowTestResultsComponent,
        BackflowTestReadingsComponent,
        BackflowTestAdditionalInformationComponent,
        BackflowTestRemarksComponent,
        BackflowTestImagesComponent,
        BackflowTestRecordLogComponent
    ]
})
export class BackflowTestDetailsComponent implements OnInit, OnDestroy {
    @ViewChild(BackflowTestRecordLogComponent)
    public recordLog?: BackflowTestRecordLogComponent;

    public id: number = 0;
    public idPrefix: string = 'backflow-test';

    public isLoading: boolean = false;
    public isSaving: boolean = false;
    public saveSuccessMessage: string = '';

    private _saveMessageTimeoutId?: ReturnType<typeof setTimeout>;

    public test: BackflowTestDetails = {};

    public selectedTab: BackflowTestTab = 'results';

    public recordLogTabTitle: string = 'Record Log';
    public testResultsTabTitle: string = 'Test Results';

    public approvedByText: string = '';
    public rejectedByText: string = '';

    public expirationBadgeClass: string = 'text-bg-secondary';

    public scheduleMonthId: string = '0';
    public forceRenewalYearsId: string = '0';

    public visibility: BackflowAssemblyVisibility = buildAssemblyVisibility(undefined);

    public stateOptions: InputOption<State>[] = [];

    public readonly scheduleMonthOptions: InputOption[];
    public readonly forceRenewalYearsOptions: InputOption[];

    constructor(
        private readonly _windowReference: WindowReference<{ id?: number }>,
        private readonly _testService: BackflowTestService,
        private readonly _lookupService: LookupService,
        private readonly _windowService: WindowService,
        private readonly _options: BackflowTestOptionsService
    ) {
        this.scheduleMonthOptions = this._options.scheduleMonthOptions;
        this.forceRenewalYearsOptions = this._options.forceRenewalYearsOptions;
    }

    public async ngOnInit(): Promise<void> {
        this.id = this._windowReference.config.model?.id ?? 0;
        this.idPrefix = `backflow-test-${this.id}`;

        await Promise.all([
            this.loadStates(),
            this.loadTest(),
            this.loadCounts()
        ]);
    }

    public ngOnDestroy(): void {
        this.dismissSaveMessage();
    }

    public onTabChange(tab: BackflowTestTab): void {
        this.selectedTab = tab;
    }

    public onDeviceTypeChange(): void {
        this.visibility = buildAssemblyVisibility(this.test.deviceType);
        this.testResultsTabTitle = this.buildTestResultsTabTitle();
    }

    public toggleIsCurrent(): void {
        this.test.isCurrent = !this.test.isCurrent;
    }

    public toggleOutOfService(): void {
        this.test.outOfService = !this.test.outOfService;
    }

    public toggleDisapproved(): void {
        this.test.disapproved = !this.test.disapproved;
    }

    public toggleRejected(): void {
        this.test.rejected = !this.test.rejected;
    }

    public toggleNeedsValidation(): void {
        this.test.needsValidation = !this.test.needsValidation;
    }

    public toggleForceRenewal(): void {
        this.test.forceRenewal = !this.test.forceRenewal;
    }

    public toggleRenewalRequired(): void {
        this.test.renewalRequired = !this.test.renewalRequired;
        this.expirationBadgeClass = this.buildExpirationBadgeClass();
    }

    public onScheduleMonthChange(value: string): void {
        this.test.backflowScheduleMonth = Number(value);
    }

    public onForceRenewalYearsChange(value: string): void {
        this.test.forceRenewalYears = Number(value);
    }

    public openSite(): void {
        const siteId = this.test.site?.id;

        if (siteId == null) {
            return;
        }

        this._windowService.addWindow(SiteEditComponent, {
            title: this.test.site?.accountNumber ?? 'Site',
            model: {
                siteId: siteId,
                waterSupplierId: this.test.waterSupplier?.id
            }
        });
    }

    public async save(): Promise<void> {
        await this.saveTest();
    }

    public async saveAndClose(): Promise<void> {
        const saved = await this.saveTest();

        if (saved) {
            this._windowReference.close();
        }
    }

    public dismissSaveMessage(): void {
        this.saveSuccessMessage = '';

        if (this._saveMessageTimeoutId != null) {
            clearTimeout(this._saveMessageTimeoutId);
            this._saveMessageTimeoutId = undefined;
        }
    }

    private async saveTest(): Promise<boolean> {
        this.dismissSaveMessage();

        try {
            this.isSaving = true;
            this.test = await this._testService.update(this.id, this.buildUpdateRequest());
        } finally {
            this.isSaving = false;
        }

        this.applyTestToEditors();

        await this.loadCounts();
        await this.reloadRecordLog();

        this.showSaveMessage();

        return true;
    }

    private buildUpdateRequest(): BackflowTestDetails {
        const test = this.test;

        const request: BackflowTestDetails = { ...test };

        request.testDate = this.toTextOrUndefined(test.testDate);
        request.expirationDate = this.toTextOrUndefined(test.expirationDate);
        request.transactionDate = this.toTextOrUndefined(test.transactionDate);
        request.initialTestDate = this.toTextOrUndefined(test.initialTestDate);
        request.repairTestDate = this.toTextOrUndefined(test.repairTestDate);

        request.amount = this.toNumberOrUndefined(test.amount) ?? 0;
        request.amountShare = this.toNumberOrUndefined(test.amountShare) ?? 0;

        request.initCV1HeldPSID = this.toNumberOrUndefined(test.initCV1HeldPSID);
        request.initCV2HeldPSID = this.toNumberOrUndefined(test.initCV2HeldPSID);
        request.initRVOpenedPSID = this.toNumberOrUndefined(test.initRVOpenedPSID);
        request.initBCHeldPSID = this.toNumberOrUndefined(test.initBCHeldPSID);
        request.initPvbAirInletOpenedPSID = this.toNumberOrUndefined(test.initPvbAirInletOpenedPSID);
        request.initPvbCVHeldPSID = this.toNumberOrUndefined(test.initPvbCVHeldPSID);
        request.initCV1HeldPSID2 = this.toNumberOrUndefined(test.initCV1HeldPSID2);
        request.initCV2HeldPSID2 = this.toNumberOrUndefined(test.initCV2HeldPSID2);
        request.initRVOpenedPSID2 = this.toNumberOrUndefined(test.initRVOpenedPSID2);

        request.finalCV1HeldPSID = this.toNumberOrUndefined(test.finalCV1HeldPSID);
        request.finalCV2HeldPSID = this.toNumberOrUndefined(test.finalCV2HeldPSID);
        request.finalRVOpenedPSID = this.toNumberOrUndefined(test.finalRVOpenedPSID);
        request.finalBCHeldPSID = this.toNumberOrUndefined(test.finalBCHeldPSID);
        request.finalPvbAirInletOpenedPSID = this.toNumberOrUndefined(test.finalPvbAirInletOpenedPSID);
        request.finalPvbCVHeldPSID = this.toNumberOrUndefined(test.finalPvbCVHeldPSID);
        request.finalCV1HeldPSID2 = this.toNumberOrUndefined(test.finalCV1HeldPSID2);
        request.finalCV2HeldPSID2 = this.toNumberOrUndefined(test.finalCV2HeldPSID2);
        request.finalRVOpenedPSID2 = this.toNumberOrUndefined(test.finalRVOpenedPSID2);

        request.meterReadingBefore = this.toNumberOrUndefined(test.meterReadingBefore);
        request.meterReadingAfter = this.toNumberOrUndefined(test.meterReadingAfter);

        return request;
    }

    private toNumberOrUndefined(value: unknown): number | undefined {
        if (value === null || value === undefined || value === '') {
            return undefined;
        }

        const parsed = Number(value);

        return Number.isNaN(parsed) ? undefined : parsed;
    }

    private toTextOrUndefined(value: unknown): string | undefined {
        if (value === null || value === undefined || value === '') {
            return undefined;
        }

        return String(value);
    }

    private showSaveMessage(): void {
        this.saveSuccessMessage = 'Backflow test saved successfully.';

        this._saveMessageTimeoutId = setTimeout(() => this.dismissSaveMessage(), SaveMessageDurationMs);
    }

    private async reloadRecordLog(): Promise<void> {
        if (this.recordLog == null) {
            return;
        }

        await this.recordLog.reload();
    }

    private async loadStates(): Promise<void> {
        this.stateOptions = await this._lookupService.getStatesAsOptions();
    }

    private async loadTest(): Promise<void> {
        try {
            this.isLoading = true;
            this.test = await this._testService.get(this.id);
        } finally {
            this.isLoading = false;
        }

        this.applyTestToEditors();
    }

    private async loadCounts(): Promise<void> {
        const counts = await this._testService.getCounts(this.id);

        this.recordLogTabTitle = `Record Log (${counts.recordLogCount ?? 0})`;
    }

    private applyTestToEditors(): void {
        this.scheduleMonthId = String(this.test.backflowScheduleMonth ?? 0);
        this.forceRenewalYearsId = String(this.test.forceRenewalYears ?? 0);

        this.visibility = buildAssemblyVisibility(this.test.deviceType);
        this.testResultsTabTitle = this.buildTestResultsTabTitle();
        this.expirationBadgeClass = this.buildExpirationBadgeClass();

        this.approvedByText = this.buildReviewerText(
            this.test.approvedBy?.contactName ?? this.test.approvedBy?.emailAddress,
            this.test.approvalDate);

        this.rejectedByText = this.buildReviewerText(
            this.test.rejectedBy?.contactName ?? this.test.rejectedBy?.emailAddress,
            this.test.rejectedDate);
    }

    private buildTestResultsTabTitle(): string {
        const description = this.test.deviceType
            ? backflowDeviceTypeDescriptions[this.test.deviceType]
            : undefined;

        return description ? `${description} - Test Results` : 'Test Results';
    }

    private buildReviewerText(name?: string, date?: string): string {
        if (!name || !date) {
            return '';
        }

        return `${name} on ${new Date(date).toLocaleString()}`;
    }

    private buildExpirationBadgeClass(): string {
        if (!this.test.renewalRequired) {
            return 'text-bg-secondary';
        }

        if (!this.test.expirationDate) {
            return 'text-bg-secondary';
        }

        const expiration = new Date(this.test.expirationDate).getTime();
        const daysExpired = Math.floor((Date.now() - expiration) / DaysInMillisecond);

        if (daysExpired <= 0) {
            return 'text-bg-success';
        }

        if (daysExpired <= 60) {
            return 'text-bg-warning';
        }

        return 'text-bg-danger';
    }
}
