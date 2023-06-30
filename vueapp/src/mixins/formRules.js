import { isEmpty } from "lodash-es";

export default {
	data() {
		return {
			rules: {
				required: (value) => !!value || 'Campo obrigatório',
				//counter: (value) => value.length <= 20 || 'Max 20 characters',

				email: (value) => {
					const pattern =
						/^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
					return isEmpty(value) || pattern.test(value) || 'E-mail inválido';
				},

				minLength: (minLength) => {
					return (value) => isEmpty(value) || value.length < minLength ? `Mínimo de ${minLength} caracteres` : true;
				},

				integer: {
					validValue: (value) => {
						if (value && !/^\d{1,5}$/.test(value)) {
							return 'Digite um número inteiro de até 5 dígitos';
						} else if (value && (value < 1 || value > 100)) {
							return 'Digite um número entre 1 e 100';
						} else {
							return true;
						}
					},
					maxSize:(value) => {
						if (value && !/^\d{1,5}$/.test(value)) {
							return 'Digite um número inteiro de até 5 dígitos';
						} else if (value && (value < 1 || value > 100)) {
							return 'Digite um número entre 1 e 100';
						} else {
							return true;
						}
					},
				},


				decimal:
				{
					validValue: (value) => {
						if (value && !/^\d{1,5}$/.test(value)) {
							return 'Digite um número inteiro de até 5 dígitos';
						} else if (value && (value < 1 || value > 100)) {
							return 'Digite um número entre 1 e 100';
						} else {
							return true;
						}
					},
					maxSize:(value) => {
						if (value && !/^\d{1,5}$/.test(value)) {
							return 'Digite um número inteiro de até 5 dígitos';
						} else if (value && (value < 1 || value > 100)) {
							return 'Digite um número entre 1 e 100';
						} else {
							return true;
						}
					},
				}

			},
		};
	},
};
