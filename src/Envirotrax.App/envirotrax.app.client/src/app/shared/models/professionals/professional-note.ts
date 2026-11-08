export interface ProfessionalNoteCreatedBy {
    id?: number;
    email?: string;
}

export interface ProfessionalNote {
    id?: number;
    noteText?: string;
    createdTime?: string;
    createdBy?: ProfessionalNoteCreatedBy;
}
