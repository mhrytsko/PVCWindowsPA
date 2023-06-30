<script>
	import p5 from 'p5';
	import _ from 'lodash-es';

	import {WindowOpeningType, WindowOpeningDirection} from '@/mixins/systemEnums.js';

	export default {
		emits: ['leaf-click'],
		props: {
			/**
             * Exemplos de dados
				{
					windowWidth: 320,
					windowHeight: 430,
					profileWidth: 10,
					panes: [
						{x: 0, y: 0, width: 320, height: 90, type: 'fixed', direction: null},
						{x: 1, y: 0, width: 160, height: 250, type: 'normal', direction: 'left'},
						{x: 1, y: 1, width: 160, height: 250, type: 'oscilobatente', direction: 'right'},
						{x: 2, y: 0, width: 320, height: 90, type: 'fixed', direction: null},
					],
				},
             */

			windowWidth: {
				type: Number,
				required: true,
			},

			windowHeight: {
				type: Number,
				required: true,
			},

			profileWidth: {
				type: Number,
				default: 70,
			},

			panes: {
				type: Array,
				required: true,
			},

			canvasWidth: {
				type: Number,
				default: 250,
			},

			canvasHeight: {
				type: Number,
				default: 250,
			},

			selectedLeaf: {
				type: Object,
			}
		},

		data() {
			return {
				sketch: null,

				shouldRedraw: true,

				panePossition: [],
			};
		},

		mounted() {
			this.sketch = new p5(this.drawWindow, this.$refs.canvas);
		},

		beforeUnmount() {
			this.sketch.remove();
		},

		methods: {
			drawWindow(sketch) {
				sketch.setup = () => {
					sketch.createCanvas(this.canvasWidth + 30, this.canvasWidth + 30);
				};

				sketch.windowResized = () => {
					// Atualizar dimensões e marcar para redesenhar
					//this.shouldRedraw = true;
				};

				// Adicione um manipulador de clique no vidro
				sketch.mouseClicked = () => {
					this.panePossition.forEach((paneCoord) => {
						// Verifique se o clique ocorreu na folha
						if (
							sketch.mouseX >= paneCoord.x &&
							sketch.mouseX <= paneCoord.x + paneCoord.width &&
							sketch.mouseY >= paneCoord.y &&
							sketch.mouseY <= paneCoord.y + paneCoord.height
						) {
							// Emita um evento com o identificador da folha
							this.$emit('leaf-click', paneCoord);
						}
					});
				};

				sketch.draw = () => {
					if (this.shouldRedraw) {
						this.panePossition = [];

						sketch.clear(); // Limpa o desenho anterior
						//sketch.background(255);

						if(this.windowHeight < 300 || this.windowWidth < 300)
						{
							this.shouldRedraw = false
							return
						}

						var scale = sketch.min([this.canvasWidth / this.windowWidth, this.canvasHeight / this.windowHeight]);
						if(Number.isNaN(scale))
							scale = 1;
						sketch.scale(scale);

						const numRows = _.isEmpty(this.panes) ? 0 : _.maxBy(this.panes, 'x').x + 1;

						const paneMatrix = Array(numRows)
							.fill()
							.map((__, row) =>
								Array(_.groupBy(this.panes, {x: row}).true).fill({
									width: 0,
									height: 0,
									openingSystem: WindowOpeningType.Fixed.value,
									openingDirection: WindowOpeningDirection.None.value,
								})
							);

						// Initializar o paneMatrix com dados das folhas
						this.panes.forEach((pane) => {
							paneMatrix[pane.x][pane.y] = pane;
						});

						// Inicializar o estilo default
						sketch.stroke('black')
						sketch.strokeWeight(1)

						// Draw window frame
						sketch.fill(255);
						sketch.rect(0, 0, this.windowWidth, this.windowHeight);


						/*
							// Define a cor da borda
  							stroke(0, 0, 255);
							strokeWeight(4)

							.text-primary {
								color: rgb(var(--v-theme-primary)) !important;
							}

							// Redefine as configurações de cor para o padrão
							noStroke();
							strokeWeight(1); // Default
						*/

						paneMatrix.forEach((row, rowIndex) => {
							row.forEach((pane, colIndex) => {
								const { width, height, openingSystem, openingDirection } = pane;
								const isSelected = pane.uid && pane.uid === this.selectedLeaf?.uid;

								const paneBorder = openingSystem === WindowOpeningType.Fixed.value ? 0 : this.profileWidth / 3/*40*/;

								const x = _.sumBy(row.slice(0, colIndex), 'width') + paneBorder;
								const y =
									_.sumBy(paneMatrix.slice(0, rowIndex), (row) => _.maxBy(row, 'height').height) +
									paneBorder;

								const paneWidth = width - paneBorder - paneBorder;
								const paneHeight = height - paneBorder - paneBorder;

								// Se é folha selecionada, marcar com border diferente
								if(isSelected)
								{
									sketch.stroke('rgb(24, 103, 192)');
									sketch.strokeWeight(8)
								}

								// Draw pane
								sketch.fill(255);
								sketch.rect(x, y, paneWidth, paneHeight);

								// Reset do estilo
								if(isSelected)
								{
									sketch.stroke('black')
									sketch.strokeWeight(1)
								}

								// Armazenar os dados da folha
								this.panePossition.push({
									x: x * scale,
									y: y * scale,
									width: paneWidth * scale,
									height: paneHeight * scale,
									pane,
								});

								// Draw glass
								if(pane.frosted)
									sketch.fill('rgba(234, 240, 240, 1)');
								else
									sketch.fill('rgba(110, 197, 234, 0.1)');
								const glassX = x + this.profileWidth,
									glassY = y + this.profileWidth,
									glassWidth = paneWidth - 2 * this.profileWidth,
									glassHeight = paneHeight - 2 * this.profileWidth;

								sketch.rect(glassX, glassY, glassWidth, glassHeight);

								// Draw handle if necessary
								if (openingSystem !== WindowOpeningType.Fixed.value) {
									let profileCenter = this.profileWidth * 0.5,
										handleHeight = 140,
										handleWidth = 20 /* this.profileWidth * 0.5 */,
										handleAuxHeight = 70,
										handleAuxWidth = 30,
										handleWidthHalf = handleWidth * 0.5, /* 10 */
										handleAuxWidthHalf = handleAuxWidth * 0.5, /* 15 */
										hwDiff = handleAuxWidthHalf - handleWidthHalf;

									if (openingDirection === WindowOpeningDirection.LeftRight.value) {
										sketch.fill(255);
										sketch.rect(x + (profileCenter - handleAuxWidthHalf), y + (paneHeight / 2) - (handleAuxHeight / 2), handleAuxWidth, handleAuxHeight, 20); // left handle - aux
										sketch.rect(x +  (profileCenter - handleWidthHalf), y + (paneHeight / 2) - hwDiff, handleWidth, handleHeight, 20); // left handle
									} else if (openingDirection ===  WindowOpeningDirection.RightLeft.value) {
										sketch.fill(255);
										sketch.rect(
											x + paneWidth - (profileCenter + handleAuxWidthHalf),
											y + (paneHeight / 2) - (handleAuxHeight / 2),
											handleAuxWidth,
											handleAuxHeight,
											20
										); // right handle - aux
										sketch.rect(
											x + paneWidth - (profileCenter + handleWidthHalf),
											y + (paneHeight / 2) - hwDiff,
											handleWidth,
											handleHeight,
											20
										); // right handle
										
									}
								}

								// Draw opening direction lines
								sketch.stroke(0);
								if (openingSystem === WindowOpeningType.SideHung.value || openingSystem === WindowOpeningType.TiltAndTurn.value)
								{
									if (openingDirection === WindowOpeningDirection.RightLeft.value) {
										sketch.line(glassX, glassY, glassX + glassWidth, glassY + glassHeight / 2);
										sketch.line(
											glassX,
											glassY + glassHeight,
											glassX + glassWidth,
											glassY + glassHeight / 2
										);
									} else if (openingDirection ===  WindowOpeningDirection.LeftRight.value) {
										sketch.line(glassX + glassWidth, glassY, glassX, glassY + glassHeight / 2);
										sketch.line(
											glassX + glassWidth,
											glassY + glassHeight,
											glassX,
											glassY + glassHeight / 2
										);
									}
								}

								if (openingSystem === WindowOpeningType.TiltAndTurn.value || openingSystem === WindowOpeningType.TiltOnly.value) {
									sketch.line(glassX, glassY + glassHeight, glassX + glassWidth / 2, glassY);
									sketch.line(
										glassX + glassWidth,
										glassY + glassHeight,
										glassX + glassWidth / 2,
										glassY
									);
								}
							});
						});

						this.shouldRedraw = false; // Redesenho concluído
					}
				};
			},
		},

		watch: {
			windowWidth() {
				// Marcar para redesenhar quando a largura da janela for alterada
				this.shouldRedraw = true;
			},
			windowHeight() {
				// Marcar para redesenhar quando a altura da janela for alterada
				this.shouldRedraw = true;
			},
			profileWidth() {
				// Marcar para redesenhar quando a largura do perfil for alterada
				this.shouldRedraw = true;
			},
			selectedLeaf() {
				// Marcar para redesenhar quando a largura do perfil for alterada
				this.shouldRedraw = true;
			},
			panes: {
				handler() {
					// Marcar para redesenhar quando as configurações dos painéis forem alteradas
					this.shouldRedraw = true;
				},
				deep: true
			},
		},
	};
</script>

<template>
	<v-row dense>
		<v-col cols="auto">
			<div ref="canvas"></div>
		</v-col>
	</v-row>
</template>
