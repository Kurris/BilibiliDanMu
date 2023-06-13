
import { createApp } from 'vue'
import { createPinia } from 'pinia'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'

import 'vue3-draggable-resizable/dist/Vue3DraggableResizable.css'
import VueDraggableResizable from 'vue3-draggable-resizable'

import App from './App.vue'
import router from './router'

const app = createApp(App)

for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
    app.component(key, component)
}

app.component('v-d-r', VueDraggableResizable)
app.use(createPinia())
    .use(ElementPlus)
    .use(router)
    .mount('#app')
