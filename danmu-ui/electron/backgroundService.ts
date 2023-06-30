import net from 'net'
import path from "path";
import { app } from "electron";
import { spawn, exec, type ChildProcessWithoutNullStreams } from 'child_process'


let liveBackend: ChildProcessWithoutNullStreams

const runSocketAndBackgroundService = async (callback: (obj: any) => void) => {
    let port = 6000

    let canBeUse = await judgePortCanUse(port);
    while (!canBeUse) {
        port++
        canBeUse = await judgePortCanUse(port);
    }

    const socketServer = net.createServer()
    socketServer.on('connection', (client) => {

        console.log('client connected');

        client.on('data', (data: Buffer) => {
            try {
                const info = JSON.parse(data.toString())
                callback(info)
            } catch (error) {
                // 看起来不会解析错误 {"method":"GameIsForeground","isForeground":false}
                // 先catch解决
                // console.log('json parse:' + data.toString());
            }
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
        runBackgroundService(port, true, false)
    })
}


const runBackgroundService = (port: number, detectGame: boolean, detectMusice: boolean) => {

    const targetPath = app.isPackaged ? path.join(process.cwd(), '/resources/danmu-exe/') : path.join(__dirname, '../danmu-exe/')

    liveBackend = spawn('dotnet', ['LiveBackgroundService.dll', port.toString(), detectGame + "", detectMusice + ""], {
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


const judgePortCanUse = (port: number) => {
    const command = `netstat -ano|findstr "${port}"`;
    return new Promise<boolean>((resolve) => {
        exec(command, (error: any, stdout: string) => {
            resolve(stdout === "");
        });
    })
}


export { runSocketAndBackgroundService, liveBackend }
