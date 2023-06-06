const { ipcRenderer, contextBridge } = require('electron')

contextBridge.exposeInMainWorld('api', {
  ignoreMouse: () => {
    ipcRenderer.send('ignoreMouse')
  }
})
