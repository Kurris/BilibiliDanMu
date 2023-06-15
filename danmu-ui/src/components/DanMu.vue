<template>
    <div id="danmu">

        <div class="danmu">
            <div class="header">
                <div style="display: flex;">
                    <div style="display: flex;">
                        <svg style="color: red;" xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                            fill="currentColor" class="bi bi-fire" viewBox="0 0 16 16">
                            <path
                                d="M8 16c3.314 0 6-2 6-5.5 0-1.5-.5-4-2.5-6 .25 1.5-1.25 2-1.25 2C11 4 9 .5 6 0c.357 2 .5 4-2 6-1.25 1-2 2.729-2 4.5C2 14 4.686 16 8 16Zm0-1c-1.657 0-3-1-3-2.75 0-.75.25-2 1.25-3C6.125 10 7 10.5 7 10.5c-.375-1.25.5-3.25 2-3.5-.179 1-.25 2 1 3 .625.5 1 1.364 1 2.25C11 14 9.657 15 8 15Z" />
                        </svg>
                        <div> : {{ signalR.data.hot.hot }}</div>
                    </div>
                    <div style="margin-left: 20px;"> 观看人数: {{ signalR.data.watched.num }}</div>
                    <div class="interactWord" v-if="signalR.data.interactWord != null">{{
                        signalR.data.interactWord.userName }}
                    </div>
                </div>
            </div>

            <div class="rows">
                <transition-group appear tag="ul" name="danmu">
                    <template v-for="item in signalR.data.comments.filter(x => x.mid != '')" :key="item.key">
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

                            <div v-html="item.comment" class="comment" :style="{ 'color': item.guard.fontColor }" />
                        </div>
                    </template>
                </transition-group>
            </div>
        </div>


        <div class="entryEffect">
            <transition-group appear tag="ul" name="entry">
                <template v-for="item in signalR.data.entryEffects" :key="item.key">
                    <EntryEffect :face="item.face" :backgroundUrl="item.baseImageUrl" :msg="item.message" />
                </template>
            </transition-group>
        </div>


    </div>
</template>
<script setup lang="ts">
import { onBeforeMount, ref, watch } from 'vue'
import EntryEffect from './EntryEffect.vue'
import { useSignalR } from '../stores/signalRStore';


const signalR = useSignalR()

const props = defineProps<{
    danmuCount: number,
    entryEffectDirection: string,
    showAvatar: boolean,
    showMedal: boolean
}>()

const direction = ref('translateX(-130px)');


watch(() => props.entryEffectDirection, (newVal) => {
    if (newVal == 'left') {
        direction.value = 'translateX(-130px)'
    } else if (newVal == 'right') {
        direction.value = 'translateX(130px)'
    } else if (newVal == 'bottom') {
        direction.value = 'translateY(130px)'
    }
})



const count = ref(7);



onBeforeMount(() => {

    //弹幕设置
    setInterval(() => {
        var item = signalR.data.queue.dequeue();
        if (item == null) {
            return
        }

        signalR.data.comments.push(item)
        if (signalR.data.comments.length > props.danmuCount) {
            const i = signalR.data.comments.length - props.danmuCount
            for (let index = 0; index < i; index++) {
                signalR.data.comments.shift()
            }
        }

    }, 300);

    //舰长进去
    setInterval(() => {
        var item = signalR.data.entryEffectQueue.dequeue();
        if (item == null) {
            return
        }

        signalR.data.entryEffects.push(item)
        count.value = 7
        if (signalR.data.entryEffects.length > 3) {
            signalR.data.entryEffects.shift()
        }
    }, 300);


    setInterval(() => {
        if (signalR.data.entryEffectQueue.isEmpty && signalR.data.entryEffects.length > 0 && count.value <= 0) {
            signalR.data.entryEffects.shift()
        }

        if (count.value != 0) {
            count.value--
        }

    }, 1000);

    //处理进入房间信息
    setInterval(() => {
        signalR.data.interactWord = null
    }, 3000);

})

</script>
<style scoped lang="scss">
.danmu {
    padding: 15px;

    .header {
        white-space: nowrap;
        margin-bottom: 20px;

        text-shadow: 0 0 2px hsl(40, 28.57%, 28.82%), 0 0 2px hsl(40, 28.57%, 28.82%), 0 0 2px hsl(40, 28.57%, 28.82%);
        font-size: 12.5px;
        font-weight: bolder;
        color: white;

        .interactWord {
            margin-left: 20px;
            text-overflow: ellipsis;
        }
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
                vertical-align: middle;
                text-shadow: 0 0 2px hsl(40, 28.57%, 28.82%), 0 0 2px hsl(40, 28.57%, 28.82%), 0 0 2px hsl(40, 28.57%, 28.82%);
                font-size: 14.5px;
                font-weight: bolder;
            }
        }
    }
}

.entryEffect {
    position: absolute;
    top: 50px;
}


.danmu-move {
    transition: all 0.3s ease;
}

.danmu-enter-active,
.danmu-leave-active {
    position: absolute;
    transition: all 0.3s ease;
}

.danmu-enter-from {
    opacity: 0;
    transform: translateX(-30px);
}

.danmu-leave-to {
    opacity: 0;
    transform: translateX(-30px);
}



.entry-move {
    transition: all 0.3s ease;
}

.entry-enter-active,
.entry-leave-active {
    position: absolute;
    transition: all 0.3s ease;
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