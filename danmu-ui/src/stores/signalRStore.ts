import { defineStore } from 'pinia'
import { HttpTransportType, HubConnectionBuilder, HubConnectionState, LogLevel, } from '@microsoft/signalr'
import { AppSetting } from '@/utils/appSetting';
import Queue from '@/utils/queue';
import { reactive, h } from 'vue';
//暂时不管
import { ElNotification } from 'element-plus'


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
        entryEffectQueue: new Queue()
    });


    const clearData = () => {
        data.comments.splice(0, data.comments.length)
        data.queue.clear()
        data.entryEffects.splice(0, data.entryEffects.length)
        data.entryEffectQueue.clear()

        data.hot.hot = 0;
        data.watched.num = 0;
        data.interactWord.userName = ''
    }

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

    connection.on("SUPER_CHAT_MESSAGE", (p: any) => {
        scNotification(p)
        const u = new SpeechSynthesisUtterance(p.speakText)
        u.lang = 'zh-CN'
        u.rate = 0.8
        window.speechSynthesis.speak(u)
    })

    connection.on("ENTRY_EFFECT", p => {
        data.entryEffectQueue.enqueue(p)
    })

    connection.on("SEND_GIFT", p => {
        data.gifts.shift()
        data.gifts.push(p)
    })


    connection.onreconnecting(() => {
        clearData()
    })

    //重连或者首次
    connection.onreconnected(id => {
        console.log(id);
    })

    //无法连接
    connection.onclose(() => {
        clearData()
    })


    const scNotification = (data: any) => {

        ElNotification({
            dangerouslyUseHTMLString: true,
            duration: 1000 * 20,
            showClose: true,
            message: h({
                template: ` 
          <div style='width:300px;background-color: ${data.backgroundColor};font-size: 10px;'>
      
            <div style="height:40px;">
              <div style="padding-left:10px;padding-top:2px">
                <img referrer="no-referrer" src="${data.userFace}" width="35" height="35" style="position:absolute;border-radius:50%;"/>
                <img v-if="'${data.userFaceFrame}'!=''" referrer="no-referrer" src="${data.userFaceFrame}" width="35" height="35" style="position:absolute;"/>
              </div>
            
              <div style="margin-left:60px;display: flex;align-items: center;padding-top:8px;">
                <div v-if="${data.hasMedal}"  style="display: flex;justify-content: center;align-items: center;background-color:orange;border-radius: 12%;">
                    <div style="color:${data.medalColor};">${data.medalName}</div>
                    <div style="color:white;margin-left:5px;margin-right:5px;">${data.medalLevel}</div>
                </div>
                <div style="color='${data.userNameColor}';margin-left:10px">${data.userName}</div>
              </div>
      
              <img src="${data.backgroundImage}" width="100" height="40" style="position:absolute;top:0;;right:-8px;"/>
              <div style="color:${data.backgroundPriceColor};position:absolute;top:8px;right:25px;">
                  ${data.priceString}
              </div>
            </div>
            <div style="background-color: ${data.backgroundBottomColor};padding: 10px;color: ${data.messageFontColor};font-size: 10px;">
                ${data.message}
            </div>
          </div>
          `
            })
        })
    }

    const connectionId = () => connection.connectionId
    const connected = () => connection.state == HubConnectionState.Connected;
    const disconnected = () => connection.state == HubConnectionState.Disconnected;
    const start = () => connection.start()




    return { connectionId, start, connected, disconnected, data }
})
