<template>
    <div id="home">

        <img v-if="streamer.info.roomInfo?.backgroundUrl" class="bg-img" referrerpolicy="no-referrer"
            :src="streamer.info.roomInfo?.backgroundUrl" alt="" srcset="" z-index="-100">

        <div class="main-container">

            <!-- <div class="connectionStatus">

                <div v-if="signalR.connected()"
                    style="background-color: green;border-radius: 50%; height: 60px;width: 60px; " />
                <div v-else style="background-color: red;border-radius: 50%; height: 60px;width: 60px;" />
            </div> -->

            <div id="music-container" @click="musicContainerRaiseClick">
                <div class="music-title">{{ mediaInfo.music?.title }}</div>
            </div>

            <div id="game-container" @click="gameContainerRaiseClick">
                <img v-if="mediaInfo.game?.image" id="game-img" :src="mediaInfo.game.image" :title="mediaInfo.game.title" />
            </div>
            <div id="foreground-container" @click="foregroundContainerRaiseClick" />

            <!-- <PreviewStyle :height="500" :width="380">
                <Barrage :danmu-count="10" entry-effect-direction="left" :show-avatar="true" :show-medal="true" />
            </PreviewStyle> -->

        </div>
    </div>
</template>
<script setup lang="ts">

import { onBeforeMount, onMounted, ref, watch } from 'vue';
import { useSignalR } from '../stores/signalRStore';
import { useMediaInfo } from '../stores/mediaInfoStore';
import { useStreamer } from '../stores/streamerStore';
import Barrage from '../components/Barrage.vue';
import PreviewStyle from '../components/PreviewStyle.vue';
import { AppSetting } from '../utils/appSetting';
import { useFetch } from '@vueuse/core';



const signalR = useSignalR()
const mediaInfo = useMediaInfo()
const streamer = useStreamer()

const bgColor = ref('#6ce88f')
watch(() => streamer.info.roomInfo.backgroundUrl, () => {
    bgColor.value = streamer.info.roomInfo.backgroundUrl != "" ? 'unset' : '#eafaee'
})


const gameContainerRaiseClick = async () => {
    const result = window.electron.getGameInfo()
    mediaInfo.game.title = result.title
    mediaInfo.game.name = result.name
    mediaInfo.game.path = result.path
    mediaInfo.game.image = result.image
    mediaInfo.game.useOverlay = true

}

const musicContainerRaiseClick = () => {
    mediaInfo.music.title = window.electron.getMusicInfo().title
}

const foregroundContainerRaiseClick = () => {
    mediaInfo.isforeground = window.electron.getIsforeground()
}


const connectRoom = () => {

    if (streamer.info.roomInfo?.roomId > 0) {
        useFetch(AppSetting.VITE_API_URL + "/api/barrage/receive").post(JSON.stringify({
            connectionId: signalR.connectionId(),
            roomId: streamer.info.roomInfo.roomId
        }), 'application/json').json()

        console.log('join room :' + streamer.info.roomInfo?.roomId);
    }

}


onBeforeMount(() => {
    // signalR.start().then(() => {
    //     connectRoom()
    // })
})

onMounted(() => {

    try {
        window.electron.runService()
        window.electron.overlay(JSON.parse(JSON.stringify(streamer.info)))
    } catch (error) {

        console.log(error);
    }



})

</script>
  
<style lang="scss">
#home {
    position: absolute;
    height: 100%;
    width: 100%;
    background-color: v-bind(bgColor);

}

.connectionStatus {
    position: absolute;
    top: -3.5%;
    left: -1.8%;
    // box-shadow: 2px 2px 5px 5px rgba(0, 0, 0, 0.2);
}

.music-title {
    height: 12.5px;
    text-shadow: 0 0 2px hsl(40, 28.57%, 28.82%), 0 0 2px hsl(40, 28.57%, 28.82%), 0 0 2px hsl(40, 28.57%, 28.82%);
    font-size: 12.5px;
    font-weight: bolder;
}

.main-container {
    position: absolute;
    left: 50%;
    top: 50%;
    transform: translate(-50%, -50%);
    width: 90%;
    height: 82%;
    padding: 40px;
    background-color: rgba($color: #ffffff, $alpha: 0.9);
    border-radius: 1%;
    border: 1px solid #182736;

    // #game-container {
    //     position: absolute;
    //     bottom: 0;
    // }
}

.bg-img {
    position: absolute;
    width: 100%;
    height: 100%;
    object-fit: fill;
}

.el-notification__group {
    margin: {
        left: 0 !important;
        right: 0 !important;
    }
}

.el-notification {
    width: unset !important;
    padding: 0 !important;
    border: unset;
}
</style>
  