import { app, BrowserWindow, ipcMain, Tray, Menu, nativeImage, globalShortcut } from "electron";
import path from "path";

import { spawn, exec, type ChildProcessWithoutNullStreams } from 'child_process'

let danmu: ChildProcessWithoutNullStreams;
console.log();


if (process.platform == 'darwin') {
  exec("lsof -i:5000 | grep -v PID | awk '{print $2}' | xargs kill -9")
}



ipcMain.on('ignoreMouse', (event) => {
  const win = BrowserWindow.fromWebContents(event.sender);
  win?.setFullScreen(true)
  win?.setIgnoreMouseEvents(true);
  win?.setAlwaysOnTop(true, 'pop-up-menu')
  win?.setSkipTaskbar(true)
})


let tray = null

const createWindow = () => {

  const win = new BrowserWindow({
    transparent: true,
    frame: false,
    // fullscreen: true,
    resizable: true,
    webPreferences: {
      contextIsolation: true, // 是否开启隔离上下文
      nodeIntegration: true, // 渲染进程使用Node API
      preload: path.join(__dirname, "../electron/preload.js"), // 需要引用js文件
    },
  });

  // win.webContents.openDevTools({ mode: 'undocked' })
  globalShortcut.register("CommandOrControl+Shift+i", () => {
    win.setIgnoreMouseEvents(false);
    win.setAlwaysOnTop(false, 'pop-up-menu')
    win.setSkipTaskbar(false);
    win.webContents.send('unIgnoreMouse')
  });

  // 如果打包了，渲染index.html
  if (app.isPackaged) {
    win.loadURL(`file://${path.join(__dirname, '../dist/index.html')}`)
  } else {
    win.loadURL('http://localhost:3000');
  }

  // win.setSkipTaskbar(true)


  tray = new Tray(nativeImage.createFromPath(path.join(__dirname, '../electron/setting.png')))
  const contextMenu = Menu.buildFromTemplate([
    // { label: 'Item1', type: 'checkbox' },
    // { label: 'Item2', type: 'checkbox' },
    {
      label: '取消覆盖', click: () => {
        // win.webContents.send('setting', 'setting')
        win.setIgnoreMouseEvents(false);
        win.setAlwaysOnTop(false, 'pop-up-menu')
        win.setSkipTaskbar(false);
        win.webContents.send('unIgnoreMouse')
      }
    },
    {
      label: '退出', click: () => {
        win.close()
        app.quit()
      }
    }
  ])
  tray.setToolTip('直播辅助工具')
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
  if (danmu.pid != null) {
    spawn('kill', [danmu!.pid.toString()])
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


