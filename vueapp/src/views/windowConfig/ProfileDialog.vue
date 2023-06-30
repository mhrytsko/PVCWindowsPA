<script>
	import {mapState} from 'pinia';
	import {useWindowProfilesDataStore} from '@/store/windowProfile.js';

	import {get, filter, isEmpty} from 'lodash-es';

	export default {
		emits: ['update:modelValue', 'update:show'],

		props: {
			modelValue: {
				required: true,
			},

			show: {
				type: Boolean,
				required: true,
				default: false,
			},

			profileBrandId: {
				type: String,
				default: null,
			},
		},

        data()
        {
            return {
                profileTab: 'one',
            }
        },

		methods: {
            isEmpty,

			onSelect(value) {
				const selectedValue = get(value, '0', this.modelValue);
				this.$emit('update:modelValue', selectedValue);
			},

			onClose() {
				this.$emit('update:show', false);
			},
		},

		computed: {
			...mapState(useWindowProfilesDataStore, ['windowProfiles']),

			brandWindowProfiles() {
				return filter(this.windowProfiles, {brandId: this.profileBrandId});
			},
		},
	};
</script>

<template>
	<v-dialog
        :modelValue="show"
		@update:modelValue="onClose"
		fullscreen
		:scrim="false"
		scrollable
		transition="dialog-bottom-transition">
		<v-card>
			<v-toolbar>
				<v-btn icon dark @click="onClose">
					<v-icon>mdi-close</v-icon>
				</v-btn>
			</v-toolbar>
			<v-card-title>
				<span class="text-h5">Perfil</span>
			</v-card-title>
			<v-card-text>
				<template v-if="isEmpty(brandWindowProfiles)">
					<v-sheet>
						<v-alert
							density="compact"
							type="warning"
							title="Marca do perfil"
							text="Primeiro, selecione uma marca de perfil para visualizar a lista." />
					</v-sheet>
				</template>
				<v-list
					color="primary"
					:selected="[modelValue]"
					@update:selected="onSelect">
					<v-list-item
						v-for="profile in brandWindowProfiles"
						:key="profile.id"
						:value="profile.id"
						elevation="3"
						class="ma-1">
						<v-row
							><v-col
								><span class="text-h6">{{ profile.name }}</span></v-col
							></v-row
						>
						<v-row>
							<v-col cols="auto">
								<v-img :src="profile.image?.fileSrc" alt="Perfil" width="250" height="250" />
							</v-col>
							<v-col>
								<v-card @click="(e) => e.stopPropagation()">
									<v-tabs v-model="profileTab" bg-color="primary">
										<v-tab value="one">Descrição</v-tab>
										<v-tab value="two">Dados técnicos</v-tab>
										<v-tab value="three">Ensaios</v-tab>
									</v-tabs>
									<v-card-text>
										<v-window v-model="profileTab">
											<v-window-item value="one">
												<span class="text-with-line-breaks">
													{{ profile.description }}
												</span>
											</v-window-item>

											<v-window-item value="two">
												<v-table density="compact" class="hide-table-scroll">
													<tbody>
														<tr>
															<td>Envidraçamento:</td>
															<td>Máx. {{ profile.maxGlassThickness }} mm</td>
														</tr>
														<tr>
															<td>Secções:</td>
															<td>Aro {{ profile.constructionDepth }} mm</td>
														</tr>
														<tr>
															<td>Dimensões máx. folha:</td>
															<td>
																<v-row dense>
																	<v-col cols="12"
																		>Largura (L)
																		{{ profile.maxLeafSizeWidth }}
																		mm</v-col
																	>
																	<v-col cols="12"
																		>Altura (H)
																		{{ profile.maxLeafSizeHeight }}
																		mm</v-col
																	>
																</v-row>
															</td>
														</tr>
														<tr>
															<td>Possibilidades de abertura:</td>
															<td>
																<v-row dense>
																	<v-col cols="12"
																		><v-icon size="small">
																			{{
																				profile.sideHungOpening
																					? 'mdi-check'
																					: 'mdi-close'
																			}}
																		</v-icon>
																		Batente</v-col
																	>
																	<v-col cols="12"
																		><v-icon size="small">
																			{{
																				profile.tiltAndTurnOpening
																					? 'mdi-check'
																					: 'mdi-close'
																			}} </v-icon
																		>Oscilobatente
																	</v-col>
																	<v-col cols="12"
																		><v-icon size="small">
																			{{
																				profile.tiltOnlyOpening
																					? 'mdi-check'
																					: 'mdi-close'
																			}}
																		</v-icon>
																		Basculante
																	</v-col>
																</v-row>
															</td>
														</tr>
													</tbody>
												</v-table>
											</v-window-item>

											<v-window-item value="three">
												<v-table density="compact" class="hide-table-scroll">
													<tbody>
														<tr>
															<td>Isolamento térmico:</td>
															<td>Uw ≥ {{ profile.thermalInsulation }} (W/m2K)</td>
														</tr>
														<tr>
															<td>Isolamento acústico:</td>
															<td>Rw até {{ profile.soundInsulation }} dB</td>
														</tr>
														<tr>
															<td>Permeabilidade ao ar:</td>
															<td>{{ profile.airPermeability }}</td>
														</tr>
														<tr>
															<td>Estanquidade à água:</td>
															<td>{{ profile.waterTightness }}</td>
														</tr>
														<tr>
															<td>Resistência ao vento:</td>
															<td>{{ profile.windResistance }}</td>
														</tr>
													</tbody>
												</v-table>
											</v-window-item>
										</v-window>
									</v-card-text>
								</v-card>
							</v-col>
						</v-row>
					</v-list-item>
				</v-list>
			</v-card-text>
		</v-card>
	</v-dialog>
</template>
