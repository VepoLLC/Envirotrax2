export enum RecordLogType {
    Login = 0,
    View = 1,
    Add = 2,
    Edit = 3,
    Delete = 4,
    Export = 5
}

export const recordLogTypeLabels: Record<RecordLogType, string> = {
    [RecordLogType.Login]: 'Login',
    [RecordLogType.View]: 'View',
    [RecordLogType.Add]: 'Add',
    [RecordLogType.Edit]: 'Edit',
    [RecordLogType.Delete]: 'Delete',
    [RecordLogType.Export]: 'Export'
};

export class RecordLog {
    id?: number;
    logDate?: string;
    logType?: RecordLogType;
    tableName?: string;
    recordId?: number;
    description?: string;
    user?: { id?: number; email?: string };
}
