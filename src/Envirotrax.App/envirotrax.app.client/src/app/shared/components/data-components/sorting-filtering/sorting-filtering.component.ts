import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from "@angular/core";
import { ModalService, ModalSize } from "@developer-partners/ngx-modal-dialog";
import { QueryColumn, QueryViewModel } from "./query-view-model";
import { SortingFilteringModalComponent } from "./sorting-filtering-modal.component";
import { Query } from "../../../models/query";

@Component({
    selector: 'vp-sorting-filtering',
    templateUrl: './sorting-filtering.component.html',
    standalone: false,
})
export class SortingFilteringComponent implements OnChanges {
    @Input()
    public isReadOnly: boolean = false;

    @Input()
    public columns: QueryColumn[] = null!;

    @Input()
    public query: Query = null!;

    @Output()
    public queryChange: EventEmitter<Query> = new EventEmitter<Query>();

    @Output()
    public queryReset: EventEmitter<void> = new EventEmitter();

    public hasFilters: boolean = false;
    public hasFiltersText: string = 'Show Sorting and Filtering. The data set has filters applied to it. Please click to see them.';
    public hasNoFiltersText: string = 'Show Sorting and Filtering.';

    constructor(private readonly _modalService: ModalService) {
    }

    private getHasFilters(): boolean {
        if (this.query) {
            if (this.query.sort) {
                if (Object.keys(this.query.sort).length > 0) {
                    return true;
                }
            }

            if (this.query.filter) {
                if (this.query.filter.length > 0) {
                    return true;
                }
            }
        }

        return false;
    }

    public ngOnChanges(changes: SimpleChanges): void {
        if (changes['query']) {
            this.hasFilters = this.getHasFilters();
        }
    }

    public showQueryDialog(): void {
        this.query = this.query || {
            filter: [],
            sort: {}
        };

        this._modalService.show<QueryViewModel>(SortingFilteringModalComponent, {
            title: 'Sorting & Filtering',
            size: ModalSize.large,
            position: 'left',
            model: {
                query: this.query,
                columns: this.columns
            }
        }).result()
            .subscribe({
                next: data => {
                    this.query.sort = data.query.sort;
                    this.query.filter = data.query.filter;
                    this.hasFilters = this.getHasFilters();

                    if (data.wasReset) {
                        this.queryReset.emit();
                    }

                    this.queryChange.emit(this.query);
                }
            });
    }
}
