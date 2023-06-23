// eslint-disable-next-line no-undef
const { ipcRenderer, contextBridge } = require('electron')

let isForeground = false
let streamerInfo = {}

ipcRenderer.on('ignoreMouse', () => {
  let styleSetting = document.getElementById('style-seteting')
  styleSetting.click()
})

ipcRenderer.on('receiveStreamerInfo', (e, info) => {
  streamerInfo = info
})

// ----------------------------------------------------------------------------------------------------

ipcRenderer.on('GameIsForeground', (e, info) => {
  const foregroundContainer = document.getElementById('foreground-container')
  if (foregroundContainer != null) {
    isForeground = info.isForeground
    foregroundContainer.click()
  }
})

contextBridge.exposeInMainWorld('electron', {
  isReady: () => {
    ipcRenderer.send('overlay-isReady')
  },
  getIsforeground: () => {
    return isForeground
  },
  getStreamerInfo: () => {
    console.log('raise getStreamerInfo')
    return streamerInfo
  }
})
