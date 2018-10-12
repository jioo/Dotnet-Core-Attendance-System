<template>
    <v-container grid-list-md>
        <v-layout row wrap>
            <v-flex md6>
                <v-text-field 
                    label="Search Name"
                    prepend-icon="search" 
                    color="orange" 
                    v-model="filter.search"></v-text-field>
            </v-flex>
        </v-layout>

        <v-layout row wrap>
            <v-flex md4>
                <v-select 
                    prepend-icon="event" 
                    v-model="presetSelected"
                    color="orange" 
                    :items="presetDates"
                    clearable
                    label="Preset Date"
                    ></v-select>
            </v-flex>
            <v-flex md4>
                <v-menu
                    ref="menu1"
                    :close-on-content-click="false"
                    v-model="menu1"
                    :return-value.sync="filter.startDate"
                    full-width
                    min-width="290px">

                    <v-text-field
                        ref="Start Date"
                        color="orange" 
                        slot="activator"
                        v-model="filter.startDate"
                        label="Start Date"
                        prepend-icon="event"
                        readonly
                        v-validate="`${rule.startDate}`" 
                        data-vv-name="Start Date"
                        data-vv-validate-on="change" 
                        :error-messages="errors.collect('Start Date')" 
                        ></v-text-field>

                    <v-date-picker v-model="filter.startDate" @input="$refs.menu1.save(filter.startDate)"></v-date-picker>
                </v-menu>
            </v-flex>

            <v-flex md4>
                <v-menu
                    ref="menu2"
                    :close-on-content-click="false"
                    v-model="menu2"
                    :return-value.sync="filter.endDate"
                    full-width
                    min-width="290px">

                    <v-text-field
                        ref="End Date"
                        color="orange" 
                        slot="activator"
                        v-model="filter.endDate"
                        label="End Date"
                        prepend-icon="event"
                        readonly
                        v-validate="`${rule.endDate}`" 
                        data-vv-name="End Date" 
                        data-vv-validate-on="change"
                        :error-messages="errors.collect('End Date')"
                        ></v-text-field>

                    <v-date-picker v-model="filter.endDate" @input="$refs.menu2.save(filter.endDate)"></v-date-picker>
                </v-menu>
            </v-flex>
        </v-layout>

        <v-layout row wrap>
            <v-flex md4>
                <v-btn block color="success" @click.prevent="emitEvent()">Apply Filter</v-btn>
            </v-flex>
            <v-flex md4>
                <v-btn block color="grey" @click.prevent="clear()">Clear</v-btn>
            </v-flex>
        </v-layout>
    </v-container>
</template>
<script>
import moment from 'moment'

export default {
    data () {
        return {
            filter: {
                startDate: '',
                endDate: '',
                search: ''
            },
            search: '',
            menu1: false,
            menu2: false,
            presetSelected: '',
            presetDates: [
                'Today',
                'Yesterday',
                'Last 7 Days',
                'Last 30 Days',
                'This Month',
                'Last Month',
                'Last 3 Month',
                'Last 6 Month'
            ],
            rule: {
                startDate: 'before:End Date, true|date_format:YYYY-MM-DD',
                endDate: 'after:Start Date, true|date_format:YYYY-MM-DD'
            },
            delayTimer: 0
        }
    },

    methods: {
        clear () {
            this.resetFilter()
            this.emitEvent()
        },

        emitEvent() {
            const payload = this.filter

            this.$validator.validateAll().then(() => {
                // Check if start & end date are valid
                if (this.errors.items.length === 0) {
                    this.$emit('onFilter', payload)
                }
            })
        },

        resetFilter() {
            this.filter = {
                startDate: '',
                endDate: '',
                search: ''
            }
        }
    },

    watch: {
        presetSelected (val) {
            let selectedValue = []
            const dateFormat = 'YYYY-MM-DD'
            switch(val) {
                case 'Today':
                    selectedValue = [ 
                        moment().format(dateFormat), 
                        moment().format(dateFormat) 
                    ]
                    break;
                
                case 'Yesterday':
                    selectedValue = [
                        moment().subtract(1, 'days').format(dateFormat), 
                        moment().subtract(1, 'days').format(dateFormat) 
                    ]
                    break;
                
                case 'Last 7 Days':
                    selectedValue = [ 
                        moment().subtract(6, 'days').format(dateFormat), 
                        moment().format(dateFormat) 
                    ]
                    break;

                case 'Last 30 Days':
                    selectedValue = [ 
                        moment().subtract(29, 'days').format(dateFormat), 
                        moment().format(dateFormat) 
                    ]
                    break;
                
                case 'This Month':
                    selectedValue = [
                        moment().startOf('month').format(dateFormat), 
                        moment().endOf('month').format(dateFormat)
                    ]
                    break;
                
                case 'Last Month':
                    selectedValue = [ 
                        moment().subtract(1, 'month').startOf('month').format(dateFormat), 
                        moment().subtract(1, 'month').endOf('month').format(dateFormat)
                    ]
                    break;

                case 'Last 3 Month':
                    selectedValue = [ 
                        moment().subtract(3, 'month').startOf('month').format(dateFormat), 
                        moment().subtract(1, 'month').endOf('month').format(dateFormat)
                    ]
                    break;

                case 'Last 6 Month':
                    selectedValue = [ 
                        moment().subtract(6, 'month').startOf('month').format(dateFormat), 
                        moment().subtract(1, 'month').endOf('month').format(dateFormat)
                    ]
                    break;

                default:
                    selectedValue = [ '', '']
                    break;
            }   
            
            this.filter.startDate = selectedValue[0]
            this.filter.endDate = selectedValue[1]
        }
    },

    mounted () {
        this.resetFilter()
    }
}
</script>