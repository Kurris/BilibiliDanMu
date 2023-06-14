<template>
  <div id="danmu-app">
    <div class="title electron-draggable">
      <!-- 自定义按钮 win.titleBarStyle.customButtonsOnHover-->
      <!-- <div class="operator">
        <el-icon id="setMini" class="noDraggable">
          <SemiSelect />
        </el-icon>
        <el-icon id="setMax" class="noDraggable">
          <FullScreen />
        </el-icon>
        <el-icon id="setToTray" class="noDraggable">
          <Close />
        </el-icon>
      </div> -->
    </div>
    <div class="main-container" v-show="currentShowWindow">
      <v-d-r :min-width="200" :w="380" :h="550" :x="200" :y="200" @dragging="resize" @resize="resize" :resizable="false"
        :parent-limitation="false" :draggable="true" classNameDragging="dragging-class">
        <el-icon class="setting-btn" @click="isDrawer = !isDrawer">
          <Setting />
        </el-icon>

        <DanMu :room-id="currentRoomId" ref="danmu" :danmu-count="currentDanmuCount"
          :entry-effect-direction="currentEntryEffectDirection" :show-avatar="currentShowAvatar"
          :show-medal="currentShowMedal" @on-sc="sc" />
      </v-d-r>
    </div>

    <el-drawer v-model="isDrawer" title="设置面板" direction="rtl" :show-close="false">
      <DmSetting @connect-room="connectRoom" @set-raise="setRaise" :is-drawer="isDrawer" />
    </el-drawer>


  </div>
</template>
<script setup lang="ts">

import { nextTick, ref, reactive, h } from 'vue';
import DmSetting from './components/DmSetting.vue'
import DanMu from './components/DanMu.vue';
import { ElNotification } from 'element-plus'

import { useWebNotification } from '@vueuse/core'

const currentRoomId = ref<number>();
const isDrawer = ref(false)
const danmu = ref<InstanceType<typeof DanMu>>()
const currentDanmuCount = ref(10)
const currentEntryEffectDirection = ref('left')
const currentShowAvatar = ref(true)
const currentShowMedal = ref(true)
const currentShowWindow = ref(true)


const {
  show,
} = useWebNotification({
  title: 'Hello, VueUse world!',
  dir: 'auto',
  lang: 'en',
  renotify: true,
  tag: 'test',
})

show()

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


const resize = (newRect) => {
  position.width = newRect.width;
  position.height = newRect.height;
  position.top = newRect.top;
  position.left = newRect.left;
}

const setRaise = (danmuCount: number, entryEffectDirection: string, showAvatar: boolean, showMedal: boolean, showWindow: boolean) => {
  currentDanmuCount.value = danmuCount
  currentEntryEffectDirection.value = entryEffectDirection
  currentShowAvatar.value = showAvatar;
  currentShowMedal.value = showMedal
  currentShowWindow.value = showWindow
}

//用户自定义处理css
// const testCustomCss = ref(`.main-container {
//    background-color: black;
//    left:60px;
// }`)
// document.getElementById('user-customer').innerHTML = testCustomCss.value;


const sc = (data) => {

  ElNotification({
    dangerouslyUseHTMLString: true,
    duration: 1000 * 120,
    showClose: true,
    message: h({
      template: ` 
    <div style='width:300px;background-color: ${data.backgroundColor};font-size: 10px;'>

      <div style="height:40px;">
        <div style="padding-left:10px;padding-top:2px">
          <img referrer="no-referrer" src="${data.userFace}" width="35" height="35" style="position:absolute;border-radius:50%;"/>
          <img v-if="'${data.userFaceFrame}'!=''" referrer="no-referrer" src="${data.userFaceFrame}" width="35" height="35" style="position:absolute;"/>
        </div>
      
        <div style="margin-left:60px;display: flex;align-items: center;padding-top:8px;">
          <div v-if="${data.hasMedal}"  style="display: flex;justify-content: center;align-items: center;background-color:orange;border-radius: 12%;">
              <div style="color:${data.medalColor};">${data.medalName}</div>
              <div style="color:white;margin-left:5px;margin-right:5px;">${data.medalLevel}</div>
          </div>
          <div style="color='${data.userNameColor}';margin-left:10px">${data.userName}</div>
        </div>

        <img src="${data.backgroundImage}" width="100" height="40" style="position:absolute;top:0;;right:-8px;"/>
        <div style="color:${data.backgroundPriceColor};position:absolute;top:8px;right:25px;">
            ${data.priceString}
        </div>
      </div>
      <div style="background-color: ${data.backgroundBottomColor};padding: 10px;color: ${data.messageFontColor};font-size: 10px;">
          ${data.message}
      </div>
    </div>
    `
    }),
  })
}


</script>

<style lang="scss">
@import 'styles/global.scss';

$radius : 1.2%;


.title {
  color: #afadad;
  height: 60px;
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  display: flex;
  justify-content: flex-end;
  border-radius: $radius $radius 0% 0%;

  .operator {

    margin: {
      top: 10px;
      right: 10px;
    }

    .el-icon:hover {
      cursor: pointer;
    }

    .el-icon+.el-icon {
      margin-left: 10px;
    }
  }
}


.dragging-class {
  background-color: #212325;
  border-radius: 2%;
  box-shadow: 3px 5px 5px #656B79;
}

.vdr-container.active {
  border-color: unset;
  border: unset;
}

.main-container {
  position: absolute;
  width: 100%;
  height: calc(100vh - 60px);
  top: 60px;
}

.setting-btn {
  cursor: pointer;
  color: #cbcccd;
  transition: all 1s ease;

  &:hover {
    transform: rotate(180deg);
  }
}
</style>
