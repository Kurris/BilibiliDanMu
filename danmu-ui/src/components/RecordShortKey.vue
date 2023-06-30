<template>
    <div id="recordShortKey">
        <!-- <div v-for="item in keys.current" :key="item">
            {{ item }}
        </div> -->

        <div class="shortKey-input" @mousedown="startOrStop" @mouseenter="hover" @mouseleave="hover">

            <div class="keyShow">
                <template v-for="item in values.slice(0, 3)" :key="item">
                    <ShortKeyItem :key-name="item" style="margin-right: 5px;" />
                </template>
            </div>

            <div class="message">
                <div class="kb">
                    <span v-if="isRecording">Stop</span>
                    <span v-else-if="isHover">Edit</span>
                    <KeyBoard v-else />
                </div>
            </div>
        </div>
    </div>
</template>
<script setup lang="ts">

import { useMagicKeys } from '@vueuse/core'
import { watch, ref, computed, onBeforeMount } from 'vue';
import ShortKeyItem from './ShortKeyItem.vue';
import KeyBoard from '../assets/KeyBoard.vue';

const tempPressKeys = ref<string[]>([])
const pressKeys = ref<string[]>([])
const values = computed(
    {
        get: () => {

            if (!isRecording.value) return pressKeys.value

            const current = Array.from(keys.current.values())
            if (keys.current.size <= tempPressKeys.value.length) {
                return tempPressKeys.value
            }
            return current
        },
        set: () => {
            pressKeys.value.splice(0, tempPressKeys.value.length)
            tempPressKeys.value.splice(0, tempPressKeys.value.length)
        }
    }
)

const keys = useMagicKeys({ reactive: true })

watch(keys.current, v => {

    if (!isRecording.value) return

    if (v.size == 0) {
        pressKeys.value.push(...tempPressKeys.value)
    }

    if (v.size > 0) {

        if (pressKeys.value.length > 0) {
            pressKeys.value.splice(0, pressKeys.value.length)
            tempPressKeys.value.splice(0, tempPressKeys.value.length)
        }

        const item = Array.from(keys.current.values())[v.size - 1]
        if (tempPressKeys.value.indexOf(item) < 0) {
            tempPressKeys.value.push(item)
        }
    }
})

const isRecording = ref(false)
const isHover = ref(false)

const startOrStop = () => {
    isRecording.value = !isRecording.value
}

const hover = () => {
    isHover.value = !isHover.value
}

onBeforeMount(() => {
    window.addEventListener('keydown', e => {
        if (isRecording.value) {
            e.preventDefault()
        }
    })
})

</script>
<style scoped lang="scss">
.shortKey-input {

    display: flex;
    align-items: center;
    justify-content: space-between;

    background-color: #2c2e32;
    border: 1.5px solid #1f2023;
    padding: 2px;
    margin: 2px;
    width: 350px;
    border-radius: 2%;
    transition: all 0.3s ease;

    &:hover {
        border: 1.5px solid #eb3e4c;
        cursor: pointer;

        .message {
            width: 120px;
        }
    }


    .keyShow {
        display: flex;
        align-items: center;
        margin-left: 3px;
    }

    .message {
        display: flex;
        justify-content: center;
        align-items: center;
        background-color: #6d6f78;
        color: white;
        font-size: 14.5px;
        font-weight: 600;
        width: 50px;
        border-radius: 2%;
        height: 36px;

        transition: all 0.3s ease;

        .kb {
            padding: {
                left: 5px;
                right: 5px;
            }
        }
    }
}
</style>