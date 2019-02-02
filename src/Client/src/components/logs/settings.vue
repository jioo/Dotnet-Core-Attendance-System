<template>
    <v-flex md12 class="pt-4">
        <v-card class="elevation-12">
            <v-toolbar dark color="orange">
                <v-btn icon :loading="isLoading" :to="{ name: 'logs' }">
                    <v-icon>arrow_back</v-icon>
                </v-btn>
                <v-toolbar-title>Update Attendance Settings</v-toolbar-title>
                <v-spacer></v-spacer>
            </v-toolbar>
            <v-card-text>
                <v-container grid-list-md>
                    <v-form ref="form">

                        <v-layout row wrap>
                            <v-flex md6>
                                <v-menu
                                    ref="menu"
                                    :close-on-content-click="false"
                                    v-model="menu.timeIn"
                                    :nudge-right="40"
                                    :return-value.sync="form.timeIn"
                                    lazy
                                    transition="scale-transition"
                                    offset-y
                                    full-width
                                    max-width="290px"
                                    min-width="290px">

                                    <v-text-field
                                        box
                                        color="orange"
                                        slot="activator"
                                        v-model="form.timeIn"
                                        label="Time In"
                                        readonly></v-text-field>

                                    <v-time-picker
                                        v-if="menu.timeIn"
                                        v-model="form.timeIn"
                                        full-width
                                        format="24hr"
                                        @change="$refs.menu.save(form.timeIn)"></v-time-picker>
                                </v-menu>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-flex md6>
                                <v-menu
                                    ref="menu"
                                    :close-on-content-click="false"
                                    v-model="menu.timeOut"
                                    :nudge-right="40"
                                    :return-value.sync="form.timeOut"
                                    lazy
                                    transition="scale-transition"
                                    offset-y
                                    full-width
                                    max-width="290px"
                                    min-width="290px">

                                    <v-text-field
                                        box
                                        color="orange"
                                        slot="activator"
                                        v-model="form.timeOut"
                                        label="Time Out"
                                        readonly></v-text-field>

                                    <v-time-picker
                                        v-if="menu.timeOut"
                                        v-model="form.timeOut"
                                        full-width
                                        format="24hr"
                                        @change="$refs.menu.save(form.timeOut)"></v-time-picker>
                                </v-menu>
                            </v-flex>
                        </v-layout>

                        <v-layout row wrap>
                            <v-flex md6>
                                <v-text-field
                                    box
                                    label="Grace Period (Minutes)" 
                                    color="orange"
                                    v-model="form.gracePeriod"
                                    v-validate="'required|numeric|min_value:0|max_value:60'"
                                    data-vv-name="Grace Period"
                                    :error-messages="errors.collect('Grace Period')"></v-text-field>
                            </v-flex>
                        </v-layout>
                        
                        <v-layout row wrap>
                            <v-btn color="success" :loading="isLoading" @click.prevent="submit">Update</v-btn>
                        </v-layout>
                    </v-form>
                </v-container>
            </v-card-text>
        </v-card>
    </v-flex>
</template>

<script>
import { mapGetters } from 'vuex'

export default {
    data () {
        return {
            form: {},
            menu: {
                timeIn: false,
                timeOut: false
            }
        }
    },

    computed: {
        ...mapGetters(["isLoading", "settings"]),
    },

    methods: {
        async submit () {
            // validate form
            await this.$validator.validateAll()

            // Check if all fields are valid
            if (this.errors.items.length === 0) {
                const result = await this.$axios.put('config', JSON.stringify(this.form))
                await this.$store.dispatch('SET_SETTINGS', result)

                this.$notify({ type: 'success', text: 'Attendance Settings has been updated!' })
                this.$router.push({ name: 'logs' })
            }
        }
    },

    mounted () {
        this.form = this.settings
    }
}
</script>