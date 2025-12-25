import { Injectable } from "@angular/core";
import { RouterStateSnapshot, TitleStrategy } from "@angular/router";
import { TitleHelperService } from "./title-helper.service";

@Injectable()
export class AppTitleStrategy extends TitleStrategy {
    constructor(private readonly _titleHelper: TitleHelperService) {
        super();
    }

    override updateTitle(routerState: RouterStateSnapshot): void {
        const routeTitle = this.buildTitle(routerState);

        if (routeTitle) {
            this._titleHelper.setTitle(routeTitle);
        }
    }
}