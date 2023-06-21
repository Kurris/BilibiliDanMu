import { defineStore } from 'pinia'
import type { IGameInfo, IMusicInfo } from '@/type.d/client'

export const useMediaInfo = defineStore('mediaInfo', () => {

    const game: IGameInfo = {
        title: '',
        name: '',
        path: '',
        image: '',
        useOverlay: false
    }

    const music: IMusicInfo = {
        title: '',
        image: ''
    }

    const isforeground = false

    return { game, music, isforeground }
})
