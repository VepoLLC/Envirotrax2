import { Injectable } from "@angular/core";
import { PageInfo } from "../../models/page-info";
import { HttpParams, HttpUrlEncodingCodec } from "@angular/common/http";
import { ComparisonOperator, Query, QueryProperty } from "../../models/query";
import { DateRange } from "../../components/input/input.component";

@Injectable({
    providedIn: 'root'
})
export class QueryHelperService {
    public pageInfoToQueryString(pageInfo: PageInfo): HttpParams {
        return new HttpParams({ encoder: new ParamEncoder() })
            .append('pageNumber', pageInfo.pageNumber || 1)
            .append('pageSize', pageInfo.pageSize || 20);
    }

    private isDefined(value: any): boolean {
        if (value !== null && value !== undefined) {
            return true;
        }

        return false;
    }

    private setQuery(params: HttpParams, queryProperties?: QueryProperty[], parentPath?: string): HttpParams {
        if (queryProperties) {
            for (let i = 0; i < queryProperties.length; i++) {
                const itemPath = parentPath
                    ? `${parentPath}.q[${i}]`
                    : `q[${i}]`;

                params = params.append(`${itemPath}.col`, queryProperties[i].columnName || '');

                if (this.isDefined(queryProperties[i].value)) {
                    params = params.append(`${itemPath}.val`, queryProperties[i].value!.toString());
                }

                if (this.isDefined(queryProperties[i].logicalOperator)) {
                    params = params.append(`${itemPath}.lop`, queryProperties[i].logicalOperator!);
                }

                if (this.isDefined(queryProperties[i].comparisonOperator)) {
                    params = params.append(`${itemPath}.op`, queryProperties[i].comparisonOperator!);
                }

                if (this.isDefined(queryProperties[i].isValueNull)) {
                    params = params.append(`${itemPath}.null`, queryProperties[i].isValueNull!);
                }

                params = this.setQuery(params, queryProperties[i].children, itemPath);
            }
        }

        return params;
    }

    public queryToQueryString(query: Query, params?: HttpParams): HttpParams {
        params = params || new HttpParams({
            encoder: new ParamEncoder()
        });

        if (query.sort) {
            for (let key in query.sort) {
                params = params.append(`s[${key}]`, query.sort[key]);
            }
        }

        params = this.setQuery(params, query.filter, undefined);

        return params;
    }

    public buildQuery(pageInfo: PageInfo, query: Query): HttpParams {
        let params = this.pageInfoToQueryString(pageInfo);

        params = this.queryToQueryString(query, params);

        return params;
    }

    private getOperator(field: FilterFieldVm): ComparisonOperator {
        switch (field.type) {
            case 'text':
                return 'Ct';
            case 'date':
                return 'Gte';
            default:
                return 'Eq';
        }
    }

    private isDateRange(value: string | DateRange | undefined): value is DateRange {
        if (value && !(typeof value == 'string')) {
            if ('startDate' in value || 'endDate' in value) {
                return true;
            }
        }

        return false;
    }

    private getChildren(field: FilterFieldVm): QueryProperty[] {
        const children: QueryProperty[] = [];

        if (this.isDateRange(field.value) && (field.value.startDate || field.value.endDate)) {
            if (field.value.startDate) {
                children.push({
                    columnName: field.fieldName,
                    value: field.value.startDate,
                    comparisonOperator: 'Gte',
                    logicalOperator: 'And'
                });
            }

            if (field.value.endDate) {
                children.push({
                    columnName: field.fieldName,
                    value: field.value.endDate,
                    comparisonOperator: 'Lte',
                    logicalOperator: 'And'
                });
            }
        }

        return children;
    }

    public convertFilterPanelFields(fields: FilterFieldVm[]): QueryProperty[] {
        return fields
            .filter(f => f.value)
            .map(f => ({
                columnName: f.fieldName,
                value: typeof f.value == 'string' ? f.value : undefined,
                comparisonOperator: this.getOperator(f),
                children: this.getChildren(f)
            }));
    }
}

class ParamEncoder extends HttpUrlEncodingCodec {
    public override encodeKey(key: string): string {
        let encodedKey = super.encodeKey(key);

        if (encodedKey.indexOf('%5B') >= 0) {
            encodedKey = encodedKey.replace(/%5B/g, '[');
        }

        if (encodedKey.indexOf('%5D') >= 0) {
            encodedKey = encodedKey.replace(/%5D/g, ']');
        }

        return encodedKey;
    }
}

export interface FilterFieldVm {
    fieldName: string;
    label: string;
    type: 'text' | 'number' | 'date' | 'daterange' | 'select';
    value?: string | DateRange;
}