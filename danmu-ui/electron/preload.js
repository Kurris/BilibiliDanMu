const { ipcRenderer, contextBridge } = require('electron')

ipcRenderer.on('ignoreMouse', (event, bg) => {
  let body = document.getElementsByTagName('body')[0]
  console.log(body)
  body.style.backgroundColor = bg
})

contextBridge.exposeInMainWorld('electron', {
  ignoreMouse: () => {
    ipcRenderer.send('ignoreMouse')
  },
  sendMessageToBackend: (...args) => {
    ipcRenderer.send('backend-server', args)
  }
})
