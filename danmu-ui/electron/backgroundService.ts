import net from 'net'
import path from "path";
import { app } from "electron";
import { spawn, exec, type ChildProcessWithoutNullStreams } from 'child_process'

let liveBackend: ChildProcessWithoutNullStreams;

export const runSocketAndBackgroundService = async (callback: (obj: any) => void) => {

    let port = 6000

    let canBeUse = await judgePorCanUse(port);
    while (!canBeUse) {
        port++
        canBeUse = await judgePorCanUse(port);
    }


    const socketServer = net.createServer()
    socketServer.on('connection', (client) => {

        console.log('client connected');

        client.on('data', (data: Buffer) => {

            const info = JSON.parse(data.toString())

            callback(info)

        })

        client.on('close', () => {
            console.log('client closed');
        })

        client.on('error', (error) => {
            console.log(error);
        })
    })

    socketServer.listen(port, () => {
        console.log('server listering on ' + port);
        runBackgroundService(port)
    })
}


const runBackgroundService = (port: number) => {

    const targetPath = app.isPackaged ? path.join(process.cwd(), '/resources/danmu-exe/') : path.join(__dirname, '../danmu-exe/')

    liveBackend = spawn('dotnet', ['LiveBackgroundService.dll', port.toString()], {
        cwd: targetPath
    });


    // 输出相关的数据
    liveBackend.stdout.on('data', (data: Buffer) => {
        try {
            console.log(data.toString());
        } catch (error) {
            console.log(error)
        }
    });
}


const judgePorCanUse = (port: number) => {
    const command = `netstat -ano|findstr "${port}"`;
    return new Promise<boolean>((resolve) => {
        exec(command, (error: any, stdout: string, stderr: string) => {
            resolve(stdout === "");
        });
    })
}
