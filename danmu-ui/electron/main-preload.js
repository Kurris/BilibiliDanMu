// eslint-disable-next-line no-undef
const { ipcRenderer, contextBridge } = require('electron')

var gameInfo = {
  handle: 0,
  title: '',
  name: '',
  path: '',
  image: ''
}

var music = {
  title: ''
}

var isForeground = false

ipcRenderer.on('ignoreMouse', (event, bg) => {
  let body = document.getElementsByTagName('body')[0]
  body.style.backgroundColor = bg
})

ipcRenderer.on('DetectGameRunning', (e, info) => {
  const gameContainer = document.getElementById('game-container')
  if (gameContainer != null) {
    if (gameInfo.handle != info.handle) {
      gameInfo = info
      gameContainer.click()
    }
  }
})

ipcRenderer.on('GetMusicInfo', (e, info) => {
  const musicContainer = document.getElementById('music-container')
  if (musicContainer != null) {
    if (music.title !== info.title) {
      music = info
      musicContainer.click()
    }
  }
})

contextBridge.exposeInMainWorld('electron', {
  ignoreMouse: () => {
    ipcRenderer.send('ignoreMouse')
  },
  sendMessageToBackend: (...args) => {
    ipcRenderer.send('backend-server', args)
  },
  getGameInfo: () => {
    return gameInfo
  },
  getMusicInfo: () => {
    return music
  },
  runService: () => {
    ipcRenderer.send('runService')
  },
  getIsforeground: () => {
    return isForeground
  },
  overlay: (info) => {
    ipcRenderer.send('overlay', info)
  }
})
