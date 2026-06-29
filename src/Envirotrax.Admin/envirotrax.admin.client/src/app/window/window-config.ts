
export interface WindowConfig<T> {
    title: string;
    model?: T;
}

export class WindowReference<T> {
    public config: WindowConfig<T>;

    constructor(config: WindowConfig<T>) {
        this.config = config;
    }
}