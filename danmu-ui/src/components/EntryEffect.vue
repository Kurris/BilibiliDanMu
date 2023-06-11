<template>
    <div class="entry-effect" v-if="backgroundUrl != ''">
        <img class="bg" referrerpolicy="no-referrer" width="380" height="90" :src="backgroundUrl" />
        <div class="container">
            <el-avatar v-if="face" class="face" :size="28" :src="face"></el-avatar>
            <!-- <el-avatar style="position:absolute;" :size="30"
                src="https://i0.hdslb.com/bfs/live/80f732943cc3367029df65e267960d56736a82ee.png">
            </el-avatar> -->

            <div class="entry-effect-name" v-html="msg"></div>
        </div>
    </div>
</template>

<script setup lang="ts">

defineProps<{
    face: string,
    msg: string,
    backgroundUrl: string
}>()


// {
//                 "level": 1,
//                 "img": "https://i0.hdslb.com/bfs/live/39164ebfdd39db3d284b1221765e7e57f5a49958.png",
//                 "name": "总督"
//             },
//             {
//                 "level": 2,
//                 "img": "https://i0.hdslb.com/bfs/live/09937c3beb0608e267a50ac3c7125c3f2d709098.png",
//                 "name": "提督"
//             },
//             {
//                 "level": 3,
//                 "img": "https://i0.hdslb.com/bfs/live/80f732943cc3367029df65e267960d56736a82ee.png",
//                 "name": "舰长"
//             }

</script>
<style scoped lang="scss">
.entry-effect {
    display: flex;
    align-items: center;
    margin-top: 10px;


    .bg {
        position: absolute;
    }

    .container {

        display: flex;
        align-items: center;
        margin-top: 16px;
        margin-left: 10px;
        z-index: 200;

        .face {
            margin-left: 10px;
        }

        .entry-effect-name {
            font-size: 12px;
            margin-left: 5px;
            color: white;
        }
    }
}
</style>