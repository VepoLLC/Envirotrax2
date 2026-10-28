import { ApplicationRef, Injectable } from '@angular/core';
import { TableViewModel } from '../models/table-view-model';
import { PagedData } from '../models/paged-data';
import { MAX_PAGE_SIZE } from '../models/page-info';

const LOADING_BODY = '<div class="d-flex justify-content-center p-5"><div class="spinner-border text-primary" role="status"></div></div>';

@Injectable({
    providedIn: 'root'
})
export class PrintableTableService {
    constructor(
        private readonly _appRef: ApplicationRef
    ) {

    }

    public async open<T>(table: TableViewModel<T>, loadTable: () => Promise<void>, getSection: () => Element): Promise<void> {
        const printWin = window.open('', '_blank');

        if (!printWin) {
            return;
        }

        printWin.document.head.innerHTML = this.buildHead();
        printWin.document.body.className = 'p-3';
        printWin.document.body.innerHTML = LOADING_BODY;

        const currentItems = table.items;
        let isOpened = false;

        try {
            table.items = this.buildAllRowsPage(currentItems);

            await loadTable();
            this._appRef.tick();

            printWin.document.body.innerHTML = this.buildPrintableHtml(getSection());
            isOpened = true;
        } finally {
            table.items = currentItems;

            if (!isOpened) {
                printWin.close();
            }
        }
    }

    private buildAllRowsPage<T>(currentItems: PagedData<T> | undefined): PagedData<T> {
        let data: T[] = [];

        if (currentItems) {
            data = currentItems.data;
        }

        return {
            data: data,
            pageInfo: { pageNumber: 1, pageSize: MAX_PAGE_SIZE }
        };
    }

    private buildHead(): string {
        const base = `<base href="${window.location.origin}/">`;
        const styles = [...document.querySelectorAll('link[rel="stylesheet"], style')]
            .map(el => el.outerHTML).join('\n');

        return `<meta charset="utf-8">${base}${styles}`;
    }

    private buildPrintableHtml(sectionElement: Element): string {
        const clone = sectionElement.cloneNode(true) as Element;

        clone.querySelectorAll('tr').forEach(row => {
            const cells = row.querySelectorAll('th, td');

            if (cells.length > 0) {
                cells[cells.length - 1].remove();
            }
        });

        clone.querySelectorAll('vp-pagination').forEach(el => el.remove());
        clone.querySelectorAll('.border-bottom.d-md-flex').forEach(el => el.remove());

        return clone.outerHTML;
    }
}
