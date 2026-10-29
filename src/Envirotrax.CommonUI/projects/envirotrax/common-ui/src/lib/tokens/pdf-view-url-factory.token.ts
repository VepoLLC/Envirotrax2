import { InjectionToken } from '@angular/core';

export type PdfViewUrlFactory = (blob: Blob, fileName: string) => Promise<string>;

export const PDF_VIEW_URL_FACTORY = new InjectionToken<PdfViewUrlFactory>('PDF_VIEW_URL_FACTORY');
