import { app } from "electron";

const appName = 'overlay'

const gotTheLock = app.requestSingleInstanceLock()

const isDevelopment = !app.isPackaged;
const isProduction = app.isPackaged;
const windowsIsTrueMacIsFalse = process.platform == 'win32'

// overlayWindow.loadURL(`file://${path.join(__dirname, '../dist/index.html')}`)
const mainWindowUrl = app.isPackaged ? 'http://isawesome.cn:8080' : 'http://localhost:3000'
const overlayWindowUrl = app.isPackaged ? 'http://isawesome.cn:8080/overlay' : 'http://localhost:3000/overlay'




export {
    appName,
    gotTheLock,
    windowsIsTrueMacIsFalse,
    isDevelopment,
    isProduction,
    mainWindowUrl,
    overlayWindowUrl
}