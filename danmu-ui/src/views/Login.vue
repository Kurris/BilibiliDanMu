<template>
    <div id="connect">
        <img referrerpolicy="no-referrer"
            v-if="streamer.info.roomInfo?.backgroundUrl || streamer.info.roomInfo?.userCoverUrl"
            :src="streamer.info.roomInfo?.backgroundUrl || streamer.info.roomInfo?.userCoverUrl" alt="" srcset=""
            z-index="-100">

        <div class="contaienr">
            <el-avatar class="avatar" v-if="streamer.info.face" :src="streamer.info.face" :size="120"></el-avatar>

            <img v-if="streamer.info.roomInfo?.userCoverUrl" style="border-radius: 10%;" height="300" width="500"
                referrerpolicy="no-referrer" :src="streamer.info.roomInfo?.userCoverUrl" alt="" srcset="">

            <el-form class="user-input">
                <el-form-item>
                    <div style="display: flex;">
                        <el-select ref="input" v-model="streamer.info" :remote-method="searchStreamer" default-first-option
                            filterable remote placeholder="请输入房间号(支持短号)" value-key="uid" no-match-text="找不到房间哦~">
                            <el-option v-for="item in tempStreamers" :key="item.uid" :value="item"
                                :label="`${item.roomInfo.roomId}:` + item.userName">

                                <div style="display: flex;justify-content: start;align-items: center;">
                                    <el-avatar v-if="item.face" :src="item.face" :size="36"></el-avatar>
                                    <span style="margin-left: 10px;">{{ item.userName }}</span>
                                </div>

                            </el-option>
                        </el-select>

                        <el-button type="success" @click="goHome"
                            :disabled="streamer.info == null || streamer.info.roomInfo.liveStatus != 1">connect</el-button>
                    </div>
                </el-form-item>
            </el-form>
        </div>
    </div>
</template>
<script setup lang="ts">

import { useFetch, useStorage } from '@vueuse/core';
import { onStartTyping } from '@vueuse/core'
import { useRouter } from 'vue-router';
import { onBeforeMount, ref } from 'vue';
import { useStreamer } from '../stores/streamerStore';
import { AppSetting } from '../utils/appSetting';
import { IStreamerInfo } from '../type.d/bilibilidata'
import { IApiResult } from '../type.d/http'
import { useSignalR } from '../stores/signalRStore';

const router = useRouter()
const signalR = useSignalR()
const streamer = useStreamer()
const tempStreamers = ref(Array<IStreamerInfo>());

const input = ref(null)

/** 输入自动聚焦 */
onStartTyping(() => {
    if (!input.value.active)
        input.value.focus()
})


/** 跳到主页 */
const goHome = async () => {

    if (!streamer.info.roomInfo?.roomId || streamer.info.roomInfo?.roomId <= 0) {
        return
    }

    const storeStreamers = useStorage<Array<IStreamerInfo>>('streamers', [])
    const index = storeStreamers.value.findIndex(x => x.uid == streamer.info.uid)

    if (index >= 0) {
        storeStreamers.value.splice(index, 1)
    }

    storeStreamers.value.unshift(streamer.info)

    router.push({
        name: 'home'
    })
}


/** 查询房间号 */
const searchStreamer = async (query: number) => {

    const value = Number(query)
    if (!Number.isNaN(value) && value != 0) {

        const response = await useFetch(AppSetting.VITE_API_URL + "/api/streamer?roomId=" + value).get().json()
        const result = response.data.value as IApiResult<IStreamerInfo>

        if (result.code != 200) {
            return
        }

        if (tempStreamers.value.find(x => x.uid == result.data.uid)) {
            return
        }

        tempStreamers.value.push(result.data)
    }
}


onBeforeMount(() => {
    const storeStreamers = useStorage<Array<IStreamerInfo>>('streamers', [])
    if (storeStreamers.value.length > 0) {
        tempStreamers.value.push(...storeStreamers.value)
        streamer.info = tempStreamers.value[0]
    }

    signalR.init()
})


</script>
<style scoped lang="scss">
#connect {
    position: absolute;
    height: 100%;
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    user-select: none;
}

.contaienr {
    position: absolute;
    height: 300px;
    width: 500px;
    background-color: white;
    border-radius: 10%;
    box-shadow: 2px 2px 5px 5px rgba(0, 0, 0, 0.2);

    display: flex;
    align-items: center;
    justify-content: center;
}

.avatar {
    position: absolute;
    left: -5%;
    top: -20%;
    box-shadow: 2px 2px 5px 5px rgba(0, 0, 0, 0.2);

    transition: all 1s ease;

    &:hover {
        transform: rotate(360deg);
    }
}

.user-input {
    position: absolute;
}

img {
    width: 100%;
    height: 100%;
    object-fit: fill;
}
</style>