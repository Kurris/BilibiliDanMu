<template>
    <div class='danMu'>
        <div>
            <el-avatar :size="avatarSize" :src="avatarUrl"></el-avatar>
            <span id="user" :class="{admin:isAdmin}">
                {{userName}}:
            </span>
            <span id="comment">
                {{comment}}
            </span>
        </div>
    </div>
</template>

<script>

export default {
    components: {},
    props: {
        comment: {
            type: String,
            required: true
        },
        isAdmin: {
            type: Boolean,
            default: false,
            required: false
        },
        userName: {
            type: String,
            required: true
        },
        avatarSize: {
            type: Number,
            default: 30
        },
        avatarUrl: {
            type: String,
            required: false,
            default: "https://cube.elemecdn.com/0/88/03b0d39583f48206768a7534e55bcpng.png"
        }
    },
    methods: {

    }
}
</script>
<style scoped>
.danMu {
    color: white;
}

#user {
    font-weight: bold;
    font-size: 12px;
}

#comment {
    padding-bottom: 5px;
    font-weight: bold;
}

.admin {
    color: red;
    font-weight: bold;
}
</style>