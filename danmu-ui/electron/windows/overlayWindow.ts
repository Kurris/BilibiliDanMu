import { BrowserWindow } from "electron";
import path from "path";
import { overlayWindowUrl } from '../consts'


let overlayWindow: BrowserWindow;
const createOverlayWindow = (callback: () => void) => {

    overlayWindow = new BrowserWindow({
        show: true,
        transparent: true,
        fullscreen: true,
        frame: false,
        resizable: false,
        skipTaskbar: true,
        webPreferences: {
            contextIsolation: true, // 是否开启隔离上下文
            nodeIntegration: true, // 渲染进程使用Node API
            preload: path.join(__dirname, "../electron/overlay-preload.js"), // 需要引用js文件
        },
    });

    overlayWindow.setIgnoreMouseEvents(true)
    overlayWindow.setAlwaysOnTop(true, 'pop-up-menu')

    overlayWindow.loadURL(overlayWindowUrl).then(() => {
        callback()
    });
}


export { createOverlayWindow, overlayWindow }
