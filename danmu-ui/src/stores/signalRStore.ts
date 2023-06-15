import { defineStore } from 'pinia'
import { HttpTransportType, HubConnectionBuilder, HubConnectionState, LogLevel, } from '@microsoft/signalr'
import { AppSetting } from '@/utils/appSetting';
import Queue from '@/utils/queue';
import { reactive, nextTick, ref } from 'vue';

export const useSignalR = defineStore('signalr', () => {

    const connection = new HubConnectionBuilder()
        .withUrl(AppSetting.VITE_SIGNALR_URL, {
            transport: HttpTransportType.WebSockets
        })
        .configureLogging(LogLevel.Debug)
        .withAutomaticReconnect()
        .build();

    const data = reactive({
        hot: {
            hot: 0
        },
        watched: {
            num: 0
        },
        interactWord: {
            userName: ''
        },
        comments: [{
            mid: '',
            faceUrl: '',
            comment: '',
            isAdmin: false,
            userName: '',
            audRank: 0,
            hasMedal: false,
            medalName: '',
            medalLevel: 0,
            top3: 0,
            time: new Date(),
            guard: {
                level: 0,
                name: '',
                frameUrl: '',
                fontColor: '',
                fansMedalIconUrl: '',
                starFrameUrl: ''
            },
            key: '',
        }],
        gifts: new Array<any>(),
        tempData: [],
        queue: new Queue(),
        entryEffects: [],
        entryEffectQueue: new Queue(),
        sc: null
    });


    //弹幕消息
    connection.on("DANMU_MSG", p => {
        data.queue.enqueue(p)
    })

    connection.on("INTERACT_WORD", p => {
        data.interactWord = p
    })

    connection.on("WATCHED_CHANGE", p => {
        data.watched = p
    })

    connection.on("HOT", p => {
        data.hot = p
    })

    connection.on("SUPER_CHAT_MESSAGE", p => {
        data.sc = p
    })

    connection.on("ENTRY_EFFECT", async p => {
        data.entryEffectQueue.enqueue(p)

        await nextTick(async () => {
            const task = new Promise(() => {
                const u = new SpeechSynthesisUtterance(p.speakText)
                u.lang = 'zh-CN'
                u.rate = 0.8
                window.speechSynthesis.speak(u)
            })
            await task;
        })
    })

    connection.on("SEND_GIFT", p => {
        data.gifts.shift()
        data.gifts.push(p)
    })



    //重连或者首次
    connection.onreconnected(id => {

    })

    //无法连接
    connection.onclose(err => {
        console.log(err);
    })


    const connectionId = () => connection.connectionId
    const connected = () => connection.state == HubConnectionState.Connected;
    const disconnected = () => connection.state == HubConnectionState.Disconnected;

    const start = () => connection.start()


    // /**
    //  * 
    //  * @param roomId 房间id(支持短号)
    //  * @returns 
    //  */
    // const connectBLiveRoom = (roomId: number) => {

    //     if (connected()) {

    //         data.comments.splice(0, data.comments.length)
    //         data.queue.clear()
    //         data.entryEffects.splice(0, data.entryEffects.length)
    //         data.entryEffectQueue.clear()

    //         return connection.invoke("Start", roomId)
    //     }
    // }

    return { connectionId, start, connected, disconnected, data }
})
