import net from 'net'
import sendBackendCommand from './sendBackendCommand'

class BackendConnector {

    private _port: number
    private _socket: net.Socket

    constructor(port: number) {
        this._socket = new net.Socket()
        this._port = port
        this.init()
    }

    private init() {
        this._socket.connect({ port: this._port, family: 4 }, () => {
            console.log(`Connected to backend in port :${this._port}`);
        });

        this._socket.on('close', () => {
            console.log('Client closed');
        });

        this._socket.on('error', (err) => {
            console.log(err);
        });
    }


    sendMessage = (command: string, obj: object, longReceive: boolean) => {

        const message = new sendBackendCommand(command, obj, longReceive).generate()

        return new Promise<string>((resolve, reject) => {


            this._socket.once('data', data => {
                resolve(data.toString('ascii'))

                if (data.toString().endsWith("<exit>")) {
                    this._socket.destroy()
                }
            })

            this._socket.once('error', (err) => {
                reject(err)
            });

            this._socket.write(Buffer.from(message))
        })
    }


    getClient = () => { return this._socket }
}


export default BackendConnector;