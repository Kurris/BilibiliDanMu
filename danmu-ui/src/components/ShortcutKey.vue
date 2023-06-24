<template>
    <div id="short-key" v-if="props.keyName">
        <div class="shadow">
            <div class="content" @mousedown="md" @mouseup="mu" @mouseleave="mu">
                {{ props.keyName }}
            </div>
        </div>
        <div v-for="item in keys.current" :key="item">
            <!-- {{ item }} -->
        </div>
    </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useMagicKeys } from '@vueuse/core'

const props = defineProps<{
    keyName: string
}>()

const keys = useMagicKeys({ reactive: true })

const shadow = ref('unset')
const transformY = ref('-4px')
const border = ref('unset')


const md = () => {
    transformY.value = '-1px'
    shadow.value = '0px 1.5px 0px 0px #313338'
    border.value = '0.5px solid #1e1f22'
}
const mu = () => {
    transformY.value = '-4px'
    shadow.value = 'unset'
    border.value = 'unset'
}


</script>
<style scoped lang="scss">
#short-key {
    margin: 0;
    text-transform: uppercase;
    font-size: 12px;

    .shadow {
        width: fit-content;
        background-color: #1e1f22;
        font-weight: 600;
        border-radius: 4px;
        margin: 0.5px;
        border: v-bind(border);
        transform: translateY(2px);

        .content {
            color: #F9F9F9;
            background-color: #41434a;
            border-radius: 4px;
            min-width: 14px;
            min-width: 14px;
            text-align: center;
            line-height: 12px;

            padding: {
                top: 3px;
                left: 5px;
                bottom: 6px;
                right: 6px;
            }

            box-shadow: v-bind(shadow);
            transform: translateY(v-bind(transformY));
        }
    }


}
</style>