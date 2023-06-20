<template>
    <div class="gift">
        <template v-for="item in signalR.data.gifts" :key="item.key">
            <el-image style="width: 100px; height: 100px" :src="item.gifUrl" fit='fill' />
            <div>
                {{ item.from }} 投喂 {{ item.giftName }} x{{ item.num }}
            </div>
        </template>
    </div>
</template>
<script setup lang="ts">
import { useSignalR } from '../stores/signalRStore';

const signalR = useSignalR()
</script>
<style scoped lang="scss">
.gift {
    color: #ae93ea;
    background-color: transparent;
    text-align: center;

    letter-spacing: 0.2rem;
    font-size: 1rem;

    background-image: -webkit-linear-gradient(left, #147B96, #E6D205 25%, #147B96 50%, #E6D205 75%, #147B96);
    -webkit-text-fill-color: transparent;
    font-weight: bolder;
    -webkit-background-clip: text;
    -webkit-background-size: 200% 100%;

    transition: all 0.3 ease;
}
</style>