// eslint-disable-next-line no-undef
const { ipcRenderer, contextBridge } = require('electron')

contextBridge.exposeInMainWorld('api', {
  ignoreMouse: () => {
    ipcRenderer.send('ignoreMouse')
  },
  ligy: 'yes'
})
