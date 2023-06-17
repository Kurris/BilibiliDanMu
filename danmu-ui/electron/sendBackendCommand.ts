export default class sendBackendCommand {

    private _method: string;
    private _obj: object;
    private _longReceive: boolean;

    constructor(method: string, obj: object, longReceive: boolean) {
        this._method = method
        this._obj = obj
        this._longReceive = longReceive
    }


    generate() {
        return `<command>|${this._method}|<obj>|${JSON.stringify(this._obj)}|<long>|${this._longReceive ? 1 : 0}`
    }
}
