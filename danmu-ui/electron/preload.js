// eslint-disable-next-line no-undef
const { ipcRenderer, contextBridge } = require('electron')

ipcRenderer.on('unIgnoreMouse', () => {
  console.log('unIgnoreMouse')
  let unIgnoreMouse = document.getElementById('unIgnoreMouse')
  unIgnoreMouse?.click()
})

contextBridge.exposeInMainWorld('api', {
  ignoreMouse: () => {
    ipcRenderer.send('ignoreMouse')
  },
  ligy: 'yes'
})
