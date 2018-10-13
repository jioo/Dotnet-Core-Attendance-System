<template>
    <div id="app">
        <v-app dark>
            <app-header />
            <v-content>
                <v-slide-y-transition mode="out-in">
                    <router-view />
                </v-slide-y-transition>
                <notifications />
            </v-content>
            <app-footer />
        </v-app>
    </div>
</template>

<script>
import AppHeader from "@/views/_shared/app-header"
import AppFooter from "@/views/_shared/app-footer"

export default {
    name: "App",
    components: {
        AppHeader,
        AppFooter
    }
}
</script>

<style>
    .vue-notification {
        padding: 10px;
        margin: 5px 5px 0 5px !important;
        font-size: 14px;
    }
</style>
