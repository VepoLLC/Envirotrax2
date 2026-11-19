/**
 * Runs the given promises in parallel and waits until every one of them has finished. Use it instead of `Promise.all`.
 *
 * `Promise.all` rejects as soon as one promise fails, without waiting for the others. A `finally` block then hides the
 * loading spinner while the remaining requests are still running, and a save can start before all the data has loaded.
 *
 * This function waits with `Promise.allSettled`, so it never returns early. Once all the promises have finished, it
 * rethrows the first failure, so a `try/catch` around it and the global error handler still see the error. A bare
 * `Promise.allSettled` would swallow it. When nothing fails, it returns the values in order, just like `Promise.all`.
 */
export function allSettled<T extends readonly unknown[] | []>(values: T): Promise<{ -readonly [P in keyof T]: Awaited<T[P]> }>;
export function allSettled<T>(values: Iterable<T | PromiseLike<T>>): Promise<Awaited<T>[]>;
export async function allSettled(values: Iterable<unknown>): Promise<unknown[]> {
    const results = await Promise.allSettled(values);
    const failure = results.find((result): result is PromiseRejectedResult => result.status === 'rejected');

    if (failure) {
        throw failure.reason;
    }

    return results.map(result => (result as PromiseFulfilledResult<unknown>).value);
}
