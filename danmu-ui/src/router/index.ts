import { createRouter, createWebHashHistory, createWebHistory } from 'vue-router'

//createWebHistory(import.meta.env.BASE_URL),
const router = createRouter({
  history: createWebHistory(),
  routes: [
    // {
    //   path: '/',
    //   name: 'home',
    //   component: HomeView
    // },
    {
      path: '/overlay',
      name: 'overlay',
      component: () => import('../views/Overlay.vue')
    }
  ]
})

export default router
