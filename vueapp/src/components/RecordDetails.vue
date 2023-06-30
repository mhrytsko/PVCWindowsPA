<script>
	export default {
		props: {
            creationDate:
            {
                type: String
            },

            modificationDate: 
            {
                type: String
            }
        },

        computed: {

        },

        methods:
        {
            getStrDate(value) {
				let dateT = Date.parse(value);
				if (Number.isNaN(dateT)) return '';
				else {
					try {
						let date = new Date(dateT);
						return date.toLocaleString();
					} catch {
						return '';
					}
				}
			},
        }
	};
</script>

<template>
    <v-row>
        <v-col cols="auto">

            <div class="text-subtitle-1 text-medium-emphasis d-flex align-center justify-space-between">
                <span class="pr-2 font-weight-medium">Data de criação</span>
                <span class="text-overline">{{ getStrDate(creationDate) }}</span>
            </div>
        </v-col>
        <v-col cols="auto">
            <div class="text-subtitle-1 text-medium-emphasis d-flex align-center justify-space-between">
                <span class="pr-2 font-weight-medium">Data de modificação</span>
                <span class="text-overline">{{ getStrDate(modificationDate) }}</span>
            </div>
        </v-col>
    </v-row>
</template>
