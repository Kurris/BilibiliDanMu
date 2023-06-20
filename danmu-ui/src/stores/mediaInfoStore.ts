import { defineStore } from 'pinia'
import { ref } from 'vue'
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

    return { game, music }
})
