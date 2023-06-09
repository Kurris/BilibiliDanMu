// console.log(window.api.ligy)

window.addEventListener('DOMContentLoaded', () => {
  let ignoreMouse = document.getElementById('ignoreMouse')
  ignoreMouse.addEventListener('click', () => {
    window.api.ignoreMouse()
  })
})
