console.log('render')
window.addEventListener('DOMContentLoaded', () => {
  let el = document.getElementById('btn')
  el.addEventListener('click', () => {
    window.api.ignoreMouse()
  })
})
