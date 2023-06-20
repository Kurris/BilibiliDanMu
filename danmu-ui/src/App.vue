<template>
  <div id="app">
    <div class="title electron-draggable" />

    <RouterView />
  </div>
</template>
<script setup lang="ts">

</script>

<style lang="scss">
@import 'styles/global.scss';


#app {
  position: absolute;
  height: 100%;
  width: 100%;

  .title {
    height: 60px;
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    display: flex;
  }
}
</style>
