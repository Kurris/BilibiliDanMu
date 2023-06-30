import path from "path";
import { app, BrowserWindow, ipcMain, globalShortcut } from "electron";
import { runSocketAndBackgroundService, liveBackend } from './backgroundService'
import { createLoadingWindow, loadingWindow } from './windows/loadingWindow'
import { createOverlayWindow, overlayWindow } from './windows/overlayWindow'
import { gotTheLock, mainWindowUrl, windowsIsTrueMacIsFalse, isDevelopment } from './consts'
import createTray from "./components/appTray";


let mainWindow: BrowserWindow;
let isCloseByTray = false
let isOverlayIgnoreMouse: boolean = true;
let isManualSetCover: boolean = false;
let isOverlayWindowsReady = false

// ---------------------------------------------------------------------------------------------


if (isDevelopment)
  app.commandLine.appendSwitch('ignore-certificate-errors')


if (!gotTheLock) {
  app.quit()
} else {

  //尝试多开  回调参数 event, commandLine, workingDirectory
  app.on('second-instance', () => {
    //用户正在尝试运行第二个实例，我们需要让焦点指向我们的窗口
    if (mainWindow) {
      if (mainWindow.isMinimized()) mainWindow.restore()
      mainWindow.focus()
    }
  })

  app.whenReady().then(() => {

    if (process.platform == 'darwin') {
      // mac特定api
      app.dock.setIcon(path.join(__dirname, '../electron/app-100.png'))
    }

  }).then(() => {

    createLoadingWindow(createMainWindow);

    // mac系统处理
    app.on("activate", () => {
      if (BrowserWindow.getAllWindows().length === 0) {
        createLoadingWindow(createMainWindow);
      }
    });
  })
}


const createMainWindow = () => {

  mainWindow = new BrowserWindow({
    show: false, //由loadingWindow展示窗体
    frame: false,
    titleBarStyle: "hidden",//customButtonsOnHover 可以用html自定义缩小,放大,关闭按钮(可以,但是没必要,会缺少平台特定功能)
    titleBarOverlay: {
      color: 'rgba(2, 3, 4, 0)',
      // symbolColor: "rgba(36,41,46,0.9)"
      // height: 5
    }, // 需要设置titleBarStyle才生效, mac上设置:true , windows使用该对象不为undefined即可
    resizable: true,
    height: 850,
    width: 1380,
    title: 'Live chat',
    webPreferences: {
      devTools: isDevelopment,
      contextIsolation: true, // 是否开启隔离上下文
      nodeIntegration: true, // 渲染进程使用Node API
      preload: path.join(__dirname, "../electron/main-preload.js"), // 需要引用js文件
    },
  });


  mainWindow.once('ready-to-show', () => {
    //一定要先隐藏,不然会有视觉问题
    loadingWindow.hide()
    loadingWindow.close()
    mainWindow.show()
  })

  mainWindow.on('close', (e) => {
    if (!isCloseByTray) {
      e.preventDefault()
      mainWindow.hide()
    } else {

      if (liveBackend != null) {
        liveBackend.kill()
      }

      if (overlayWindow != null) {
        overlayWindow.hide()
        overlayWindow.close()
      }
    }
  })


  mainWindow.loadURL(mainWindowUrl).then(() => {

    createTray(() => {
      mainWindow?.setSkipTaskbar(false)
      mainWindow.show()
    }, () => {
      isManualSetCover = !isManualSetCover
    }, () => {
      isCloseByTray = true
      app.quit()
    })

  });
}



// 关闭窗口
app.on("window-all-closed", () => {
  if (!windowsIsTrueMacIsFalse) {
    return
  }
  app.quit();
});




// ipcMain listen event ----------------------------------------------------------------------------

// 确保只运行一次
ipcMain.once('runService', () => {

  runSocketAndBackgroundService(info => {

    if (info.method != "GameIsForeground") {
      if (mainWindow != null) {
        mainWindow.webContents.send(info.method, info)
      }
    } else {
      if (overlayWindow != null) {

        //手动设置覆盖,则始终为true
        if (isManualSetCover) {
          overlayWindow.webContents.send(info.method, {
            isForeground: true
          })
        } else {
          overlayWindow.webContents.send(info.method, info)
        }
      }
    }
  })
})



ipcMain.on('overlay', (e, info) => {

  createOverlayWindow(() => {
    globalShortcut.register("CommandOrControl+Shift+G", () => {

      //未加载完成前快捷键不可触发
      if (!isOverlayWindowsReady) return

      isOverlayIgnoreMouse = !isOverlayIgnoreMouse;
      overlayWindow.setIgnoreMouseEvents(isOverlayIgnoreMouse);
      overlayWindow.webContents.send('ignoreMouse')

    });

    console.debug('send streamer info to overlay')
    overlayWindow.webContents.send('receiveStreamerInfo', info)
  })
})


ipcMain.on("overlay-isReady", () => {
  console.debug('overlay isReady');
  isOverlayWindowsReady = true
})







