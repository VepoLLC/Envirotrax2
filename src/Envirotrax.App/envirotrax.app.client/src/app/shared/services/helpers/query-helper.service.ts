import { Injectable } from "@angular/core";
import { PageInfo } from "../../models/page-info";
import { HttpParams, HttpUrlEncodingCodec } from "@angular/common/http";
import { Query } from "../../models/query";

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

    public queryToQueryString(query: Query, params?: HttpParams): HttpParams {
        params = params || new HttpParams({
            encoder: new ParamEncoder()
        });

        if (query.sort) {
            for (let key in query.sort) {
                params = params.append(`s[${key}]`, query.sort[key]);
            }
        }

        if (query.filter) {
            for (let i = 0; i < query.filter.length; i++) {
                params = params.append(`q[${i}].col`, query.filter[i].columnName || '');

                if (this.isDefined(query.filter[i].value)) {
                    params = params.append(`q[${i}].val`, query.filter[i].value!.toString());
                }

                if (this.isDefined(query.filter[i].logicalOperator)) {
                    params = params.append(`q[${i}].lop`, query.filter[i].logicalOperator!);
                }

                if (this.isDefined(query.filter[i].comparisonOperator)) {
                    params = params.append(`q[${i}].op`, query.filter[i].comparisonOperator!);
                }

                if (this.isDefined(query.filter[i].isValueNull)) {
                    params = params.append(`q[${i}].null`, query.filter[i].isValueNull!);
                }
            }
        }

        return params;
    }

    public buildQuery(pageInfo: PageInfo, query: Query): HttpParams {
        let params = this.pageInfoToQueryString(pageInfo);

        params = this.queryToQueryString(query, params);

        return params;
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