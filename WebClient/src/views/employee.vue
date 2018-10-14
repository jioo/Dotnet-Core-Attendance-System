<template lang="html">
    <v-container fluid class="pt-0">
        <v-layout align-center justify-center>
            <v-slide-x-transition mode="out-in">
                <router-view />
            </v-slide-x-transition>
        </v-layout>
    </v-container>
</template>