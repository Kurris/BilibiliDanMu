import path from "path";
import { Tray, Menu, nativeImage } from "electron";
import { appName } from "../consts";

let tray: Tray

const createTray = (click: () => void, overlayChanged: () => void, quit: () => void) => {
    tray = new Tray(nativeImage.createFromPath(path.join(__dirname, '../electron/assets/app-24.png')))

    const contextMenu = Menu.buildFromTemplate([
        {
            label: '始终覆盖', type: 'checkbox', click: () => {
                // isManualSetCover = !isManualSetCover
                // if (process.platform == 'darwin') {
                //   //苹果电脑setFullScreen会切换到另一个子屏幕
                //   overlayWindow.setSimpleFullScreen(!cover)
                // } else {
                //   overlayWindow.setFullScreen(!cover)
                //   overlayWindow.setSkipTaskbar(!cover);
                // }
                overlayChanged()
            }
        },
        {
            label: '退出', click: () => {
                quit()
            }
        }
    ])

    tray.setToolTip(appName)
    tray.setContextMenu(contextMenu)
    tray.on('click', () => {
        click()
    })
}

export default createTray;

