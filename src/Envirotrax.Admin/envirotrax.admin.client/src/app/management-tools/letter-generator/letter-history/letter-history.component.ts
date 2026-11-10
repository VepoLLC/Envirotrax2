import { CommonModule, formatDate } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ModalSize } from '@developer-partners/ngx-modal-dialog';
import { ModalHelperService, ToastService } from '@envirotrax/common-ui';
import { SharedComponentsModule } from '../../../shared/components/shared.components.module';
import { LetterProcess } from '../../../shared/models/letters/letter-process';
import { LetterProcessService } from '../../../shared/services/letters/letter-process.service';
import { LetterFilesComponent, LetterFilesModalData } from './letter-files/letter-files.component';

@Component({
    selector: 'vp-letter-history',
    imports: [
        CommonModule,
        SharedComponentsModule
    ],
    templateUrl: './letter-history.component.html'
})
export class LetterHistoryComponent implements OnInit {
    public processes: LetterProcess[] = [];

    public isLoading: boolean = false;

    constructor(
        private readonly _letterProcessService: LetterProcessService,
        private readonly _modalHelper: ModalHelperService,
        private readonly _toastService: ToastService
    ) {

    }

    public async ngOnInit(): Promise<void> {
        await this.loadProcesses();
    }

    public viewFiles(process: LetterProcess): void {
        this._modalHelper.show<LetterFilesModalData, void>(LetterFilesComponent, {
            title: `Letters Generated for ${process.id}`,
            model: { processId: process.id! },
            size: ModalSize.large,
            mode: 'disableFullScreen'
        });
    }

    public deleteProcess(process: LetterProcess): void {
        const processDate = formatDate(process.processDate!, 'MMMM d y h:mm a', 'en-US');

        this._modalHelper.confirm({
            title: 'Confirm Process Deletion',
            messages: [`Are you sure you want to delete the process dated ${processDate}?`]
        }).result().subscribe(async () => {
            try {
                this.isLoading = true;

                await this._letterProcessService.delete(process.id!);

                this._toastService.successFullyDeleted('Letter Process');
            } finally {
                this.isLoading = false;
            }

            await this.loadProcesses();
        });
    }

    private async loadProcesses(): Promise<void> {
        try {
            this.isLoading = true;

            this.processes = await this._letterProcessService.getAll();
        } finally {
            this.isLoading = false;
        }
    }
}
