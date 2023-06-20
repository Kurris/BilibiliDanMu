import { type IStreamerInfo } from '../type.d/bilibilidata'
import { defineStore } from 'pinia'




export const useStreamer = defineStore('streamer', () => {

    const info: IStreamerInfo = {
        uid: 0,
        userName: '',
        face: '',
        gender: 0,
        level: 0,
        followerNum: 0,
        pendant: '',
        boardMessage: '',
        roomInfo: {
            uid: 0,
            roomId: 0,
            shortRoomId: 0,
            title: '',
            tags: undefined,
            description: '',
            areaId: 0,
            areaName: '',
            parentAreaId: 0,
            parentAreaName: '',
            keyFrameUrl: '',
            userCoverUrl: '',
            backgroundUrl: '',
            liveTime: '',
            liveStatus: 0,
            key: ''
        }
    }

    return { info }
})
