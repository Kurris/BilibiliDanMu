<template>
  <div id="app">
    <div id="setting" class="set-room">
      <div class="container">
        <div class="title">设置面板</div>
        <span>请输出房间号:</span>
        <el-input v-model="roomId" type="number"></el-input>
        <el-button id="btn" @click="connectRoom">连接房间</el-button>
      </div>
    </div>
    <DanMu :room-id="roomId" ref="danmu" />
  </div>
</template>
<script setup lang="ts">

import { ref } from 'vue';
import DanMu from './components/DanMu.vue';

const roomId = ref(23982773);

const danmu = ref<InstanceType<typeof DanMu>>()

const connectRoom = () => {
  danmu.value!.connectRoom()
}

</script>

<style scoped lang="scss">
@import 'styles/global.scss';

#app {
  overflow-y: hidden;
  overflow-x: hidden;
  overflow: hidden;
}

.set-room {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background-color: white;
  color: black;
  height: 600px;
  width: 800px;

  border-radius: 3%;
  box-shadow: 1px 2px 3px 4px rgba(221, 220, 220, 0.2);

  padding: 20px;

  .container {
    display: flex;
    justify-content: center;
    align-items: center;
    flex-direction: column;

    .title {
      display: flex;
      justify-content: center;
      align-items: center;
    }
  }
}
</style>
