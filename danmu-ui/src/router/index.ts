import { createRouter, createWebHistory } from 'vue-router'

//createWebHistory(import.meta.env.BASE_URL),
const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/",
      redirect: "/login",
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/Login.vue')
    },
    {
      path: '/home',
      name: 'home',
      component: () => import('../views/Home.vue')
    },
    {
      path: '/overlay',
      name: 'overlay',
      component: () => import('../views/Overlay.vue')
    },
  ]
})




// {
//   path: '/login',
//   name: 'login',
//   component: () => import('../views/Login.vue')
// },


export default router
