<template>
  <div id="app">
    <el-drawer v-model="isDrawer" title="设置面板" direction="rtl">
      <DmSetting @connect-room="connectRoom" @set-raise="setRaise" :is-drawer="isDrawer" @cover="cover" />
    </el-drawer>

    <DanMu :room-id="currentRoomId" ref="danmu" @on-drag-stop="onDragStop" :danmu-count="currentDanmuCount"
      :entry-effect-direction="currentEntryEffectDirection" :show-avatar="currentShowAvatar"
      :show-medal="currentShowMedal" @on-sc="sc" />
  </div>
</template>
<script setup lang="ts">

import { nextTick, ref } from 'vue';
import DmSetting from './components/DmSetting.vue'
import DanMu from './components/DanMu.vue';
import { ElNotification } from 'element-plus'

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

const sc = (data) => {
  console.log(data);
  ElNotification({
    dangerouslyUseHTMLString: true,
    duration: 1000 * 20,
    showClose: false,
    message: ` <div style='width:300px;background-color: ${data.background_color};font-size: 10px;'>
      <div style="display: flex;align-items: center; justify-content: space-between;height: 40px;padding: 8px;">
        <div class="avatar-medal-name">
          <div class="medal">
            <span class="medal-name">${data.medal_info.medal_name}</span>
            <span class="medal-lvl">${data.medal_info.medal_level}</span>
          </div>
          <div class="name" style="color='${data.user_info.name_color}'">${data.user_info.uname}</div>
        </div>
        <div>
          ${data.price * 10}电池
        </div>
      </div>
      <div style="background-color: #2A60B2;padding: 10px;color: ${data.message_font_color};font-size: 10px;">
        ${data.message}
      </div>
    </div>`,
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

.avatar-medal-name {
  font-size: 13px;
  white-space: nowrap;
  display: flex;
  align-items: center;

  .medal {
    padding: 1.2px;
    border-radius: 12%;

    .medal-name {
      border: 1px solid orange;
      background-color: orange;

      padding: {
        left: 3px;
        right: 3px;
      }
    }

    .medal-lvl {
      border: 1px solid orange;
      font-weight: bolder;

      padding: {
        left: 3px;
        right: 3px;
      }
    }
  }

  .name {
    margin-right: 3.5px;

    padding: {
      left: 3px;
      right: 3px;
    }

    border-radius: 10%;
    color: black;
  }
}
</style>
