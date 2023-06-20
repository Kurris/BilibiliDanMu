<template>
    <el-form :model="form" label-width="120px">
        <el-form-item label="房间号(支持短号)">
            <div style="display: flex;">
                <el-input-number v-model="roomInfo.id" />
                <el-button @click="connectRoom" type="warning" style="margin-left: 20px;">连接房间</el-button>
            </div>
        </el-form-item>
        <el-form-item label="弹幕数显示">
            <el-input-number type="number" v-model="form.danmuCount" />
        </el-form-item>
        <el-form-item label="舰长提醒方向">
            <el-radio-group v-model="form.entryEffectDirection">
                <el-radio label="left">左到右</el-radio>
                <el-radio label="right">右向左</el-radio>
                <!-- <el-radio label="bottom">下向上</el-radio> -->
            </el-radio-group>
        </el-form-item>
        <el-form-item label="是否显示头像">
            <el-switch v-model="form.showAvatar" />
        </el-form-item>
        <el-form-item label="是否显示牌子">
            <el-switch v-model="form.showMedal" />
        </el-form-item>
        <el-form-item label="是否窗口边框">
            <el-switch v-model="form.showWindow" />
        </el-form-item>
    </el-form>
</template>
<script setup lang="ts">
import { reactive, watch } from 'vue'
import { useSignalR } from '../stores/signalRStore';
import { useFetch } from '@vueuse/core'
import { AppSetting } from '../utils/appSetting';
import { useRoom } from '../stores/streamerStore';

const signalR = useSignalR();
const roomInfo = useRoom()

const form = reactive({
    danmuCount: 15,
    entryEffectDirection: 'left',
    showAvatar: true,
    showMedal: true,
    showWindow: true,
})

watch(() => JSON.stringify(form), () => {
    emits('setRaise', form.danmuCount, form.entryEffectDirection, form.showAvatar, form.showMedal, form.showWindow)
})


const emits = defineEmits<{
    (e: 'setRaise', danmuCount: number, entryEffectDirection: string, showAvatar: boolean, showMedal: boolean, showWindow: boolean): void
}>()



const connectRoom = () => {
    useFetch(AppSetting.VITE_API_URL + "/api/barrage/receive").post(JSON.stringify({
        connectionId: signalR.connectionId(),
        roomId: roomInfo.id
    }), 'application/json').json()
}

</script>
<style scoped lang="scss"></style>