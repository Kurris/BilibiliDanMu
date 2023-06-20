export interface IStreamerInfo {
    uid: number,
    userName: string,
    face: string,
    gender: number,
    level: number,
    followerNum: number,
    pendant: string,
    boardMessage: string,
    roomInfo: IRoomInfo
}

export interface IRoomInfo {
    uid: number,
    roomId: number,
    shortRoomId: number,
    title: string,
    tags: Array,
    description: string,
    areaId: number,
    areaName: string,
    parentAreaId: number,
    parentAreaName: string,
    keyFrameUrl: string,
    userCoverUrl: string,
    backgroundUrl: string,
    liveTime: string,
    liveStatus: number,
    key: string
}

