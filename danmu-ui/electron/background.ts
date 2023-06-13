import { app, BrowserWindow, ipcMain, Tray, Menu, nativeImage, globalShortcut, type Rectangle } from "electron";
import path from "path";
import { spawn, exec, type ChildProcessWithoutNullStreams } from 'child_process'

let danmu: ChildProcessWithoutNullStreams;
let rectangle: Rectangle;
let cover: boolean = false;
let mainWindow: BrowserWindow;
let loading: BrowserWindow;
let tray: Tray;
let shortCutIgnoreMouse: boolean = false;


// 苹果电脑杀掉当前端口进程
if (process.platform == 'darwin') {
  if (app.isPackaged) {
    // exec("lsof -i:5000 | grep -v PID | awk '{print $2}' | xargs kill -9")
  }
}

const gotTheLock = app.requestSingleInstanceLock()
if (!gotTheLock) {
  app.quit()
} else {
  app.on('second-instance', (event, commandLine, workingDirectory) => {
    // 用户正在尝试运行第二个实例，我们需要让焦点指向我们的窗口
    if (mainWindow) {
      if (mainWindow.isMinimized()) mainWindow.restore()
      mainWindow.focus()
    }
  })

  app.whenReady().then(() => {

    if (process.platform == 'darwin') {
      app.dock.setIcon(path.join(__dirname, '../electron/app-100.png'))
    }
  }).then(() => {
    // runExec()
    creatLoading();
    app.on("activate", () => {
      if (BrowserWindow.getAllWindows().length === 0) creatLoading();
    });
  })

}


const creatLoading = () => {
  loading = new BrowserWindow({
    show: false,
    frame: false, // 无边框（窗口、工具栏等），只包含网页内容
    width: 550,
    height: 350,
    resizable: false,
    transparent: true, // 窗口是否支持透明，如果想做高级效果最好为true
  });

  loading.once("show", createWindow);
  loading.loadFile(path.join(__dirname, '../pacman-loading/index.html'));
  loading.show()
}

const createWindow = () => {

  mainWindow = new BrowserWindow({
    show: false,
    // transparent: true,
    frame: false,
    titleBarStyle: "hidden",//customButtonsOnHover 可以自定义缩小,放大,关闭按钮
    titleBarOverlay: {
      color: '#ffffff',
      // symbolColor 符号颜色
      height: 5
    }, // 需要设置titleBarStyle才生效, mac设置:true , windows使用该对象即可
    resizable: true,
    height: 850,
    width: 1380,
    title: '直播辅助工具',
    webPreferences: {
      contextIsolation: true, // 是否开启隔离上下文
      nodeIntegration: true, // 渲染进程使用Node API
      preload: path.join(__dirname, "../electron/preload.js"), // 需要引用js文件
    },
  });


  mainWindow.once('ready-to-show', () => {
    loading.hide()
    loading.close()
    mainWindow.show()
  })

  // mainWindow.webContents.openDevTools({ mode: 'undocked' })
  // win.webContents.openDevTools({ mode: 'right' });

  globalShortcut.register("CommandOrControl+Shift+i", () => {
    mainWindow.setIgnoreMouseEvents(!shortCutIgnoreMouse);
  });

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

        if (process.platform == 'darwin') {
          //苹果电脑setFullScreen会切换到另一个子屏幕
          mainWindow.setSimpleFullScreen(!cover)
        } else {
          mainWindow.setFullScreen(!cover)
          mainWindow.setSkipTaskbar(!cover);
        }

        mainWindow.setIgnoreMouseEvents(!cover);
        mainWindow.setAlwaysOnTop(!cover, 'pop-up-menu')

        cover = !cover
      }
    },
    {
      label: '退出', click: () => {
        mainWindow.close()
        app.quit()
      }
    }
  ])

  tray.setToolTip('直播辅助工具')
  tray.setContextMenu(contextMenu)
  tray.on('click', () => {
    mainWindow?.setSkipTaskbar(false)
    mainWindow.show()
  })


};

ipcMain.on("setMini", (event) => {
  const win = BrowserWindow.fromWebContents(event.sender);
  win?.minimize()
})

ipcMain.on("restoreSize", (event) => {
  const win = BrowserWindow.fromWebContents(event.sender);
  if (win?.isMaximized()) {
    win?.setContentBounds(rectangle, true)
  }
})


ipcMain.on("setMax", (event) => {
  const win = BrowserWindow.fromWebContents(event.sender);
  if (win?.isMaximized()) {
    win?.setContentBounds(rectangle, true)
  } else {
    rectangle = win!.getContentBounds()
    win?.maximize()
  }
})

ipcMain.on("setToTray", (event) => {
  const win = BrowserWindow.fromWebContents(event.sender);
  win?.minimize()
  win?.setSkipTaskbar(true)
})


ipcMain.on("dragTitle", (event, position) => {
  const win = BrowserWindow.fromWebContents(event.sender);
  win?.setPosition(position.x, position.y)
})


// 关闭窗口
app.on("window-all-closed", () => {
  if (process.platform == 'darwin') {
    if (danmu?.pid != null) {
      spawn('kill', [danmu!.pid.toString()])
    }
  }
  app.quit();
});


const runExec = () => {

  const targetPath = app.isPackaged ? path.join(process.cwd(), '/resources/danmu-exe/') : path.join(__dirname, '../danmu-exe/')

  danmu = spawn('dotnet', ['DanMuServer.dll', '--urls', 'http://*:5000'], {
    cwd: targetPath
  });

  // 输出相关的数据
  danmu.stdout.on('data', (data) => {
    try {
      console.log('server: ' + data);
    } catch (error) {
      console.log(error)
    }
  });

  // 错误的输出
  danmu.stderr.on('data', (data) => {

    try {
      console.log('server: ' + data);
    } catch (error) {
      console.log(error)
    }
  });

  // 子进程结束时输出
  danmu.on('close', (data) => {
    try {
      console.log('server: ' + data);
    } catch (error) {
      console.log(error)
    }
  });
}


