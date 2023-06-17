import { app, BrowserWindow, ipcMain, Tray, Menu, nativeImage, globalShortcut, desktopCapturer } from "electron";
import path from "path";
import { spawn, exec, type ChildProcessWithoutNullStreams } from 'child_process'


// 变量 ---------------------------------------------------------------------------------------------
let mainWindow: BrowserWindow;
let loadingWindow: BrowserWindow;
let overlayWindow: BrowserWindow;

const windowsIsTrueMacIsFalse = process.platform == 'win32'
let danmu: ChildProcessWithoutNullStreams;
let cover: boolean = false;
let tray: Tray;
let shortCutIgnoreMouse: boolean = false;
const gotTheLock = app.requestSingleInstanceLock()

// ---------------------------------------------------------------------------------------------



// 程序开始前 ---------------------------------------------------------------------------------------------

// 苹果电脑杀掉当前端口进程
if (process.platform == 'darwin') {
  if (app.isPackaged) {
    // exec("lsof -i:5000 | grep -v PID | awk '{print $2}' | xargs kill -9")
  }
}






// ---------------------------------------------------------------------------------------------------------



if (!gotTheLock) {
  app.quit()
} else {

  //尝试多开
  app.on('second-instance', (event, commandLine, workingDirectory) => {
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

    // runLiveServer()
    creatLoading();

    // mac系统
    app.on("activate", () => {
      if (BrowserWindow.getAllWindows().length === 0) creatLoading();
    });

  }).then(() => {

    globalShortcut.register("CommandOrControl+Shift+i", () => {
      if (cover) {
        shortCutIgnoreMouse = !shortCutIgnoreMouse;
        mainWindow.setIgnoreMouseEvents(shortCutIgnoreMouse);

        const bgColor = shortCutIgnoreMouse ? 'unset' : 'rgba(36,41,46,0.9)'
        mainWindow.webContents.send('ignoreMouse', bgColor)
      }
    });


    desktopCapturer.getSources({ types: ['window'] }).then(async sources => {
      for (const source of sources) {
        console.log(source);

      }
    })
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

  // 由loadingWindow打开mainWindow
  loadingWindow.once("show", createWindow);
  // 先加载文件再展示
  loadingWindow.loadFile(path.join(__dirname, '../pacman-loading/index.html')).then(() => {
    loadingWindow.show()
  }).catch(() => {
    app.quit()
  });
}

const createWindow = () => {

  mainWindow = new BrowserWindow({
    show: false, //由loadingWindow展示窗体
    transparent: true,
    fullscreen: windowsIsTrueMacIsFalse,
    frame: false,
    titleBarStyle: "hidden",//customButtonsOnHover 可以用html自定义缩小,放大,关闭按钮(可以,但是没必要,会缺少平台特定功能)
    titleBarOverlay: {
      color: '#ffffff',
      // symbolColor 符号颜色
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

  // mainWindow.webContents.openDevTools({ mode: 'undocked' })
  // win.webContents.openDevTools({ mode: 'right' });

  setTimeout(() => {
    // 如果打包了，渲染index.html
    if (app.isPackaged) {
      mainWindow.loadURL(`file://${path.join(__dirname, '../dist/index.html')}`)
    } else {
      mainWindow.loadURL('http://localhost:3000');
    }
  }, 3000)


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
        shortCutIgnoreMouse = !shortCutIgnoreMouse
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
}


const createCover = () => {
  coverWindow = new BrowserWindow({
    show: false,
    transparent: true,
    fullscreen: true,
    frame: false,
    resizable: false,
    webPreferences: {
      contextIsolation: true, // 是否开启隔离上下文
      nodeIntegration: true, // 渲染进程使用Node API
      preload: path.join(__dirname, "../electron/preload.js"), // 需要引用js文件
    },
  });

  globalShortcut.register("CommandOrControl+Shift+i", () => {
    if (cover) {
      shortCutIgnoreMouse = !shortCutIgnoreMouse;
      overlayWindow.setIgnoreMouseEvents(shortCutIgnoreMouse);

      const bgColor = shortCutIgnoreMouse ? 'unset' : 'rgba(36,41,46,0.9)'
      overlayWindow.webContents.send('ignoreMouse', bgColor)
    }
  });

  // 如果打包了，渲染index.html
  if (app.isPackaged) {
    overlayWindow.loadURL(`file://${path.join(__dirname, '../dist/index.html')}`)
  } else {
    overlayWindow.loadURL('http://localhost:3000');
  }
}

// 关闭窗口
app.on("window-all-closed", () => {
  if (process.platform == 'darwin') {
    if (danmu?.pid != null) {
      spawn('kill', [danmu!.pid.toString()])
    }
  }
  app.quit();
});




// ---

const runLiveServer = () => {

  const targetPath = app.isPackaged ? path.join(process.cwd(), '/resources/danmu-exe/') : path.join(__dirname, '../danmu-exe/')

  danmu = spawn('dotnet', ['LiveServer.dll', '--urls', 'http://*:5000'], {
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


