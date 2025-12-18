import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from "@angular/core";
import { PageInfo } from "../../../models/page-info";

@Component({
    selector: 'vp-pagination',
    templateUrl: './pagination.component.html',
    standalone: false,
    styleUrls: [
        './pagination.component.css'
    ]
})
export class PaginationComponent implements OnChanges {
    @Input()
    public pageInfo: PageInfo = null!;

    @Input()
    public isReadOnly: boolean = false;

    @Input()
    public maxVisiblePages: number = 5;

    @Output()
    public pageInfoChange: EventEmitter<PageInfo> = new EventEmitter();

    public visiblePages: number[] = [];

    public setDisabledState?(isDisabled: boolean): void {
        this.isReadOnly = isDisabled;
    }

    private getDistanceFromEachSide(): number {
        let maxPageWithoutCurrent = this.maxVisiblePages - 1;

        if (maxPageWithoutCurrent > 0) {
            return maxPageWithoutCurrent / 2;
        }

        return 5;
    }

    private getVisiblePageStart(): number {
        let start = (this.pageInfo?.pageNumber || 1) - this.getDistanceFromEachSide() - 1;

        if (start < 0) {
            return 0;
        }

        return start;
    }

    private setPageInfo(pageInfo: PageInfo): void {
        this.pageInfo = pageInfo;

        if (this.pageInfo && this.pageInfo?.totalPages! > 0) {
            this.visiblePages.splice(0, this.visiblePages.length);

            let start = this.getVisiblePageStart();
            let end = start + this.maxVisiblePages;

            for (let i = start; i < end; i++) {
                let page = i + 1;

                if (page > 0 && page <= this.pageInfo?.totalPages!) {
                    this.visiblePages.push(page);
                }
            }
        }
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if (changes['pageInfo']) {
            this.setPageInfo(changes['pageInfo'].currentValue as PageInfo);
        }
    }

    private raisePageChangedEvent(): void {
        this.setPageInfo(this.pageInfo);
        this.pageInfoChange.emit(this.pageInfo);
    }

    public prevPage(): void {
        this.pageInfo = this.pageInfo || {};
        this.pageInfo.pageNumber = (this.pageInfo?.pageNumber || 1) - 1;
        this.raisePageChangedEvent();
    }

    public nextPage(): void {
        this.pageInfo = this.pageInfo || {};
        this.pageInfo.pageNumber = (this.pageInfo.pageNumber || 1) + 1;
        this.raisePageChangedEvent();
    }

    public selectPage(pageNumber: number): void {
        this.pageInfo = this.pageInfo || {};
        this.pageInfo.pageNumber = pageNumber;
        this.raisePageChangedEvent();
    }
}
