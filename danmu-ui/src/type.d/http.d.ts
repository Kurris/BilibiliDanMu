export interface IApiResult<T> {
    code: number,
    data: T,
    message: string
}