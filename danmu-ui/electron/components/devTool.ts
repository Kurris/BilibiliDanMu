import type { BrowserView } from "electron"

const useDevTool = (win: BrowserView) => {
    win.webContents.openDevTools({ mode: 'right' })
}

export { useDevTool }