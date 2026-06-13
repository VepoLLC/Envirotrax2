import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { BackflowTest } from "../../../../shared/models/backflow/backflow-test";
import { BackflowTestService } from "../../../../shared/services/backflow/backflow-test.service";

@Component({
    selector: 'app-professional-backflow-test-details',
    standalone: false,
    templateUrl: './professional-backflow-test-details.component.html'
})
export class ProfessionalBackflowTestDetailsComponent implements OnInit {
    public id: number = 0;
    public test: BackflowTest | null = null;
    public isLoading: boolean = false;

    constructor(
        private readonly _activatedRoute: ActivatedRoute,
        private readonly _testService: BackflowTestService
    ) { }

    public ngOnInit(): void {
        this.initialize();
    }

    private initialize(): void {
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
            this.test = await this._testService.getForProfessional(this.id);
        } finally {
            this.isLoading = false;
        }
    }
}
