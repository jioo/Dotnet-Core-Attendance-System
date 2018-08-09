<template>
    <div id="app">
        <v-app dark>
            <app-header />
            <v-content>
                <router-view />
                <notifications />
            </v-content>
            <app-footer />
            <!-- <v-footer app>
                <span>&copy; 2018</span>
            </v-footer> -->
        </v-app>
    </div>
</template>

<script>
import AppHeader from '@/views/app-header'
import AppFooter from '@/views/app-footer'

export default {
    name: 'App',
    components: {
        AppHeader,
        AppFooter
    }
}
</script>

<style>
    .vue-notification {
        padding: 10px;
        margin: 12px 5px 0 5px;
        font-size: 14px;
    }
</style>

