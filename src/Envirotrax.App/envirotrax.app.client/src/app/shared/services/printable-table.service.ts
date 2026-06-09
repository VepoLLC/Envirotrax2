import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class PrintableTableService {
    public open(sectionElement: Element): void {
        const clone = sectionElement.cloneNode(true) as Element;

        clone.querySelectorAll('tr').forEach(row => {
            const cells = row.querySelectorAll('th, td');
            if (cells.length > 0) cells[cells.length - 1].remove();
        });

        clone.querySelectorAll('vp-pagination').forEach(el => el.remove());
        clone.querySelectorAll('.border-bottom.d-md-flex').forEach(el => el.remove());

        const base = `<base href="${window.location.origin}/">`;
        const styles = [...document.querySelectorAll('link[rel="stylesheet"], style')]
            .map(el => el.outerHTML).join('\n');

        const printWin = window.open('', '_blank');
        if (!printWin) return;
        printWin.document.write(
            `<!DOCTYPE html><html lang="en"><head><meta charset="utf-8">${base}${styles}</head>` +
            `<body class="p-3">${clone.outerHTML}</body></html>`
        );
        printWin.document.close();
    }
}
