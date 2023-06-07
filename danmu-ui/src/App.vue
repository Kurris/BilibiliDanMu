<template>
  <div id="app">
    <el-drawer v-model="isDrawer" title="设置面板" direction="rtl">
      <DmSetting @connect-room="connectRoom" @set-raise="setRaise" :is-drawer="isDrawer" @cover="cover" />
    </el-drawer>

    <DanMu :room-id="currentRoomId" ref="danmu" @on-drag-stop="onDragStop" :danmu-count="currentDanmuCount"
      :entry-effect-direction="currentEntryEffectDirection" :show-avatar="currentShowAvatar"
      :show-medal="currentShowMedal" />

  </div>
</template>
<script setup lang="ts">

import { nextTick, ref } from 'vue';
import DmSetting from './components/DmSetting.vue'
import DanMu from './components/DanMu.vue';

const currentRoomId = ref<number>();
const isDrawer = ref(true)
const danmu = ref<InstanceType<typeof DanMu>>()
const currentDanmuCount = ref(15)
const currentEntryEffectDirection = ref('left')
const currentShowAvatar = ref(true)
const currentShowMedal = ref(true)

const connectRoom = (roomId: number) => {
  currentRoomId.value = roomId
  nextTick(() => {
    danmu.value!.connectRoom()
  })
}

const onDragStop = () => {
  console.log('drawer');

  isDrawer.value = true
}
const cover = () => {
  isDrawer.value = false
}

const setRaise = (danmuCount: number, entryEffectDirection: string, showAvatar: boolean, showMedal: boolean) => {
  currentDanmuCount.value = danmuCount
  currentEntryEffectDirection.value = entryEffectDirection
  currentShowAvatar.value = showAvatar;
  currentShowMedal.value = showMedal
}

</script>

<style scoped lang="scss">
@import 'styles/global.scss';


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
