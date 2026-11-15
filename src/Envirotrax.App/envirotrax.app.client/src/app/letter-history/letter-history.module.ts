import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedComponentsModule } from '../shared/components/shared.components.module';
import { LetterHistoryRoutingModule } from './letter-history-routing.module';
import { LetterHistoryComponent } from './letter-history.component';

@NgModule({
    declarations: [LetterHistoryComponent],
    imports: [CommonModule, SharedComponentsModule, LetterHistoryRoutingModule]
})
export class LetterHistoryModule {}
