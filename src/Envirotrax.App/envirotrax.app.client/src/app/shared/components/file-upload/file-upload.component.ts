import { Component, EventEmitter, forwardRef, Input, Output } from "@angular/core";
import { ControlValueAccessor, NgForm, NG_VALUE_ACCESSOR } from "@angular/forms";

@Component({
    selector: 'vp-file-upload',
    templateUrl: './file-upload.component.html',
    standalone: false,
    providers: [
        {
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => FileUploadComponent),
            multi: true
        }
    ],
    styles: `
        .drop-zone {
            border: 2px dashed var(--bs-border-color);
            border-radius: 0.5rem;
            padding: 2rem;
            text-align: center;
            cursor: pointer;
            transition: border-color 0.2s, background-color 0.2s;
        }

        .drop-zone:hover,
        .drop-zone.drag-over {
            border-color: var(--bs-primary);
            background-color: var(--bs-primary-bg-subtle);
        }

        .drop-zone.has-file {
            border-color: var(--bs-success);
            background-color: var(--bs-success-bg-subtle);
        }

        .file-icon {
            font-size: 2rem;
            margin-bottom: 0.5rem;
        }
    `
})
export class FileUploadComponent implements ControlValueAccessor {
    private _onChanged: (value: any) => void = null!;
    private _onTouched: () => void = null!;

    @Input()
    public name: string = 'fileUpload';

    @Input()
    public accept: string = '*/*';

    @Input()
    public label: string = 'Choose a file or drag and drop here';

    @Input()
    public validationLabel: string = 'File';

    @Input()
    public required: boolean = false;

    @Input()
    public form?: NgForm;

    @Input()
    public file: File | null = null;

    @Output()
    public fileChange = new EventEmitter<File | null>();

    public isDragOver: boolean = false;
    public extensionError: string | null = null;

    public get fileNameValue(): string {
        return this.file?.name ?? '';
    }

    public writeValue(obj: File | null): void {
        this.file = obj ?? null;
    }

    public registerOnChange(fn: any): void {
        this._onChanged = fn;
    }

    public registerOnTouched(fn: any): void {
        this._onTouched = fn;
    }

    public onFileSelected(event: Event): void {
        const input = event.target as HTMLInputElement;
        const file = input.files?.[0] ?? null;
        this.setFile(file);
        input.value = '';
    }

    public onDragOver(event: DragEvent): void {
        event.preventDefault();
        event.stopPropagation();
        this.isDragOver = true;
    }

    public onDragLeave(event: DragEvent): void {
        event.preventDefault();
        event.stopPropagation();
        this.isDragOver = false;
    }

    public onDrop(event: DragEvent): void {
        event.preventDefault();
        event.stopPropagation();
        this.isDragOver = false;

        const file = event.dataTransfer?.files?.[0] ?? null;
        if (file) {
            this.setFile(file);
        }
    }

    public clear(event: MouseEvent): void {
        event.stopPropagation();
        this.setFile(null);
    }

    private validateExtension(file: File | null): boolean {
        if (file && this.accept != '*/*') {
            const ext = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();

            const normalized = this.accept.split(',').map(e => {
                const cleaned = e.trim().toLowerCase();
                return cleaned.startsWith('.') ? cleaned : `.${cleaned}`
            });

            if (!normalized.includes(ext)) {
                this.extensionError = `Only ${normalized.join(', ')} files are accepted.`;
                return false;
            }
        }

        this.extensionError = null;

        return true;
    }

    private setFile(file: File | null): void {
        if (this.validateExtension(file)) {
            this.file = file;
            this.fileChange.emit(file);
            if (this._onChanged) this._onChanged(file);
            if (this._onTouched) this._onTouched();
        }
    }

    public formatSize(bytes: number): string {
        if (bytes < 1024) return `${bytes} B`;
        if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
        return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
    }
}
