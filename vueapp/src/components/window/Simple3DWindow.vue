<template>
	<v-container ref="container" class="fill-height ma-0 pa-0" min-height="50px">
		<model-viewer
			id="model-viewer"
			:src="gltfData"
			:ios-src="gltfDataIOS"
			:skybox-image="skyBox"
			camera-controls
			ar
			:ar-modes="arModes"
			:ar-placement="arPlacement"
			touch-action="pan-y"
			xr-environment
			shadow-intensity="1.5"
			shadow-softness="0.5"
			alt="Janela PVC">
		</model-viewer>
	</v-container>
</template>

<style>
	model-viewer {
		height: 100%;
		width: 100%;
	}
</style>

<script>
	import * as THREE from 'three';
	import {GLTFExporter} from 'three/examples/jsm/exporters/GLTFExporter.js';
	import {USDZExporter} from 'three/examples/jsm/exporters/USDZExporter.js';
	import {GLTFLoader} from 'three/examples/jsm/loaders/GLTFLoader.js';

	import '@google/model-viewer';
	//skybox-image="/hdr/lilienstein_4k.hdr"

	import {Brush, Evaluator, ADDITION /*, SUBTRACTION, INTERSECTION, DIFFERENCE*/} from 'three-bvh-csg';

	import _ from 'lodash-es';
	import { markRaw } from 'vue'
	import netApi from '@/api/network/index.js';

	import {WindowOpeningType, WindowOpeningDirection, ColorType} from '@/mixins/systemEnums.js';

	// TODO: https://tympanus.net/codrops/2021/10/27/creating-the-effect-of-transparent-glass-and-plastic-in-three-js/
	// https://codesandbox.io/s/10-40wj3?from-embed=&file=/src/index.js

	export default {
		props: {
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

			indorColorId: {
				type: String,
			},

			indorColorType: {
				default: ColorType.Solid.value,
			},

			// 3D & AR
			useARCore: {
				type: Boolean,
				default: false,
			},

			tryWall: {
				type: Boolean,
				default: true,
			},

			showSkybox: {
				type: Boolean,
				default: true,
			},
		},

		data() {
			return {
				pixelScale: 0.001,

				gltfData: undefined,
				gltfDataIOS: undefined,

				isIOS: false,

				handleModel: markRaw({})
			};
		},

		computed: {
			arModes() {
				return this.useARCore ? 'scene-viewer webxr quick-look' : 'webxr scene-viewer quick-look';
			},

			arPlacement() {
				return this.tryWall ? 'wall' : 'floor';
			},

			skyBox() {
				return this.showSkybox ? '/hdr/resting_place_4k.hdr' : undefined;
			},
		},

		mounted() {
			console.log('3D mounted! (start)');

			this.isIOS = /iPad|iPhone|iPod/.test(navigator.userAgent) && !window.MSStream;

			/*this.getHandle().then(() => {
				new Promise((resolve) => {
					const janela = this.get3DJanela();
					resolve(janela);
				}).then((janela) => {
					//console.warn('Janela: ', janela);
					this.getGLTF(janela);
				});
			})*/

			this.getHandle()
				.then(() => {
					return this.get3DJanela();
				})
				.then((janela) => {
					return this.getGLTF(janela);
				})
				.catch((error) => {
					console.error('Erro (mounted):', error);
				});
		},

		beforeUnmount() {
			console.log('THREE Unmount!');
		},

		methods: {
			async getGLTF(janela) {
				if (!this.isIOS) {
					// Normal
					const exporter = new GLTFExporter();
					exporter.parse(
						janela,
						(gltf) => {
							const imageBlob = new Blob([JSON.stringify(gltf, null, 2)], {
								type: 'application/json',
							});
							this.gltfData = URL.createObjectURL(imageBlob);
						},
						(error) => console.error('getGLTF', error),
						{
							trs: false,
							binary: false,
						}
					);
				} else {
					// iOS
					const exporterIOS = new USDZExporter();
					const arraybuffer = await exporterIOS.parse(janela);
					const fileBlob = new Blob([arraybuffer], {
						type: 'application/octet-stream',
					});
					this.gltfDataIOS = URL.createObjectURL(fileBlob);
				}
			},

			comSombras(obj3D) {
				obj3D.castShadow = true; // o objeto lança sombras
				obj3D.receiveShadow = true; // o objeto recebe sombras
				return obj3D;
			},

			async getFrameTexture() {
				//MeshPhongMaterial | MeshStandardMaterial |
				const frameMaterial = this.isIOS
					? new THREE.MeshStandardMaterial({color: 0xffffff})
					: new THREE.MeshPhongMaterial({
							color: 0xffffff,
							side: THREE.DoubleSide,
							metalness: 1,
							roughness: 0.5,
				});
				return new Promise((resolve) => {
					if (!_.isEmpty(this.indorColorId)) {
						if (this.indorColorType === ColorType.Pattern.value) {
							// Crie um carregador de textura
							var textureLoader = new THREE.TextureLoader();

							// Definir o URL da imagem
							var url = netApi.apiUrl('WindowColors', 'GetTexture', this.indorColorId);

							// Use o carregador de textura para carregar a imagem
							textureLoader.load(
								url,
								(texture) => {
									// Quando a imagem for carregada, criar um material com a textura
									const material = this.isIOS
										? new THREE.MeshStandardMaterial({map: texture})
										: new THREE.MeshPhongMaterial({map: texture, metalness: 0.75, roughness: 0.6});

									resolve(material);
								},
								undefined,
								(error) => {
									// Se houver algum erro ao carregar a imagem
									console.error('Houve um erro ao carregar a textura:', error);
									resolve(frameMaterial);
								}
							);
						} else resolve(frameMaterial);
					} else resolve(frameMaterial);
				});
			},

			async getHandle() {
				return new Promise((resolve) => {
					let loader = new GLTFLoader();
					loader.load(
						'/models/windowHandle.gltf',
						(gltf) => {
							this.handleModel = gltf; //.scene
							resolve(true);
						},
						undefined,
						function (error) {
							console.error('getHandle', error);
							resolve(false);
						}
					);
				});
			},

			getFrameObj(largura, altura, espesura, profundidade, z, openingSystem, openingDirection) {
				if(z === undefined)
					z = 0;

				const diff = espesura * 1.01,
					handleZ = z + (profundidade / 2);

				let frameCima = this.comSombras(new THREE.BoxGeometry(espesura, largura + diff, profundidade)),
					frameBaixo = this.comSombras(new THREE.BoxGeometry(espesura, largura + diff, profundidade)),
					frameEsquerdo = this.comSombras(new THREE.BoxGeometry(espesura, altura + diff, profundidade)),
					frameDireito = this.comSombras(new THREE.BoxGeometry(espesura, altura + diff, profundidade));

				frameCima.rotateZ(Math.PI / 2);
				frameBaixo.rotateZ(Math.PI / 2);

				frameCima.translate(0, altura / 2, z); // Ajuste para mover o centro para cima
				frameBaixo.translate(0, -altura / 2, z); // Ajuste para mover o centro para baixo

				frameEsquerdo.translate(-largura / 2, 0, z); // Ajuste para mover o centro para a esquerda
				frameDireito.translate(largura / 2, 0, z); // Ajuste para mover o centro para a direita

				if(openingSystem && openingSystem !== WindowOpeningType.Fixed.value && this.handleModel.scene)
				{
					const handle = this.handleModel.scene.clone(true);

					if (openingSystem === WindowOpeningType.SideHung.value || openingSystem === WindowOpeningType.TiltAndTurn.value)
					{
						if (openingDirection === WindowOpeningDirection.LeftRight.value) {
							const handleGeometry = _.map(handle.children, mesh => {
								let gClone = mesh.geometry.clone(true);
								gClone.scale(0.1, 0.1, 0.1)
								gClone.rotateX(Math.PI);
								gClone.rotateY(-Math.PI / 2);
								gClone.translate(-largura / 2, 0, handleZ);
								
								return gClone
							});

							frameEsquerdo = this.mergeObjs([frameEsquerdo, ...handleGeometry], true)
						}
						else if (openingDirection ===  WindowOpeningDirection.RightLeft.value) {
							const handleGeometry = _.map(handle.children, mesh => {
								let gClone = mesh.geometry.clone(true);
								gClone.scale(0.1, 0.1, 0.1)
								gClone.rotateX(Math.PI);
								gClone.rotateY(-Math.PI / 2);
								gClone.translate(largura / 2, 0, handleZ);
								
								return gClone
							});

							frameDireito = this.mergeObjs([frameDireito, ...handleGeometry], true)
						}
					}
					else if (openingSystem === WindowOpeningType.TiltOnly.value)
					{
						const handleGeometry = _.map(handle.children, mesh => {
							let gClone = mesh.geometry.clone(true);
							gClone.scale(0.1, 0.1, 0.1)
							gClone.rotateX(Math.PI / 2);
							gClone.rotateY(-Math.PI / 2);
							gClone.translate(0, altura / 2, handleZ);
							
							return gClone
						});

						frameCima = this.mergeObjs([frameCima, ...handleGeometry], true)
					}
				}

				return [frameCima, frameBaixo, frameEsquerdo, frameDireito];
			},

			mergeObjs(objs, target = false) {
				// Mesclar as geometrias em uma única geometria
				const csgEvaluator = new Evaluator();
				csgEvaluator.useGroups = false;
				let result;

				const brushes = _.map(objs, elem => elem.isBrush ? elem : new Brush(elem))

				for (let i = 0; i < brushes.length; i++) {
					const brush = brushes[i];

					if (i === 0) {
						result = brush;
					} else {
						result = csgEvaluator.evaluate(result, brush, ADDITION, target ? brushes[0] : undefined);
					}
				}

				result.prepareGeometry();
				return result;
			},

			async get3DJanela() {
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

				const janela = new THREE.Scene();

				const pixelScale = this.pixelScale;
				const larguraPerfil = 70 * pixelScale,
					perfilAuxBorder = 45 * pixelScale,
					larguraPerfilAux = 75 * pixelScale;
				const perfilZ = 0,
					auxPerfilZ = 15 * pixelScale;

				const frameMaterial = await this.getFrameTexture();

				const glassMaterial = this.isIOS
					? new THREE.MeshStandardMaterial({
							color: 0x89cff0,
							opacity: 0.2,
							transparent: true,
					})
					: new THREE.MeshPhongMaterial({
							color: 0x89cff0,
							opacity: 0.2,
							transparent: true,
							side: THREE.DoubleSide,
							roughness: 0.3,
					});

				const glassFrostedMaterial = this.isIOS
					? new THREE.MeshStandardMaterial({
							color: 0xffffff,
							opacity: 0.99,
							transparent: true,
					})
					: new THREE.MeshPhongMaterial({
							color: 0xffffff,
							opacity: 0.99,
							transparent: true,
							side: THREE.DoubleSide,
					});

				frameMaterial.receiveShadow = true;

				// Criar as folhas da janela
				paneMatrix.forEach((row, rowIndex) => {
					row.forEach((pane, colIndex) => {
						const _x = _.sumBy(row.slice(0, colIndex), 'width'),
							x = _x * 1 * pixelScale;
						const _y = _.sumBy(paneMatrix.slice(0, rowIndex), (row) => _.maxBy(row, 'height').height),
							y = _y * -1 * pixelScale;

						// dimensão
						const larguraFolha = pane.width * pixelScale;
						const alturaFolha = pane.height * pixelScale;

						// centro(s)
						const larguraCentro = larguraFolha / 2,
							xCentro = x + larguraCentro;
						const alturaCentro = alturaFolha / 2,
							yCentro = y - alturaCentro;

						let frameFolhaSegments = this.getFrameObj(
								larguraFolha,
								alturaFolha,
								larguraPerfil,
								larguraPerfil,
								perfilZ
							),
							frameFolha = this.mergeObjs(frameFolhaSegments);

						//console.log('frameFolhaSegments', frameFolhaSegments);

						// opening system
						if (pane.openingSystem !== WindowOpeningType.Fixed.value) {
							const larguraPorta = larguraFolha - perfilAuxBorder * 2,
								alturaPorta = alturaFolha - perfilAuxBorder * 2;

							const portaSegments = this.getFrameObj(
									larguraPorta,
									alturaPorta,
									larguraPerfilAux,
									larguraPerfilAux,
									auxPerfilZ,
									pane.openingSystem,
									pane.openingDirection
								),
								porta = this.mergeObjs(portaSegments);

							frameFolha = this.mergeObjs([frameFolha, porta]);
						}

						const folha = frameFolha.isMesh ? frameFolha : new THREE.Mesh(frameFolha, frameMaterial);
						frameFolha.material = frameMaterial;
						folha.position.set(xCentro, yCentro, 0);

						// colocar a folha no lugar certo
						janela.add(folha);

						// Criar o vidro da janela
						const glassGeometry = new THREE.PlaneGeometry(larguraFolha, alturaFolha);
						const glass1 = new THREE.Mesh(
							glassGeometry,
							pane.frosted ? glassFrostedMaterial : glassMaterial
						);
						glass1.position.set(xCentro, yCentro, -10 * pixelScale);

						const glass2 = new THREE.Mesh(glassGeometry, glassMaterial);
						glass2.position.set(xCentro, yCentro, 10 * pixelScale);

						// folha dupla :D
						janela.add(glass1);
						janela.add(glass2);
					});
				});

				return janela;
			},
		},
	};
</script>
