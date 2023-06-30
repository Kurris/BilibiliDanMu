import path from "path";
import { app, BrowserWindow } from "electron";

let loadingWindow: BrowserWindow
const createLoadingWindow = (callback: () => void) => {
    loadingWindow = new BrowserWindow({
        show: false, //加载完本地html再展示
        frame: false,
        width: 550,
        height: 350,
        resizable: false,
        transparent: true,
    });

    // 先加载文件再展示
    loadingWindow.loadFile(path.join(__dirname, '../pacman-loading/index.html')).then(() => {
        loadingWindow.show()
        callback()

    }).catch(() => {
        app.quit()
    });
}

export { createLoadingWindow, loadingWindow }