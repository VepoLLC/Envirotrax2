import { Component, EventEmitter, Input, OnInit, Optional, Output, SkipSelf } from "@angular/core";
import { ControlContainer, NgForm } from "@angular/forms";

@Component({
    standalone: false,
    selector: 'vp-lookup-field',
    templateUrl: './lookup-field.component.html',
    viewProviders: [
        {
            provide: ControlContainer,
            useFactory: (container: ControlContainer) => container,
            deps: [[new SkipSelf(), new Optional(), ControlContainer]]
        }
    ]
})
export class LookupFieldComponent implements OnInit {
    private static _counter: number = 0;

    @Input()
    public name: string = null!;

    @Input()
    public label: string = null!;

    @Input()
    public displayText?: string;

    @Input()
    public form?: NgForm;

    @Input()
    public required: boolean = false;

    @Input()
    public canSearch: boolean = false;

    @Output()
    public search: EventEmitter<void> = new EventEmitter();

    public ngOnInit(): void {
        LookupFieldComponent._counter++;

        if (!this.name) {
            this.name = `LookupField${LookupFieldComponent._counter}`;
        }
    }

    public onSearch(): void {
        if (this.canSearch) {
            this.search.emit();
        }
    }
}