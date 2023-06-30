<script>
	import menuHandlers from '@/mixins/menuHandlers.js';
	import DataTableConfig from '@/mixins/dataTableConfig.js';

	import systemEnums from '@/mixins/systemEnums.js';

	import _find from 'lodash-es/find';

	export default {
		name: 'UsersList',
		mixins: [menuHandlers],

		data() {
			return {
				menuInfo: {
					controller: 'Users',
					supportForm: 'management-user',
					goBack: {name: 'home'},
				},

				tableConfig: new DataTableConfig({
					headers: [
						{
							title: 'Username',
							align: 'start',
							sortable: true,
							key: 'userName',
						},

						{
							title: 'Nível de acesso',
							align: 'start',
							sortable: false,
							key: 'role',
						},
					],
					queryUpdate: this.queryUpdate,
				}),
			};
		},

		methods: {
			getUserPermission(uLevel) {
				return _find(systemEnums.PermissionLevel, (pLevel) => pLevel.value === uLevel)?.text;
			},
		},
	};
</script>

<template>
	<v-container>
		<v-responsive class="align-left text-left fill-height">
			<v-row>
				<v-col>
					<m-data-table :table-config="tableConfig" title="Utilizadores" @row-click="handleClick">
						<template v-slot:item.userName="{item}">
							<v-chip :color="item.raw.emailConfirmed ? 'green' : 'red'">
								{{ item.raw.userName }}
							</v-chip>
						</template>
						<template v-slot:item.role="{item}">
							{{ getUserPermission(item.raw.role) }}
						</template>
						<template v-slot:tfoot>
							<tr>
								<td>
									<!-- <v-text-field
										v-model="name"
										hide-details
										placeholder="Search name..."
										class="ma-2"
										density="compact"></v-text-field> -->
								</td>
							</tr>
						</template>
					</m-data-table>
				</v-col>
			</v-row>

			<v-row>
				<v-col cols="12">
					<v-btn block class="mb-8" color="blue" size="large" variant="tonal" @click="onAddItem">
						Adicionar um novo utilizador
					</v-btn>
				</v-col>
			</v-row>
		</v-responsive>
	</v-container>
</template>
