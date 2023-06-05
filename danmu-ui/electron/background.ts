import { app, BrowserWindow, ipcMain, Tray, Menu, nativeImage } from "electron";
import path from "path";

import iconvLite from 'iconv-lite';
import { spawn, type ChildProcessWithoutNullStreams } from 'child_process'

let danmu: ChildProcessWithoutNullStreams;

ipcMain.on('set-ignore-mouse-events', (event, ...args) => {
  BrowserWindow.fromWebContents(event!.sender)!.setIgnoreMouseEvents(...args)
})


ipcMain.on('ignoreMouse', (event, args) => {
  const win = BrowserWindow.fromWebContents(event.sender);
  if (win != null) {
    // win.setFullScreen(true);
    // win.setIgnoreMouseEvents(true);
    // win.setAlwaysOnTop(true, 'pop-up-menu')
  }
})


let tray = null

//const isDevelopment = process.env.NODE_ENV !== 'production'
const createWindow = () => {

  const win = new BrowserWindow({
    transparent: true,
    frame: false,
    // fullscreen: true,
    webPreferences: {
      contextIsolation: false, // 是否开启隔离上下文
      nodeIntegration: true, // 渲染进程使用Node API
      preload: path.join(__dirname, "../electron/preload.js"), // 需要引用js文件
    },
  });

  // win.webContents.openDevTools({ mode: 'undocked' })
  // globalShortcut.register("CommandOrControl+Shift+i", function () {
  //   win.webContents.openDevTools();
  // });

  // 如果打包了，渲染index.html
  if (app.isPackaged) {
    win.loadURL(`file://${path.join(__dirname, '../dist/index.html')}`)
  } else {
    win.loadURL('http://localhost:3000');
  }

  // win.setSkipTaskbar(true)


  tray = new Tray(nativeImage.createFromPath(path.join(__dirname, '../electron/setting.png')))
  const contextMenu = Menu.buildFromTemplate([
    { label: 'Item1', type: 'checkbox' },
    { label: 'Item2', type: 'checkbox' },
    { label: 'Item3', type: 'checkbox', checked: true },
    {
      label: '退出', click: () => {
        win.close()
        app.quit()
      }
    }
  ])
  tray.setToolTip('B站直播辅助工具')
  tray.setContextMenu(contextMenu)
  tray.on('click', () => {
    win.show()
  })


};


app.whenReady().then(() => {
  runExec()
  createWindow();
  app.on("activate", () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});



// 关闭窗口
app.on("window-all-closed", () => {
  if (process.platform !== "darwin") {
    app.quit();
  }
});




const runExec = () => {

  return

  if (app.isPackaged) {
    danmu = spawn(path.join(process.cwd(), '/resources/danmu-exe/DanMuServer.exe'), {});
  } else {
    danmu = spawn(path.join(__dirname, "../danmu-exe/DanMuServer.exe"), {});
  }

  // 输出相关的数据
  danmu.stdout.on('data', (data) => {
    try {
      console.log('data from child: ' + iconvLite.decode(data, 'cp936'));
    } catch (error) {
      console.log(error)
    }
  });

  // 错误的输出
  danmu.stderr.on('data', (data) => {

    try {
      console.log('error from child: ' + iconvLite.decode(data, 'cp936'));
    } catch (error) {
      console.log(error)
    }
  });

  // 子进程结束时输出
  danmu.on('close', (code) => {

    try {
      console.log('child exists with code: ' + iconvLite.decode(code, 'cp936'));
    } catch (error) {
      console.log(error)
    }
  });
}
