<template>
    <div id="danmu">
        <div class="danmu">
            <div class=" header">
                <div style="display: flex;">
                    <div style="display: flex;">
                        <svg style="color: red;" xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                            fill="currentColor" class="bi bi-fire" viewBox="0 0 16 16">
                            <path
                                d="M8 16c3.314 0 6-2 6-5.5 0-1.5-.5-4-2.5-6 .25 1.5-1.25 2-1.25 2C11 4 9 .5 6 0c.357 2 .5 4-2 6-1.25 1-2 2.729-2 4.5C2 14 4.686 16 8 16Zm0-1c-1.657 0-3-1-3-2.75 0-.75.25-2 1.25-3C6.125 10 7 10.5 7 10.5c-.375-1.25.5-3.25 2-3.5-.179 1-.25 2 1 3 .625.5 1 1.364 1 2.25C11 14 9.657 15 8 15Z" />
                        </svg>
                        <div> : {{ state.hot }}</div>
                    </div>
                    <div style="margin-left: 20px;"> 观看人数: {{ state.watched }}</div>
                    <div style="margin-left: 20px;" v-if="state.interactWord != null">{{ state.interactWord }}</div>
                </div>
            </div>

            <div class="rows">
                <transition-group appear tag="ul" name="danmu" key="danmu">
                    <template v-for="item in state.comments.filter(x => x.mid != '')" :key="item.key">
                        <div :class="{ 'message': true }">
                            <div class="avatar-medal-name ">

                                <el-avatar :size="28" v-if="showAvatar"
                                    :src="item.faceUrl ?? 'http://i0.hdslb.com/bfs/face/member/noface.jpg'"></el-avatar>
                                <div style="color:red;border: 1px solid red; border-radius: 12%;" v-if="item.isAdmin">
                                    <span class="admin">房</span>
                                </div>
                                <div v-if="item.top3 > 0"
                                    style="font-size: 8px;border: 1px solid #ff5283 ;border-radius:12%; background-color: #ff5283;">
                                    <span>榜单 {{ item.top3 }}</span>
                                </div>
                                <template v-if="item.hasMedal && showMedal">
                                    <div class="medal">
                                        <span class="medal-name">{{ item.medalName }}</span>
                                        <span class="medal-lvl"> {{ item.medalLevel }}</span>
                                    </div>
                                </template>
                                <div class="name">{{ item.userName }}</div>
                            </div>

                            <div v-html="item.comment" :class="{ 'comment': true, 'comment-bg-color': item.color != '' }">
                            </div>
                        </div>
                    </template>
                </transition-group>
            </div>
        </div>
        <div class="join-room">

        </div>
        <div class="entryEffect">
            <transition-group appear tag="ul" name="entry" key="entry">
                <template v-for="item in state.entryEffects" :key="item.uid">
                    <EntryEffect :face="item.face" :backgroundUrl="item.web_basemap_url" :msg="item.copy_writing" />
                </template>
            </transition-group>
        </div>

    </div>
</template>
<script setup lang="ts">
import { onBeforeMount, reactive, ref, watch } from 'vue'
import { HubConnectionBuilder, LogLevel, HubConnectionState } from '@microsoft/signalr'
import Queue from '../utils/queue.js'
import EntryEffect from './EntryEffect.vue'


const props = defineProps<{
    roomId?: number,
    danmuCount: number,
    entryEffectDirection: string,
    showAvatar: boolean,
    showMedal: boolean
}>()

const direction = ref('translateX(-230px)');


watch(() => props.entryEffectDirection, (newVal) => {
    if (newVal == 'left') {
        direction.value = 'translateX(-230px)'
    } else if (newVal == 'right') {
        direction.value = 'translateX(230px)'
    } else if (newVal == 'bottom') {
        direction.value = 'translateY(130px)'
    }
})


const connection = new HubConnectionBuilder()
    .withUrl("http://localhost:5000/danmu")
    .configureLogging(LogLevel.Debug)
    .withAutomaticReconnect()
    .build();


const count = ref(7);


const state = reactive({
    hot: 0,
    watched: 0,
    interactWord: '',
    comments: [{
        mid: '',
        faceUrl: '',
        comment: '',
        isAdmin: false,
        time: new Date(),
        userName: '',
        audRank: 0,
        hasMedal: false,
        medalName: '',
        medalLevel: 0,
        top3: 0,
        color: '',
        key: '',
    }],
    tempData: [],
    time: new Date(),
    queue: new Queue(),
    entryEffects: [],
    entryEffectQueue: new Queue(),
    sc: null
});


const emits = defineEmits<{
    (e: 'onSc', msg: any): void
}>()


const connectRoom = () => {
    if (props.roomId != null) {
        if (connection.state == HubConnectionState.Connected) {
            state.comments.splice(0, state.comments.length)
            state.queue.clear()
            state.entryEffects.splice(0, state.entryEffects.length)
            state.entryEffectQueue.clear()
            connection.invoke("Start", props.roomId).catch(err => {
                console.log(err)
            })
            return true;
        }
    }
}


defineExpose({
    connectRoom
})

onBeforeMount(() => {

    //弹幕设置
    setInterval(() => {
        var item = state.queue.dequeue();
        if (item == null) {
            return
        }

        state.comments.push(item)
        state.time = new Date()
        if (state.comments.length > props.danmuCount) {
            state.comments.shift()
        }

    }, 300);

    //舰长
    setInterval(() => {
        var item = state.entryEffectQueue.dequeue();
        if (item == null) {
            return
        }

        state.entryEffects.push(item)
        count.value = 7
        if (state.entryEffects.length > 3) {
            state.entryEffects.shift()
        }

    }, 300);

    //处理进入房间信息
    setInterval(() => {
        state.interactWord = null
    }, 3000);


    setInterval(() => {
        if (state.entryEffectQueue.isEmpty && state.entryEffects.length > 0 && count.value <= 0) {
            state.entryEffects.shift()
        }

        if (count.value != 0) {
            count.value--
        }

    }, 1000);

    //弹幕消息
    connection.on("addDanmu", p => {
        state.queue.enqueue(p)
    })

    connection.on("joinRoom", p => {
        state.interactWord = p
    })

    connection.on("watched", p => {
        state.watched = p
    })

    connection.on("hot", p => {
        state.hot = p
    })

    connection.on("sc", p => {
        state.sc = p
        emits('onSc', p)
    })

    connection.on("entry_effect", p => {
        let item = JSON.parse(p)
        item.copy_writing = item.copy_writing.replace('<%', '<span style="color:yellow">')
        item.copy_writing = item.copy_writing.replace('%>', '</span>')

        state.entryEffectQueue.enqueue(item)
    })
    //重连或者首次
    connection.onreconnected(id => {
        console.log(id);

    })
    //无法连接
    connection.onclose(err => {
        console.log(err);

    })
    connection.start();

})

</script>
<style scoped lang="scss">
.danmu {
    min-width: 30vh;
    text-overflow: ellipsis;

    .header {

        margin-bottom: 20px;

        text-shadow: 0 0 2px hsl(40, 28.57%, 28.82%), 0 0 2px hsl(40, 28.57%, 28.82%), 0 0 2px hsl(40, 28.57%, 28.82%);
        // /*多个叠加*/
        font-size: 12.5px;
        font-weight: bolder;
        color: white;
    }

    .rows {
        font-size: 14.6px;
        color: white;
        display: flex;
        flex-direction: column;

        .message {
            display: flex;
            align-items: center;

            .avatar-medal-name {
                font-size: 13px;
                white-space: nowrap;
                display: flex;
                align-items: center;

                .medal {
                    padding: 1.2px;
                    border-radius: 12%;

                    .medal-name {
                        border: 1px solid orange;
                        background-color: orange;

                        padding: {
                            left: 3px;
                            right: 3px;
                        }
                    }

                    .medal-lvl {
                        border: 1px solid orange;
                        font-weight: bolder;

                        padding: {
                            left: 3px;
                            right: 3px;
                        }
                    }
                }

                .name {
                    margin-right: 3.5px;

                    padding: {
                        left: 3px;
                        right: 3px;
                    }

                    background-color: #9acfd9;
                    border-radius: 10%;
                    color: black;
                }
            }

            .comment {

                display: flex;
                align-items: center;

                text-shadow: 0 0 2px hsl(40, 28.57%, 28.82%), 0 0 2px hsl(40, 28.57%, 28.82%), 0 0 2px hsl(40, 28.57%, 28.82%);
                // /*多个叠加*/
                font-size: 14.5px;
                font-weight: bolder;
            }
        }
    }
}

.comment-bg-color {
    background-color: rgba($color: #a5cbfc, $alpha: 0.4);

    margin: {
        left: 5px;
    }

    border-radius: 10%;
}

.entryEffect {
    position: absolute;
    top: 20px;
    left: 0;
}

.danmu-move {
    transition: all 0.5s ease;
}

.danmu-enter-active,
.danmu-leave-active {
    position: absolute;
    transition: all 0.5s ease;
}

.danmu-enter-from {
    opacity: 0;
    transform: translateY(30px);
}

.danmu-leave-to {
    opacity: 0;
    transform: translateY(-30px);
}


.entry-move {
    transition: all 0.5s ease;
}

.entry-enter-active,
.entry-leave-active {
    transition: all 2s ease;
}

.entry-enter-from {
    opacity: 0;
    transform: v-bind(direction);
}

.entry-leave-to {
    opacity: 0;
    transform: v-bind(direction);
}


li {
    list-style: none;
}

ul {
    padding: 0 0 0 0;
}
</style>