<template>
    <div>

        <div class="preview-bg" :style="{ 'height': props.height + 'px', 'width': props.width + 'px' }">
            <slot />
        </div>

    </div>
</template>
<script setup lang="ts">


const props = defineProps<{
    height: number,
    width: number
}>()

</script>
<style scoped lang="scss"></style>