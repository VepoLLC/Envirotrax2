
export interface Query {
    sort?: { [key: string]: SortOperator },
    filter?: QueryProperty[];
}

export type SortOperator = 'Asc' | 'Desc';

export type ComparisonOperator =
    'Eq' | // Equals
    'NotEq' | // Not Equals
    'Gt' | // Greater Than
    'Gte' | // Greater Than or Equal
    'Lt' | // Less Than
    'Lte' | // Less Than or Equal
    'StW' | // Starts With
    'EndW' | // Ends With
    'Ct' | // Contains
    'NotStW' | // Not Starts With
    'NotEndW' | // Not Ends With
    'NotCt'; // Not Contains

export interface QueryProperty {
    columnName?: string;
    value?: string;
    isValueNull?: boolean;
    logicalOperator?: 'And' | 'Or';
    comparisonOperator?: ComparisonOperator;
    children?: QueryProperty[]
    tag?: any
}