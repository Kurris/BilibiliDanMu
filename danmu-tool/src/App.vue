<template>
    <div id="app">
        <!-- <el-button type="primary" @click="add">添加</el-button>
        <el-button type="primary" @click="clear">清空</el-button> -->
        <div id="info">
            <div id="top">
                <div id="roomInfo">
                    观看过: {{watched}}
                </div>
                <div id="hot">
                    人气值: {{hot}}
                </div>
            </div>
            <div id="interact_word">
                {{interactWord}}
            </div>
            <div id="comment" ref="comment">
                <transition-group appear tag="ul">
                    <li v-for="item in comments" :key="item.key" class="item">
                        <dan-mu :isAdmin="item.isAdmin" :userName="item.userName" :comment="item.comment" :avatarUrl="item.faceUrl" />
                    </li>
                </transition-group>
            </div>
        </div>
    </div>
</template>

<script>
import DanMu from './components/DanMu.vue'
import Queue from './Queue.js'
const signalR = require('@microsoft/signalr')

const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/danmu")
    .configureLogging(signalR.LogLevel.Information)
    .build();


export default {
    name: 'App',
    components: {
        DanMu
    },
    data() {
        return {
            hot: 0,
            watched: 0,
            interactWord: '',
            comments: [],
            tempData: [],
            time: new Date(),
            queue: null,
            avatarUrl: "http://i2.hdslb.com/bfs/face/e4306a8bd5bfed253b660ed5b7680246e8402c08.jpg"
        }
    },
    methods: {
        clear() {
            this.comments = []
        },
        add() {
            this.queue.enqueue(this.count)
        },
        getSecondByDateSub(begin, end) {
            var beginDate = new Date(begin);
            var endDate = new Date(end);
            var diff = endDate.getTime() - beginDate.getTime();
            var sec = diff / 1000;
            return sec;
        }
    },
    mounted() {
        this.queue = new Queue()

        setInterval(() => {
            if (this.queue == null || this.queue.size == 0)
                return

            var item = this.queue.dequeue();

            //this.tempData.push(item)

            this.comments.push(item)
            this.time = new Date()
            if (this.comments.length > 15) {
                this.comments.shift()
            }

        }, 200);

        // setInterval(() => {
        //     console.log(this.comments.length);
        //     if (this.comments.length < 10) {
        //         if (this.getSecondByDateSub(this.time, new Date()) > 5) {
        //             this.comments.shift()
        //         }
        //     } else {

        //     }

        // }, 5000)

        //弹幕消息
        connection.on("addDanmu", p => {
            this.queue.enqueue(p)
        })

        connection.on("joinRoom", p => {
            this.interactWord = p
        })

        connection.on("watched", p => {
            this.watched = p
        })

        connection.on("hot", p => {
            this.hot = p
        })

        connection.start()
    }
}
</script>

<style>
#app {
    font-family: "黑体", "Microsoft YaHei", "黑体", "宋体", sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    overflow-y: hidden;
    overflow-x: hidden;
    overflow: hidden;
    color: white;
    font-size: 13px;
}

#info {
    height: 400px;
    width: 300px;
    position: absolute;
    top: 40%;
    left: 0%;
    transform: translate(1%, -50%);
}

#top {
    display: flex;
}
#hot {
    margin-left: 10px;
}

#interact_word {
    height: 20px;
}

.v-enter,
.v-lerve-to {
    opacity: 0;
    transform: translateY(10px);
}
.v-enter-active,
.v-lerve-active {
    transition: all 1s;
}
.v-move {
    transition: all 1s;
}
.v-leave-active {
    position: absolute;
}

li {
    list-style: none;
}

ul {
    padding: 0 0 0 0;
}
</style>
