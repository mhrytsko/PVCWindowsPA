<script>
    export default
    {
        emits: [
            'submit',
            'apply',
            'cancel',
            'delete'
        ],

        props:
        {
            showDelete:
            {
                type: Boolean,
                default: true
            },

            showApply:
            {
                type: Boolean,
                default: false
            }
        },

        methods:
        {
            submitForm()
            {
                this.$emit('submit')
            },

            applyForm()
            {
                this.$emit('apply')
            },

            cancelForm()
            {
                this.$emit('cancel')
            },

            deleteForm()
            {
                this.$emit('delete')
            }
        }
    }
</script>

<template>
    <slot name="prepend"></slot>
	<v-row>
		<slot name="crud-prepend"></slot>
        <v-col cols="auto">
			<v-btn @click="submitForm" color="primary">Gravar</v-btn>
		</v-col>
        <v-col cols="auto" v-if="showApply">
			<v-btn @click="applyForm" color="secondary">Aplicar</v-btn>
		</v-col>
        <v-col cols="auto">
			<v-btn @click="cancelForm">Voltar</v-btn>
		</v-col>
        <slot name="crud-append"></slot>
	</v-row>
    <v-row v-if="showDelete">
        <v-col cols="auto">
            <v-btn @click="deleteForm" color="red">Eliminar</v-btn>
		</v-col>
    </v-row>
    <slot name="append"></slot>
</template>
