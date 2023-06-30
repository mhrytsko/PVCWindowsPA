<script>
    import {v4 as uuidv4} from 'uuid';
    import Inputmask from 'inputmask'

    //import $ from 'jquery'

    //import {reactive} from 'vue'

    //import { max } from 'lodash-es';

	export default {
        emits: ['update:modelValue'],

		props: {
            modelValue: {
                type: Number,
                required: true
            },
            label: {
                type: String,
                required: true
            },
            prefix: {
                type: String,
                default: ''
            },
            suffix: {
                type: String,
                default: ''
            },

            isDecimal: {
                type: Boolean,
                default: false
            },

            min: {
                type: Number,
                default: undefined
            },

            max: {
                type: Number,
                default: undefined
            },

            digits: {
                type: Number,
                default: 2
            },

            readonly: {
                type: Boolean
            },

            variant: {
                type: String
            }


        },

        data()
        {
            return {
                id: uuidv4(),
                inputMask: null,
                inputValue: this.modelValue,
                iMask: null,
            }
        },

        created()
        {
            if(this.isDecimal)
            {
                this.inputMask = Inputmask("decimal", {
                    min: 0,//this.min,
                    max: this.max,
                    digits: this.digits,
                    digitsOptional: false,
                    allowMinus: false,
                    inputtype: "number",
                    numericInput: true,
                    rightAlign: false
                })
            }
            else
            {
                this.inputMask = Inputmask("integer", {
                    min: 0,//this.min,
                    max: this.max,
                    allowMinus: false,
                    inputtype: "number",
                    rightAlign: false
                })
            }
        },

        mounted()
        {
            const inputElement = this.$refs.input.$el.querySelector('input');
            this.iMask = this.inputMask.mask(/*`#${this.id}`*/inputElement);
        },

        beforeUnmount()
        {
            if(this.iMask)
                this.iMask.remove()
        },

        methods:
        {
            toValidNumber(newValue) {
                return Number(newValue);
            },

            minValueRule(value)
            {
                if(this.min !== undefined && value < this.min)
                    return `O valor minimo é ${this.min}`
            }
        },

        watch: {
            modelValue(newVal) {
                // Atualizar cópia local quando a propriedade 'modelValue' for alterada externamente
                const value = this.toValidNumber(newVal)
                this.inputValue = value;
            },
            inputValue(newVal) {
                // Emitir evento para notificar o componente pai
                const value = this.toValidNumber(newVal)
                this.$emit('update:modelValue', value);
            }
        }
	};
</script>

<template>
	<v-text-field
        ref="input"
		:id="id"
		v-model="inputValue"
		:label="label"
		:prefix="prefix"
		:suffix="suffix"
        :readonly="readonly"
        :variant="variant"
        :rules="[minValueRule]">
        <template v-if="$slots.append" v-slot:append>
			<slot name="append"></slot>
		</template>
        <template v-if="$slots.prepend" v-slot:prepend>
			<slot name="prepend"></slot>
		</template>
    </v-text-field>
</template>
