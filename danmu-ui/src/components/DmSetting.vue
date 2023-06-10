<template>
    <el-form :model="form" label-width="120px">
        <el-form-item label="房间号">
            <div style="display: flex;">
                <el-input-number v-model="form.roomId" />
                <el-button @click="connectRoom" type="warning" style="margin-left: 20px;">连接房间</el-button>
            </div>
        </el-form-item>
        <el-form-item label="弹幕数显示">
            <el-input-number type="number" v-model="form.danmuCount" />
        </el-form-item>
        <el-form-item label="弹窗提醒方向">
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
        <!-- <el-form-item label="Activity zone">
                <el-select v-model="form.region" placeholder="please select your zone">
                    <el-option label="Zone one" value="shanghai" />
                    <el-option label="Zone two" value="beijing" />
                </el-select>
            </el-form-item>
            <el-form-item label="Activity time">
                <el-col :span="11">
                    <el-date-picker v-model="form.date1" type="date" placeholder="Pick a date" style="width: 100%" />
                </el-col>
                <el-col :span="2" class="text-center">
                    <span class="text-gray-500">-</span>
                </el-col>
                <el-col :span="11">
                    <el-time-picker v-model="form.date2" placeholder="Pick a time" style="width: 100%" />
                </el-col>
            </el-form-item>
        
            <el-form-item label="Activity type">
                <el-checkbox-group v-model="form.type">
                    <el-checkbox label="Online activities" name="type" />
                    <el-checkbox label="Promotion activities" name="type" />
                    <el-checkbox label="Offline activities" name="type" />
                    <el-checkbox label="Simple brand exposure" name="type" />
                </el-checkbox-group>
            </el-form-item>

            <el-form-item label="Activity form">
                <el-input v-model="form.desc" type="textarea" />
            </el-form-item> -->
    </el-form>
</template>
<script setup lang="ts">
import { reactive, watch } from 'vue'

const form = reactive({
    roomId: 6750632,
    danmuCount: 10,
    region: '',
    date1: '',
    date2: '',
    delivery: false,
    type: [],
    entryEffectDirection: 'left',
    desc: '',
    showAvatar: true,
    showMedal: true,
    showWindow: true,
})

watch(() => JSON.stringify(form), (newValue) => {
    // console.log(newValue);
    emits('setRaise', form.danmuCount, form.entryEffectDirection, form.showAvatar, form.showMedal, form.showWindow)
})


const emits = defineEmits<{
    (e: 'connectRoom', roomId: number): number,
    (e: 'setRaise', danmuCount: number, entryEffectDirection: string, showAvatar: boolean, showMedal: boolean, showWindow: boolean): void
}>()



const connectRoom = () => {
    emits('connectRoom', form.roomId)
}


</script>
<style scoped lang="scss"></style>