import { app, BrowserWindow, ipcMain, Tray, Menu, nativeImage, globalShortcut } from "electron";
import path from "path";
import { runSocketAndBackgroundService } from './backgroundService'
import { type ChildProcessWithoutNullStreams } from 'child_process'


// 变量 ---------------------------------------------------------------------------------------------
let mainWindow: BrowserWindow;
let loadingWindow: BrowserWindow;
let overlayWindow: BrowserWindow;
const overlayWindowUrl = app.isPackaged ? 'http://isawesome.cn:8080/overlay' : 'http://localhost:3000/overlay'
// overlayWindow.loadURL(`file://${path.join(__dirname, '../dist/index.html')}`)

const windowsIsTrueMacIsFalse = process.platform == 'win32'
const gotTheLock = app.requestSingleInstanceLock()


let tray: Tray;
let isOverlayIgnoreMouse: boolean = true;
let isManualSetCover: boolean = false;

let closeByTray = false
let liveBackend: ChildProcessWithoutNullStreams;

let overlayWindowsIsReady = false
// ---------------------------------------------------------------------------------------------



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

    creatLoading();
    // mac系统
    app.on("activate", () => {
      if (BrowserWindow.getAllWindows().length === 0) creatLoading();
    });

  })
}


const creatLoading = () => {
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
    createWindow()

  }).catch(() => {
    app.quit()
  });
}

const createWindow = () => {

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
    if (!closeByTray) {
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

  // 如果打包了，渲染index.html
  if (app.isPackaged) {
    // mainWindow.loadURL(`file://${path.join(__dirname, '../dist/index.html')}`)
    mainWindow.loadURL('http://isawesome.cn:8080');
  } else {
    mainWindow.loadURL('http://localhost:3000');
  }


  tray = new Tray(nativeImage.createFromPath(path.join(__dirname, '../electron/app-24.png')))

  const contextMenu = Menu.buildFromTemplate([
    {
      label: '始终覆盖/取消', click: () => {
        isManualSetCover = !isManualSetCover
        // if (process.platform == 'darwin') {
        //   //苹果电脑setFullScreen会切换到另一个子屏幕
        //   overlayWindow.setSimpleFullScreen(!cover)
        // } else {
        //   overlayWindow.setFullScreen(!cover)
        //   overlayWindow.setSkipTaskbar(!cover);
        // }
      }
    },
    {
      label: '退出', click: () => {
        closeByTray = true
        app.quit()
      }
    }
  ])

  tray.setToolTip('live chat')
  tray.setContextMenu(contextMenu)
  tray.on('click', () => {
    mainWindow?.setSkipTaskbar(false)
    mainWindow.show()
  })


  // mainWindow.webContents.openDevTools({ mode: 'right' });
}

const createCover = () => {

  if (overlayWindow != null) {
    return
  }

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

  overlayWindow.setIgnoreMouseEvents(isOverlayIgnoreMouse)
  overlayWindow.setAlwaysOnTop(true, 'pop-up-menu')

  overlayWindow.loadURL(overlayWindowUrl);

  globalShortcut.register("CommandOrControl+Shift+g", () => {

    if (!overlayWindowsIsReady) return

    isOverlayIgnoreMouse = !isOverlayIgnoreMouse;
    overlayWindow.setIgnoreMouseEvents(isOverlayIgnoreMouse);
    overlayWindow.webContents.send('ignoreMouse')
  });
  // overlayWindow.webContents.openDevTools({ mode: 'undocked' })
}

// 关闭窗口
app.on("window-all-closed", () => {
  if (!windowsIsTrueMacIsFalse) {
    return
  }
  app.quit();
});


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
  }).then(p => liveBackend = p)
})


ipcMain.on('overlay', (e, info) => {
  createCover()
  if (overlayWindow != null) {
    overlayWindow.webContents.send('receiveStreamerInfo', info)
  }
})

ipcMain.on("overlay-isReady", () => {
  overlayWindowsIsReady = true
})







