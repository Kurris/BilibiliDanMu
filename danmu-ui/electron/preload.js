// eslint-disable-next-line no-undef
const { ipcRenderer, contextBridge } = require('electron')

contextBridge.exposeInMainWorld('api', {
  ignoreMouse: () => {
    ipcRenderer.send('ignoreMouse')
  },
  setMini: () => {
    ipcRenderer.send('setMini')
  },
  setMax: () => {
    ipcRenderer.send('setMax')
  },
  setToTray: () => {
    ipcRenderer.send('setToTray')
  },
  dragTitle: (position) => {
    ipcRenderer.send('dragTitle', position)
  },
  restoreSize: () => {
    ipcRenderer.send('restoreSize')
  }
})
