<template>
    <div id="app">
        <v-app dark>
          <app-header />
          <v-content>
              <!-- <transition name="fade-transition"> -->
                  <router-view />
              <!-- </transition> -->
              <notifications />
          </v-content>
          <v-footer app>
            <span>&copy; 2018</span>
          </v-footer>
        </v-app>
    </div>
</template>

<script>
import AppHeader from '@/views/app-header'

export default {
    name: 'App',
    components: {
        AppHeader
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

