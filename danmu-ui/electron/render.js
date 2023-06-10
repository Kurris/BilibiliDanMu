let moveSetRestore = false

export function registerTitleEvent() {
  let setMini = document.getElementById('setMini')
  setMini.addEventListener('click', () => {
    window.api.setMini()
  })

  let setMax = document.getElementById('setMax')
  setMax.addEventListener('click', () => {
    window.api.setMax()
  })

  let setToTray = document.getElementById('setToTray')
  setToTray.addEventListener('click', () => {
    window.api.setToTray()
  })

  // class="draggable"
  let title = document.getElementById('title')
  title.addEventListener('dblclick', () => {
    window.api.setMax()
  })

  title.addEventListener('mousedown', (downEvent) => {
    document.onmousemove = (moveEvent) => {
      if (!moveSetRestore) {
        window.api.restoreSize()
        moveSetRestore = true
      }

      window.api.dragTitle({
        x: moveEvent.screenX - downEvent.x,
        y: moveEvent.screenY - downEvent.y
      })
    }

    document.onmouseup = () => {
      moveSetRestore = false
      document.onmousemove = document.onmouseup = null
    }
  })
}

window.addEventListener('DOMContentLoaded', () => {
  registerTitleEvent()
})
