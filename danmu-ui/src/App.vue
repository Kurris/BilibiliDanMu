<template>
  <div id="app">
    <div class="title electron-draggable" v-if="router.currentRoute.value.name != 'overlay'" />

    <RouterView />
  </div>
</template>
<script setup lang="ts">
import { useRouter } from 'vue-router';
const router = useRouter()
</script>

<style lang="scss">
@import 'styles/global.scss';


#app {
  position: absolute;
  height: 100%;
  width: 100%;

  .title {
    height: 60px;
  }
}
</style>
