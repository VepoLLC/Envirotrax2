import { Query } from '@envirotrax/common-ui';

// A user details window lists that user's licenses through the company's licenses endpoint. The filter goes on
// the request rather than on the table's query, so the table's own filters, sorting and reset are left alone.
export function withLicenseUserFilter(query: Query, userId?: number): Query {
    if (!userId) {
        return query;
    }

    return {
        ...query,
        filter: [
            ...(query.filter ?? []),
            { columnName: 'user.id', comparisonOperator: 'Eq', value: String(userId) }
        ]
    };
}
