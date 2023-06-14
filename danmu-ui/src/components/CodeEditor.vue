<template>
    <Codemirror v-model:value="code" :options="cmOptions" border ref="cmRef" height="400" width="600" @input="onInput"
        @ready="onReady">
    </Codemirror>
</template>
<script lang="ts" setup>
import { ref, onMounted, onUnmounted } from "vue"
import "codemirror/mode/javascript/javascript.js"
import Codemirror from "codemirror-editor-vue3"
import type { CmComponentRef } from "codemirror-editor-vue3"
import type { Editor, EditorConfiguration } from "codemirror"
// language
import "codemirror/mode/javascript/javascript.js";
// placeholder
import "codemirror/addon/display/placeholder.js";
// theme
import "codemirror/theme/dracula.css";

const code = ref(
    `var i = 0;
  for (; i < 9; i++) {
      console.log(i);
      // more statements
  }
  `
)
const cmRef = ref<CmComponentRef>()
const cmOptions: EditorConfiguration = {
    mode: "text/javascript",
    theme: 'dracula'
}


const onInput = (val: string) => {
    console.log(val)
}

const onReady = (cm: Editor) => {
    console.log(cm.focus())
}

onUnmounted(() => {
    cmRef.value?.destroy()
})
</script>
  