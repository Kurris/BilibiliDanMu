<template>
    <div id="danmu">

        <div class="danmu">


            <div class="header">
                <div class="interactWord" v-if="signalR.data.interactWord != null">{{
                    signalR.data.interactWord.userName }}
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
        if (signalR.data.entryEffects.length > 0 && signalR.data.entryEffectQueue.isEmpty) {
            if (count.value <= 0) {
                signalR.data.entryEffects.shift()
            }
        }

        if (count.value != 0) {
            count.value--
        }

    }, 1000);
})

</script>
<style scoped lang="scss">
.danmu {
    padding: 15px;

    .header {
        height: 12.5px;
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
    transition: all 1.2s ease;
}

.entry-enter-active,
.entry-leave-active {
    position: absolute;
    transition: all 1.2s ease;
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