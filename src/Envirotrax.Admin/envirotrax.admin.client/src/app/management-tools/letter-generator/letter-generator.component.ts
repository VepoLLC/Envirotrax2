import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { ProcessLettersComponent } from './process-letters/process-letters.component';

type LetterGeneratorTab = 'processLetters' | 'letterHistory';

@Component({
    imports: [
        CommonModule,
        ProcessLettersComponent
    ],
    templateUrl: './letter-generator.component.html'
})
export class LetterGeneratorComponent {
    public activeTab: LetterGeneratorTab = 'processLetters';

    public setActiveTab(tab: LetterGeneratorTab): void {
        this.activeTab = tab;
    }
}
