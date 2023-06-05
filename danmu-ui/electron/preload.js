const { ipcRenderer, contextBridge } = require('electron')

ipcRenderer.send('test', 'testing')
contextBridge.exposeInMainWorld('api', {
  ignoreMouse: () => {
    ipcRenderer.send('ignoreMouse')
  }
})

// window.addEventListener('DOMContentLoaded', () => {
//   const el = document.getElementById('setting')
//   el.addEventListener('mouseenter', () => {
//     ipcRenderer.send('set-ignore-mouse-events', false)
//   })
//   el.addEventListener('mouseleave', () => {
//     ipcRenderer.send('set-ignore-mouse-events', true, { forward: true })
//   })
// })
