import { LetterFile } from './letter-file';

export interface LetterHistory {
    id?: string;
    processDate?: string;
    files?: LetterFile[];
}
