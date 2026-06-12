import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { NgForm } from "@angular/forms";
import { BackflowTest } from "../../../shared/models/backflow/backflow-test";
import { BackflowTestService } from "../../../shared/services/backflow/backflow-test.service";
import { State } from "../../../shared/models/lookup/state";
import { LookupService } from "../../../shared/services/lookup/lookup.service";
import { AuthService } from "../../../shared/services/auth/auth.service";
import { PermissionAction, PermissionType } from "../../../shared/models/permission-type";
import { ToastService } from "../../../shared/services/toast.service";
import { HelperService } from "../../../shared/services/helpers/helper.service";
import { ImageUrlChange } from "./images/backflow-test-images.component";
import { InputOption } from "@envirotrax/common-ui";

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
    public states: InputOption<State>[] = [];
    public validationErrors: string[] = [];

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _testService: BackflowTestService,
        private readonly _lookupService: LookupService,
        private readonly _authService: AuthService,
        private readonly _toastService: ToastService,
        private readonly _helper: HelperService
    ) { }

    public async ngOnInit(): Promise<void> {
        await this.initialize();
    }

    private async initialize(): Promise<void> {
        const [states, canModify, canViewTesters] = await Promise.all([
            this._lookupService.getAllStatesAsOptions(true),
            this._authService.hasAnyPermisison(PermissionAction.CanModify, PermissionType.BackflowTests),
            this._authService.hasAnyPermisison(PermissionAction.CanView, PermissionType.BackflowTesters)
        ]);

        this.states = states;
        this.canModify = canModify;
        this.canViewTesters = canViewTesters;

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
}
