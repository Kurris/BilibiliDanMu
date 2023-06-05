<template>
    <div class="entry-effect" v-if="backgroundUrl != ''" :style="{ 'background-image': `url(${backgroundUrl})` }">
        <el-avatar v-if="face" class="face" :size="28" :src="face"></el-avatar>
        <div class="entry-effect-name" v-html="msg"></div>
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
    height: 80px;
    width: 350px;
    background-size: 100% 100%;
    background-repeat: no-repeat;

    display: flex;
    align-items: center;

    z-index: 200;

    .face {
        margin-left: 20px;
        margin-top: 16px;
    }

    .entry-effect-name {
        font-size: 12px;
        margin-left: 5px;
        margin-top: 16px;
        color: white;
    }
}
</style>