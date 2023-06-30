import { get } from 'lodash-es';

export default class DataTableConfig {
	/**
	 * Configuração da tabela
	 * @param {Object} props - Propriedades da tabela
	 * @param {number} props.itemsPerPage - Número de itens por página (padrão: 10)
	 * @param {number} props.page - Página atual (padrão: 1)
	 * @param {string} props.search - Texto de pesquisa (padrão: '')
	 * @param {boolean} props.loading - Indicador de carregamento (padrão: false)
	 * @param {string} props.itemValue - Nome da propriedade que contém o valor único do item (padrão: 'id')
	 * @param {number} props.itemsLength - Número total de itens (padrão: 0)
	 * @param {Array} props.items - Lista de itens (padrão: [])
	 * @param {Array} props.headers - Lista de cabeçalhos da tabela (padrão: [])
	 * @param {Object} props.customFilter - Filtro personalizado (padrão: undefined)
	 * @param {boolean} props.hideActions - Indicador de ocultar ações (padrão: false)
	 * @param {boolean} props.multiSort - Indicador de classificação múltipla (padrão: false)
	 * @param {boolean} props.showSelect - Indicador de exibir seleção (padrão: false)
	 * @param {string} props.sortBy - Campo de ordenação (padrão: [])
	 * @param {boolean} props.sortDesc - Indicador de ordenação descendente (padrão: false)
	 * @param {number} props.totalItems - Número total de itens (padrão: 0)
	 * @param {Function} props.rowClick - Callback para o evento de clique em uma linha
	 * @param {Function} props.queryUpdate - Callback para a atualização da pesquisa / paginação
	 */
	constructor(props = {}) {
		// Configurações
		/** Número de itens por página (padrão: 10) */
		this.itemsPerPage = props.itemsPerPage || 10;
		/** Página atual (padrão: 1) */
		this.page = props.page || 1;
		/** Texto de pesquisa (padrão: '') */
		this.search = props.search || '';
		/** Indicador de carregamento (padrão: false) */
		this.loading = props.loading || false;
		/** Nome da propriedade que contém o valor único do item (padrão: 'id') */
		this.itemValue = props.itemValue || 'id';
		/** Número total de itens (padrão: 0) */
		this.itemsLength = props.itemsLength || 0;
		/** Lista de itens (padrão: []) */
		this.items = props.items || [];
		/** Lista de cabeçalhos da tabela (padrão: []) */
		this.headers = props.headers || [];

		/** Filtro personalizado (padrão: undefined) */
		this.customFilter = props.customFilter || undefined;
		/** Indicador de ocultar ações (padrão: false) */
		this.hideActions = props.hideActions || false;
		/** Indicador de classificação múltipla (padrão: false) */
		this.multiSort = props.multiSort || false;
		/** Indicador de exibir seleção (padrão: false) */
		this.showSelect = props.showSelect || false;
		/** Campo de ordenação (padrão: []) */
		this.sortBy = props.sortBy || [];
		/** Indicador de ordenação descendente (padrão: false) */
		this.sortDesc = props.sortDesc || false;
		/** Número total de itens (padrão: 0) */
		this.totalItems = props.totalItems || 0;

		/** Selected rows (padrão: []) */
		this.selected = props.selected || [];
		/** Shows the select checkboxes in both the header and rows (padrão: false) */
		this.showSelect = props.showSelect || false;
		this.returnObject = props.returnObject || false;


		/** Callback para o evento de clique em uma linha */
		this.rowClick = props.rowClick || undefined;
		/** Callback para a atualização da pesquisa / paginação */
		this.queryUpdate = props.queryUpdate || undefined;
	}

	// Handlers de eventos da tabela
	onRowClick(item) {
		if (typeof this.rowClick === 'function') {
			this.rowClick(item);
		}
	}

	getQuery()
	{
		return {
			search: get(this, "search", ''),
			rowsPerPage: get(this, "itemsPerPage", 10),
			page: get(this, "page", 1),
			sortBy: get(this, "sortBy[0].key", null),
			order: get(this, "sortBy[0].order", null),
		}
	}

	updateOptions(newOptions)
	{
		this.search = get(newOptions, "search", this.search)
		this.itemsPerPage = get(newOptions, "itemsPerPage", this.itemsPerPage)
		this.page = get(newOptions, "page", this.page)
		this.sortBy = get(newOptions, "sortBy", this.sortBy)

		if (typeof this.queryUpdate === 'function') {
			const query = this.getQuery()
			// Lógica do handler da pesquisa
			this.queryUpdate(query);
		}
	}

	onSearch(search) {
		this.search = typeof search !== 'string' ? '' : search
	}

	// Função para atualizar os dados da tabela
	updateData({items, totalItems}) {
		this.items = items;
		this.totalItems = totalItems;
	}
}
