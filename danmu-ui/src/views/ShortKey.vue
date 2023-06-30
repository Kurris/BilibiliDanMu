<template>
    <div id="shortKey">
        <RecordShortKey />
    </div>
</template>
<script setup lang="ts">
import { onBeforeMount } from 'vue';
import RecordShortKey from '../components/RecordShortKey.vue';


onBeforeMount(() => {
    // window.addEventListener('keydown', (e) => {
    //     e.preventDefault()
    // })
})


</script>
<style scoped lang="scss"></style>