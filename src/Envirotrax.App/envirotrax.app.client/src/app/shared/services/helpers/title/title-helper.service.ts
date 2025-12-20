import { Injectable } from "@angular/core";
import { Title } from "@angular/platform-browser";

@Injectable({
    providedIn: 'root'
})
export class TitleHelperService {
    constructor(private readonly _titleService: Title) {

    }

    public setTitle(title: string): void {
        this._titleService.setTitle(`Envirotrax - ${title}`);
    }
}