<template>
    <div id="overlay">
        <v-d-r v-show="!state.isIgnoreMouse" class="status item-bg" :w="380" :h="100" :x="10" :y="height - 120"
            :resizable="false" :parent="true" :draggable="true">

            <div class="room" style="padding: 10px;">
                <div style="display: flex;">

                    <div>
                        <div v-if="signalR.connected()" title="connected"
                            style="height: 16px ;width: 16px; background-color: green;border-radius: 50%;" />
                        <div v-else title="disconnect"
                            style="height: 16px ;width: 16px; background-color: red;border-radius: 50%;" />
                    </div>


                    <div style="display: flex;margin-left: 20px;">
                        <svg style="color: rgb(7, 94, 109);" xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                            fill="currentColor" class="bi bi-house-heart" viewBox="0 0 16 16">
                            <path d="M8 6.982C9.664 5.309 13.825 8.236 8 12 2.175 8.236 6.336 5.309 8 6.982Z" />
                            <path
                                d="M8.707 1.5a1 1 0 0 0-1.414 0L.646 8.146a.5.5 0 0 0 .708.707L2 8.207V13.5A1.5 1.5 0 0 0 3.5 15h9a1.5 1.5 0 0 0 1.5-1.5V8.207l.646.646a.5.5 0 0 0 .708-.707L13 5.793V2.5a.5.5 0 0 0-.5-.5h-1a.5.5 0 0 0-.5.5v1.293L8.707 1.5ZM13 7.207V13.5a.5.5 0 0 1-.5.5h-9a.5.5 0 0 1-.5-.5V7.207l5-5 5 5Z" />
                        </svg>
                        <div>
                            : {{ streamser.info.roomInfo.roomId }}
                        </div>
                    </div>
                    <div style="display: flex;margin-left: 20px;">
                        <svg style="color: red;" xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                            fill="currentColor" class="bi bi-fire" viewBox="0 0 16 16">
                            <path
                                d="M8 16c3.314 0 6-2 6-5.5 0-1.5-.5-4-2.5-6 .25 1.5-1.25 2-1.25 2C11 4 9 .5 6 0c.357 2 .5 4-2 6-1.25 1-2 2.729-2 4.5C2 14 4.686 16 8 16Zm0-1c-1.657 0-3-1-3-2.75 0-.75.25-2 1.25-3C6.125 10 7 10.5 7 10.5c-.375-1.25.5-3.25 2-3.5-.179 1-.25 2 1 3 .625.5 1 1.364 1 2.25C11 14 9.657 15 8 15Z" />
                        </svg>
                        <div> : {{ signalR.data.hot.hot }}</div>
                    </div>

                    <div style="display: flex;margin-left: 20px;">
                        <el-icon style="width: 16px;height: 16px;color: orange">
                            <UserFilled />
                        </el-icon>
                        <div>: {{ signalR.data.watched.num }}</div>
                    </div>

                </div>
            </div>

            <div class="streamer">
                <div>
                    <img height=" 42" width="42" :src="streamser.info.face"
                        style="position:  absolute;border-radius: 50%; " />
                    <img height="60" width="60" style="position: absolute;margin-top: -9px;margin-left: -9px;"
                        :src="streamser.info.pendant" />
                </div>

                <div style="margin-left: 60px;">
                    <div style="display:flex">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor"
                            class="bi bi-flag-fill" viewBox="0 0 16 16">
                            <path
                                d="M14.778.085A.5.5 0 0 1 15 .5V8a.5.5 0 0 1-.314.464L14.5 8l.186.464-.003.001-.006.003-.023.009a12.435 12.435 0 0 1-.397.15c-.264.095-.631.223-1.047.35-.816.252-1.879.523-2.71.523-.847 0-1.548-.28-2.158-.525l-.028-.01C7.68 8.71 7.14 8.5 6.5 8.5c-.7 0-1.638.23-2.437.477A19.626 19.626 0 0 0 3 9.342V15.5a.5.5 0 0 1-1 0V.5a.5.5 0 0 1 1 0v.282c.226-.079.496-.17.79-.26C4.606.272 5.67 0 6.5 0c.84 0 1.524.277 2.121.519l.043.018C9.286.788 9.828 1 10.5 1c.7 0 1.638-.23 2.437-.477a19.587 19.587 0 0 0 1.349-.476l.019-.007.004-.002h.001" />
                        </svg>
                        <div style="margin-left: 5px;">: {{ streamser.info.roomInfo.title }}</div>

                    </div>
                    <div style="margin-top: 5px;">{{ streamser.info.userName }}</div>
                </div>
            </div>


        </v-d-r>

        <v-d-r v-show="state.isForeground || !state.isIgnoreMouse" :class="{ 'item-bg': !state.isIgnoreMouse }" :w="300"
            :h="150" :x="width / 2 - 300 / 2" :y="10" :resizable="false" :parent="true" :draggable="true">
            <GiftCover />
        </v-d-r>


        <v-d-r v-show="state.isForeground || !state.isIgnoreMouse" :class="{ 'item-bg': !state.isIgnoreMouse }"
            :min-width="200" :w="380" :h="650" :x="3" :y="height / 2 - 650 / 2" :resizable="false" :parent="true"
            :draggable="true">

            <Barrage :room-id="currentRoomId" ref="danmu" :danmu-count="currentDanmuCount"
                :entry-effect-direction="currentEntryEffectDirection" :show-avatar="currentShowAvatar"
                :show-medal="currentShowMedal" />

        </v-d-r>
    </div>
    <div id="foreground-container" @click="getIsForeground" />
    <div id="style-seteting" @click="styleSetting" />
</template>
<script setup lang="ts">

import { ref, reactive, onBeforeMount } from 'vue';
import Barrage from '../components/Barrage.vue';
import { useFetch, useWindowSize } from '@vueuse/core'


import { useSignalR } from '../stores/signalRStore';
import GiftCover from '../components/GiftCover.vue';
import { AppSetting } from '../utils/appSetting';
import { useStreamer } from '../stores/streamerStore';

const streamser = useStreamer()
const currentRoomId = ref<number>();
const currentDanmuCount = ref(15)
const currentEntryEffectDirection = ref('left')
const currentShowAvatar = ref(true)
const currentShowMedal = ref(true)
const signalR = useSignalR()
const { height, width } = useWindowSize()

const state = reactive({
    isForeground: false,
    isIgnoreMouse: true,
})

const bgColor = ref('transparent')

const connectRoom = () => {

    useFetch(AppSetting.VITE_API_URL + "/api/barrage/receive").post(JSON.stringify({
        connectionId: signalR.connectionId(),
        roomId: streamser.info.roomInfo.roomId
    }), 'application/json').json()
}

//会被electron触发
const getIsForeground = () => {
    state.isForeground = window.electron.getIsforeground()
    bgColor.value = state.isIgnoreMouse ? 'transparent' : 'rgba(36,41,46,0.9)'
}



const styleSetting = () => {
    state.isIgnoreMouse = !state.isIgnoreMouse
}

onBeforeMount(() => {

    streamser.info = window.electron.getStreamerInfo()

    signalR.start().then(() => {
        connectRoom()
    })
})


</script>
  
<style lang="scss">
$radius : 1.2%;

body {}

#overlay {
    position: absolute;
    height: 100%;
    width: 100%;
    background-color: transparent;
    user-select: none;
    background-color: v-bind(bgColor);
}


h1 {
    width: 100px;
    overflow: hidden;
    white-space: nowrap;
    text-overflow: ellipsis;
}


.item-bg {
    background-color: #212325;
    border-radius: 2%;
    box-shadow: 2px 2px 5px 5px rgba(0, 0, 0, 0.2);
}

.status {
    text-shadow: 0 0 2px hsl(40, 28.57%, 28.82%), 0 0 2px hsl(40, 28.57%, 28.82%), 0 0 2px hsl(40, 28.57%, 28.82%);
    font-size: 12.5px;
    font-weight: bolder;
    color: white;

    .room {
        display: flex;
        justify-content: center;
    }

    .streamer {
        display: flex;
        justify-content: center;
    }
}


.el-notification__group {
    margin: {
        left: 0 !important;
        right: 0 !important;
    }
}

.vdr-container.active {
    border-color: unset;
    border: unset;
}

.el-notification {
    width: unset !important;
    padding: 0 !important;
    border: unset;
}
</style>
  