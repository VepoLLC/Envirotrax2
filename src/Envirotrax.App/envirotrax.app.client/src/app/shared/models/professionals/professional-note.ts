export interface ProfessionalNoteCreatedBy {
    id?: number;
    email?: string;
}

export interface ProfessionalNote {
    id?: number;
    noteText?: string;
    isReadOnly?: boolean;
    createdTime?: string;
    createdBy?: ProfessionalNoteCreatedBy;
}
