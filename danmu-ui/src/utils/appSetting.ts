const config = {
    VITE_SIGNALR_URL: import.meta.env.VITE_SIGNALR_URL as string,
    VITE_API_URL: import.meta.env.VITE_API_URL as string,
}

//dev输出配置
if (import.meta.env.DEV) {
    console.log('application setting :\r\n' + JSON.stringify(config, null, 4))
}
export const AppSetting = config