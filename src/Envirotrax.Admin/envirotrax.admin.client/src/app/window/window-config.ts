
export interface WindowConfig<T> {
    title: string;
    model?: T;
}

export class WindowReference<T> {
    public config: WindowConfig<T>;

    // Wired by the host WindowComponent so a windowed component can close itself programmatically
    // (same path as the title-bar close button — removes this window, leaving the rest untouched).
    private _closeHandler?: () => void;

    constructor(config: WindowConfig<T>) {
        this.config = config;
    }

    public setCloseHandler(handler: () => void): void {
        this._closeHandler = handler;
    }

    public close(): void {
        this._closeHandler?.();
    }
}