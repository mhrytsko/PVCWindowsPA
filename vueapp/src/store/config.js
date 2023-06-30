import { defineStore } from 'pinia'

export const useConfigStore = defineStore('config', {
    state: () => ({
        count: 0
    }),
    actions: {
        increment() {
            this.count++
        }
    }
})