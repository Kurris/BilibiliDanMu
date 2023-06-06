window.addEventListener('DOMContentLoaded', () => {
  let el = document.getElementById('ignoreMouse')
  el.addEventListener('click', () => {
    window.api.ignoreMouse()
  })
})
