module.exports = {
	pluginOptions: {
		electronBuilder: {
			builderOptions: {
				appId: 'danmuku-1',
				productName: 'danmuku', //项目名
				// files: ['build/icons/*', 'src/**/*', 'main.js', 'preload.js', 'renderer.js', 'index.html', 'node_modules/**/*'],
				copyright: 'Copyright © 22', //版权信息
				directories: {
					output: './dist', //输出文件路径
				},
				win: {
					//win相关配置
					icon: './dist_electron/icons/small.ico', //图标，当前图标在根目录下，注意这里有两个坑
					target: [
						{
							target: 'nsis', //利用nsis制作安装程序
							arch: [
								'x64', //64位
								'ia32', //32位
							],
						},
					],
				},
			},
		},
	},
}
