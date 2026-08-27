import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { SharedComponentsModule } from '../../../../shared/components/shared.components.module';
import { BackflowTestDetails } from '../../../../shared/models/backflow/backflow-test';
import { SiteLog, siteLogTypeLabels } from '../../../../shared/models/sites/site-log';
import { BackflowTestService } from '../../../../shared/services/backflow/backflow-test.service';

interface ValidationNoteRow extends SiteLog {
    logTypeLabel: string;
}

@Component({
    selector: 'vp-backflow-test-validation-notes',
    templateUrl: './backflow-test-validation-notes.component.html',
    imports: [CommonModule, FormsModule, SharedComponentsModule]
})
export class BackflowTestValidationNotesComponent implements OnInit {
    @Input() public test: BackflowTestDetails = {};
    @Input() public form?: NgForm;

    public isLoading: boolean = false;

    public isExpanded: boolean = false;

    public notes: ValidationNoteRow[] = [];

    constructor(private readonly _testService: BackflowTestService) {

    }

    public async ngOnInit(): Promise<void> {
        this.isExpanded = !!this.test.validationNotes;

        if (this.test.id == null) {
            return;
        }

        try {
            this.isLoading = true;

            const logs = await this._testService.getSiteLogs(this.test.id);

            this.notes = logs.map(log => ({
                ...log,
                logTypeLabel: log.logType == null ? '' : siteLogTypeLabels[log.logType]
            }));

            if (this.notes.length > 0) {
                this.isExpanded = true;
            }
        } finally {
            this.isLoading = false;
        }
    }
}
