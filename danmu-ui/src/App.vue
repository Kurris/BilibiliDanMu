<template>
  <div id="app">
    <el-drawer v-model="isDrawer" title="设置面板" direction="rtl">
      <DmSetting @connect-room="connectRoom" @set-raise="setRaise" :is-drawer="isDrawer" @cover="cover" />
    </el-drawer>

    <VueDragResize min-width="200" :w="380" :h="550" @dragging="resize" @resize="resize">
      <el-icon style="cursor: pointer;" @click="isDrawer = !isDrawer">
        <Setting />
      </el-icon>
      <DanMu :room-id="currentRoomId" ref="danmu" :danmu-count="currentDanmuCount"
        :entry-effect-direction="currentEntryEffectDirection" :show-avatar="currentShowAvatar"
        :show-medal="currentShowMedal" @on-sc="sc" />
    </VueDragResize>

    <button style="display:none ;" id="unIgnoreMouse" @click="unIgnoreMouse"></button>
  </div>
</template>
<script setup lang="ts">

import { nextTick, ref, reactive } from 'vue';
import DmSetting from './components/DmSetting.vue'
import DanMu from './components/DanMu.vue';
import { ElNotification } from 'element-plus'
import VueDragResize from 'vue-drag-resize'

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

const position = reactive({
  width: 0,
  height: 0,
  top: 0,
  left: 0
})



const cover = () => {
  isDrawer.value = false
}
const unIgnoreMouse = () => {
  isDrawer.value = true
}

const resize = (newRect) => {
  position.width = newRect.width;
  position.height = newRect.height;
  position.top = newRect.top;
  position.left = newRect.left;

  // console.log(position);
}

const setRaise = (danmuCount: number, entryEffectDirection: string, showAvatar: boolean, showMedal: boolean) => {
  currentDanmuCount.value = danmuCount
  currentEntryEffectDirection.value = entryEffectDirection
  currentShowAvatar.value = showAvatar;
  currentShowMedal.value = showMedal

}

const sc = (data) => {

  ElNotification({
    dangerouslyUseHTMLString: true,
    duration: 1000 * 200,
    showClose: false,
    message: ` 
    <div style='width:300px;background-color: ${data.background_color};font-size: 10px;'>

      <div style="height:40px;">
        <div style="padding-left:10px;">
          <img src="${data.user_info.face}" width="35" height="35" style="position:absolute;border-radius:50%;"/>
          <img src="${data.user_info.face_frame}" width="35" height="35" style="position:absolute;"/>
        </div>
      
        <div style="margin-left:60px;display: flex;align-items: center;padding-top:8px;">
          <div style="display: flex;justify-content: center;align-items: center;background-color:orange;border-radius: 12%;">
              <div style="color:${data.medal_info.medal_color};">${data.medal_info.medal_name}</div>
              <div style="color:white;margin-left:5px;margin-right:5px;">${data.medal_info.medal_level}</div>
          </div>
          <div style="color='${data.user_info.name_color}';margin-left:10px">${data.user_info.uname}</div>
        </div>

        <img src="${data.background_image}" width="100" height="40" style="position:absolute;top:0;;right:-8px;"/>
        <div style="color:${data.background_price_color};position:absolute;top:8px;right:25px;">
            ${data.price * 10}电池
        </div>
      </div>
      <div style="background-color: ${data.background_bottom_color};padding: 10px;color: ${data.message_font_color};font-size: 10px;">
          ${data.message}
      </div>
    </div>
    `,
  })
}

</script>

<style   lang="scss">
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
