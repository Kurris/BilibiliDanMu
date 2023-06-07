// console.log(window.api.ligy)
window.addEventListener('DOMContentLoaded', () => {
  let el = document.getElementById('ignoreMouse')
  el.addEventListener('click', () => {
    window.api.ignoreMouse()
  })
})
