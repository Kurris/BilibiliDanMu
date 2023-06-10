import { app, BrowserWindow, ipcMain, Tray, Menu, nativeImage, globalShortcut, type Rectangle } from "electron";
import path from "path";
import { spawn, exec, type ChildProcessWithoutNullStreams } from 'child_process'

let danmu: ChildProcessWithoutNullStreams;
let rectangle: Rectangle;
let cover: boolean = false;
let mainWindow: BrowserWindow;
let loading: BrowserWindow;
let tray: Tray;


// 苹果电脑杀掉当前端口进程
if (process.platform == 'darwin') {
  exec("lsof -i:5000 | grep -v PID | awk '{print $2}' | xargs kill -9")
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
    runExec()
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
    transparent: true,
    frame: false,
    resizable: true,
    height: 750,
    width: 1280,
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

  // win.webContents.openDevTools({ mode: 'undocked' })
  // win.webContents.openDevTools({ mode: 'right' });

  globalShortcut.register("CommandOrControl+Shift+i", () => {
    mainWindow.setIgnoreMouseEvents(false);
    mainWindow.setAlwaysOnTop(false, 'pop-up-menu')
    mainWindow.setSkipTaskbar(false);
  });

  setTimeout(() => {
    // 如果打包了，渲染index.html
    if (app.isPackaged) {
      mainWindow.loadURL(`file://${path.join(__dirname, '../dist/index.html')}`)
    } else {
      mainWindow.loadURL('http://localhost:3000');
    }
  }, 3000);


  tray = new Tray(nativeImage.createFromPath(path.join(__dirname, '../electron/setting.png')))
  const contextMenu = Menu.buildFromTemplate([
    // { label: 'Item1', type: 'checkbox' },
    // { label: 'Item2', type: 'checkbox' },
    {
      label: '覆盖屏幕/窗口化', click: () => {
        mainWindow.setFullScreen(!cover)
        mainWindow.setIgnoreMouseEvents(!cover);
        mainWindow.setAlwaysOnTop(!cover, 'pop-up-menu')
        mainWindow.setSkipTaskbar(!cover);
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


ipcMain.on('ignoreMouse', (event) => {
  const win = BrowserWindow.fromWebContents(event.sender);
  win?.setFullScreen(true)
  win?.setIgnoreMouseEvents(true);
  win?.setAlwaysOnTop(true, 'pop-up-menu')
  win?.setSkipTaskbar(true)
})

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
    if (danmu.pid != null) {
      spawn('kill', [danmu!.pid.toString()])
    }
  }
  app.quit();
});


const runExec = () => {
  return
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


