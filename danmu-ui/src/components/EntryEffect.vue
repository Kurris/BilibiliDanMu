<template>
    <div class="entry-effect" v-if="backgroundUrl != ''">
        <img class="bg" referrerpolicy="no-referrer" width="380" height="90" :src="backgroundUrl" />
        <div class="container">
            <el-avatar v-if="face" class="face" :size="28" :src="face"></el-avatar>
            <div class="entry-effect-name" v-html="msg"></div>
        </div>
    </div>
</template>

<script setup lang="ts">

defineProps<{
    face: string,
    msg: string,
    backgroundUrl: string
}>()

</script>
<style scoped lang="scss">
.entry-effect {
    background-size: 100% 100%;
    background-repeat: no-repeat;

    display: flex;
    align-items: center;
    margin-top: 10px;


    .bg {
        position: absolute;
    }

    .container {

        display: flex;
        align-items: center;
        margin-top: 16px;
        z-index: 200;

        .face {
            margin-left: 10px;
        }

        .entry-effect-name {
            font-size: 12px;
            margin-left: 5px;
            color: white;
        }
    }
}
</style>