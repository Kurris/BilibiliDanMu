import { app, BrowserWindow, ipcMain, Tray, Menu, nativeImage, globalShortcut } from "electron";
import path from "path";
import { runSocketAndBackgroundService } from './backgroundService'


// 变量 ---------------------------------------------------------------------------------------------
let mainWindow: BrowserWindow;
let loadingWindow: BrowserWindow;
let overlayWindow: BrowserWindow;

const windowsIsTrueMacIsFalse = process.platform == 'win32'
const gotTheLock = app.requestSingleInstanceLock()


let tray: Tray;
let shortCutIgnoreMouse: boolean = false;



// ---------------------------------------------------------------------------------------------


// ipcMain.on('snedBackendServerMessage', (e, ...args) => {

//   const command: string = args[0]
//   const obj: object = args[1]
//   const needDisconnect: boolean = args[2]

//   ganmeDetectConnector.sendMessage(command, obj, needDisconnect).then(x => {
//     console.log("receive:" + x);
//   })
// })

// 程序开始前 ---------------------------------------------------------------------------------------------

// 苹果电脑杀掉当前端口进程
// if (!windowsIsTrueMacIsFalse) {
//   if (app.isPackaged) {
//     // exec("lsof -i:5000 | grep -v PID | awk '{print $2}' | xargs kill -9")
//   }
// }



// ---------------------------------------------------------------------------------------------------------



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

  }).then(() => {

    // globalShortcut.register("CommandOrControl+Shift+i", () => {
    //   if (cover) {
    //     shortCutIgnoreMouse = !shortCutIgnoreMouse;
    //     mainWindow.setIgnoreMouseEvents(shortCutIgnoreMouse);

    //     const bgColor = shortCutIgnoreMouse ? 'unset' : 'rgba(36,41,46,0.9)'
    //     mainWindow.webContents.send('ignoreMouse', bgColor)
    //   }
    // });



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
      preload: path.join(__dirname, "../electron/preload.js"), // 需要引用js文件
    },
  });


  mainWindow.once('ready-to-show', () => {
    //一定要先隐藏,不然会有视觉问题
    loadingWindow.hide()
    loadingWindow.close()
    mainWindow.show()
  })

  // 如果打包了，渲染index.html
  if (app.isPackaged) {
    mainWindow.loadURL(`file://${path.join(__dirname, '../dist/index.html')}`)
  } else {
    mainWindow.loadURL('http://localhost:3000');
  }


  tray = new Tray(nativeImage.createFromPath(path.join(__dirname, '../electron/app-24.png')))

  const contextMenu = Menu.buildFromTemplate([
    {
      label: '覆盖屏幕/窗口化', click: () => {

        // if (process.platform == 'darwin') {
        //   //苹果电脑setFullScreen会切换到另一个子屏幕
        //   overlayWindow.setSimpleFullScreen(!cover)
        // } else {
        //   overlayWindow.setFullScreen(!cover)
        //   overlayWindow.setSkipTaskbar(!cover);
        // }

        // overlayWindow.setIgnoreMouseEvents(!cover);
        // overlayWindow.setAlwaysOnTop(!cover, 'pop-up-menu')

        // cover = !cover
        // shortCutIgnoreMouse = !shortCutIgnoreMouse
      }
    },
    {
      label: '退出', click: () => {
        mainWindow.close()
        overlayWindow.hide()
        overlayWindow.close()
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

  // createCover()

  // mainWindow.webContents.openDevTools({ mode: 'undocked' })
  mainWindow.webContents.openDevTools({ mode: 'right' });
}


const createCover = () => {

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
      preload: path.join(__dirname, "../electron/preload.js"), // 需要引用js文件
    },
  });

  overlayWindow.setIgnoreMouseEvents(true)
  overlayWindow.setAlwaysOnTop(true, 'pop-up-menu')

  // 如果打包了，渲染index.html
  if (app.isPackaged) {
    overlayWindow.loadURL(`file://${path.join(__dirname, '../dist/index.html')}`)
  } else {
    overlayWindow.loadURL('http://localhost:3000/overlay');
  }
  shortCutIgnoreMouse = !shortCutIgnoreMouse;

  //     mainWindow.setIgnoreMouseEvents(shortCutIgnoreMouse);

  //     const bgColor = shortCutIgnoreMouse ? 'unset' : 'rgba(36,41,46,0.9)'
  //     mainWindow.webContents.send('ignoreMouse', bgColor)
  // overlayWindow.webContents.openDevTools({ mode: 'undocked' })

  globalShortcut.register("CommandOrControl+Shift+i", () => {

    shortCutIgnoreMouse = !shortCutIgnoreMouse;
    overlayWindow.setIgnoreMouseEvents(shortCutIgnoreMouse);

    const bgColor = shortCutIgnoreMouse ? 'transparent' : 'rgba(36,41,46,0.9)'
    overlayWindow.webContents.send('ignoreMouse', bgColor)

    console.log("ctrl+shift+i");
  });

}

// 关闭窗口
app.on("window-all-closed", () => {
  if (!windowsIsTrueMacIsFalse) {
    return
  }
  app.quit();
});


ipcMain.once('runService', () => {

  runSocketAndBackgroundService(6000, info => {
    if (mainWindow != null) {
      mainWindow.webContents.send(info.method, info)
    }
  })

})









