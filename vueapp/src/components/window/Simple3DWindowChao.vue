<script>
	//import 'https://unpkg.com/es-module-shims@1.6.3/dist/es-module-shims.js';

	//import {markRaw} from 'vue'

	import * as THREE from 'three';
	import {ARButton} from 'three/examples/jsm/webxr/ARButton.js';
	import {OrbitControls} from 'three/examples/jsm/controls/OrbitControls.js';
	import { nextTick } from 'vue';

	//import {Node} from 'three/render/core/node.js';

	import _ from 'lodash-es'

	import {WindowOpeningType, WindowOpeningDirection} from '@/mixins/systemEnums.js';

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
		},

		data() {
			return {

				pixelScale: 0.01,
				clientWidth: 0,
				clientHeight: 0,


				/*renderer: undefined,
				scene: undefined,
				camera: undefined,
				controls: undefined,
				light: undefined,
				controller: undefined,
				raycaster: undefined*/
				//paneObjects: new Set(),
			};
		},

		mounted() {
			console.log('THREE mounted!');

			this.paneObjects = new Set();

			this.hitTestSource = null;
			this.hitTestSourceRequested = false;

			nextTick(() => {
				this.initThreeJS();
				//this.create3DWindow();
				this.animate();
			})
			
		},

		beforeUnmount() {
			console.log('THREE Unmount!');

			this.cleanupThreeJS();
		},

		methods: {
			initThreeJS() {
				console.log('THREE Start!');

				const container = this.$refs.container.$el;
				this.clientWidth = container.clientWidth;
				this.clientHeight = container.clientHeight;

				console.log("renderer size:", container.clientWidth, container.clientHeight, "racio:", window.devicePixelRatio)

				// Inicializar o Renderer
				this.renderer = new THREE.WebGLRenderer({antialias: true /*, alpha: false*/});
				this.renderer.setPixelRatio(window.devicePixelRatio);
				this.renderer.setSize(/*window.innerWidth, window.innerHeight*/container.clientWidth, container.clientHeight);

				this.renderer.xr.enabled = true;

				container.appendChild(this.renderer.domElement);

				container.appendChild(
					ARButton.createButton(this.renderer, {
						requiredFeatures: ['hit-test', 'dom-overlay'],
					})
				);

				// Inicializar Scene
				this.scene = new THREE.Scene();

				// Inicializar a Camera
				this.camera = new THREE.PerspectiveCamera(70, /*window.innerWidth / window.innerHeight*/container.clientWidth / container.clientHeight, 0.01, 2000);
				this.camera.position.set(0, 0, 10);
				//this.camera.position.z = 5; //this.camera.position.set(0, 0, 0);

				// Handler para eventos do plane detection
				this.renderer.xr.addEventListener('sessionstart', () => {
					//this.camera.position.set(0, 0, 0);

					this.janela.visible = false;
				});

				// Inicializar Controls
				this.controls = new OrbitControls(this.camera, this.renderer.domElement);

				// Inicializar a Luz
				this.light = new THREE.HemisphereLight(0xffffff, 0xbbbbff, 1);
				this.light.position.set(0.5, 1, 0.25);
				this.scene.add(this.light);

				// Handler para o Resize da janela
				window.addEventListener('resize', this.onWindowResize);

				// Adicionar o controller para eventos (select)
				this.controller = this.renderer.xr.getController(0);
				this.controller.addEventListener('select', this.onSelect);
				this.scene.add(this.controller);

				// ??
				this.reticle = new THREE.Mesh(
					new THREE.RingGeometry(0.15, 0.2, 32).rotateX(-Math.PI / 2),
					new THREE.MeshBasicMaterial()
				);
				this.reticle.matrixAutoUpdate = false;
				this.reticle.visible = false;
				this.scene.add(this.reticle);


				// 1. Inicializar o Raycaster
				//this.raycaster = new THREE.Raycaster();
				//this.mouse = new THREE.Vector2();

				// Desenhar os objetos
				this.rebuildScene();
			},

			cleanupThreeJS() {
				window.removeEventListener('resize', this.onWindowResize);
				//this.renderer.domElement.removeEventListener('click', this.onPaneClick);
			},

			onWindowResize() {
				const container = this.$refs.container.$el;
				this.clientWidth = container.clientWidth;
				this.clientHeight = container.clientHeight;

				console.log("On Resize", container.clientWidth, container.clientHeight)

				this.camera.aspect = container.clientWidth / container.clientHeight;//window.innerWidth / window.innerHeight;
				this.camera.updateProjectionMatrix();
				this.renderer.setSize(/*window.innerWidth, window.innerHeight*/container.clientWidth, container.clientHeight);
			},

			placeWindow() {
				// Suponha que a janela esteja 1 unidade à frente do dispositivo
				let offset = new THREE.Vector3(0, 0, -1);
				offset.applyQuaternion(this.camera.quaternion);

				this.janela.position.copy(this.camera.position);
				this.janela.position.add(offset);

				// Fazer a janela olhar para o dispositivo
				this.janela.lookAt(this.camera.position);
			},

			onSelect(/*event*/) {
				this.janela.visible = !this.janela.visible
				this.placeWindow()

				

				//if (this.reticle.visible) {
					/*const geometry = new THREE.CylinderGeometry(0.1, 0.1, 0.2, 32).translate(0, 0.1, 0);

					const material = new THREE.MeshPhongMaterial({color: 0xffffff * Math.random()});
					const mesh = new THREE.Mesh(geometry, material);
					this.reticle.matrix.decompose(mesh.position, mesh.quaternion, mesh.scale);
					mesh.scale.y = Math.random() * 2 + 1;
					this.scene.add(mesh);*/

					//this.janela.position.z = this.reticle.position.z
					/*let size = this.getObjectSize(this.janela)
					let scale = */

					//this.reticle.matrix.decompose(this.janela.position, this.janela.quaternion, this.janela.scale);
					//this.janela.matrix = this.reticle.matrix;
					//this.janela.visible = true;
				//}
				//else
					//this.janela.visible = false


				// Calcular objetos que intersectam a linha de picking
				/*let intersects = this.raycaster.intersectObjects(this.scene.children, true);

				console.log('onSelect', event, intersects)

				for (let i = 0; i < intersects.length; i++) {
					// Código para interagir com o objeto que foi tocado/clicado.
					// Por exemplo, você pode mudar sua cor, mover-se para uma nova posição, etc.
					// Neste caso, definimos este objeto como uma "parede" onde outros objetos podem ser posicionados.

					intersects[i].object.material.color.set(0xff0000);
				}*/
			},

			getObjectSize(obj3D)
			{
				// Calcular a caixa de delimitação
				let box = new THREE.Box3().setFromObject(obj3D);
				let size = new THREE.Vector3();
				box.getSize(size);

				return size;
			},

			get3DJanela()
			{
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

		
				const janela = new THREE.Group();

				const pixelScale = 0.01;//this.pixelScale
				const larguraPerfil = 70 * pixelScale;
				const increaseFrameH = 0.0;

				const frameMaterial = new THREE.MeshBasicMaterial({color: 0xffffff, side: THREE.DoubleSide});
				const glassMaterial = new THREE.MeshBasicMaterial({color: 0x89CFF0, opacity: 0.2, transparent: true, side: THREE.DoubleSide});


				// Criar as folhas da janela
				paneMatrix.forEach((row, rowIndex) => {
					row.forEach((pane, colIndex) => {
						const _x = _.sumBy(row.slice(0, colIndex), 'width'),
							x = _x * pixelScale;
						const _y = _.sumBy(paneMatrix.slice(0, rowIndex), (row) => _.maxBy(row, 'height').height),
							y = _y * pixelScale;

						// dimensão
						const larguraFolha = pane.width * pixelScale;
						const alturaFolha = pane.height * pixelScale;

						// centro(s)
						const larguraCentro = larguraFolha / 2,
							xCentro = x + larguraCentro;
						const alturaCentro = alturaFolha / 2,
							yCentro = y + alturaCentro;

						// frames / border
						const frameHorizontal = new THREE.BoxGeometry(larguraFolha + increaseFrameH, larguraPerfil, larguraPerfil);
						const frameVertical = new THREE.BoxGeometry(larguraPerfil, alturaFolha, larguraPerfil);

						// grupo para a folha inteira
						const folhaJanela = new THREE.Group();

						// objetos 3D da folha
						const frameCima = new THREE.Mesh(frameHorizontal, frameMaterial);
						const frameBaixo = new THREE.Mesh(frameHorizontal, frameMaterial);
						const frameEsquerdo = new THREE.Mesh(frameVertical, frameMaterial);
						const frameDireito = new THREE.Mesh(frameVertical, frameMaterial);

						// Possicionar as frames no sitio certo
						frameCima.position.set(xCentro, y + alturaFolha, 0);
						folhaJanela.add(frameCima);

						
						frameBaixo.position.set(xCentro, y, 0);
						folhaJanela.add(frameBaixo);

						
						frameEsquerdo.position.set( x, yCentro, 0);
						folhaJanela.add(frameEsquerdo);

						
						frameDireito.position.set( x + larguraFolha, yCentro, 0);
						folhaJanela.add(frameDireito);

						// mais para futuro...
						//folhaJanela.userData = pane;

						// Criar o vidro da janela
						const glassGeometry = new THREE.PlaneGeometry(larguraFolha, alturaFolha);
						const glass = new THREE.Mesh(glassGeometry, glassMaterial);
						glass.position.set( xCentro , yCentro, 0);

						folhaJanela.add(glass);

						// colocar a folha no lugar certo
						janela.add(folhaJanela)
					});
				});

				return janela;
			},

			rebuildScene()
			{
				const janela = this.get3DJanela();

				const size = this.getObjectSize(janela);
				janela.position.set((size.x / 2) * -1, (size.y / 2) * -1)
				this.camera.position.set(0, 0, Math.max(size.x, size.y) * 2)

				if(this.janela)
					this.scene.remove(this.janela)
				this.janela = janela

				this.scene.add(this.janela);
			},

			animate() {
				this.renderer.setAnimationLoop(this.render);
				this.controls.update();
			},

			render(timestamp, frame) {
				if (frame) {
					const referenceSpace = this.renderer.xr.getReferenceSpace();
					const session = this.renderer.xr.getSession();

					if (this.hitTestSourceRequested === false) {
						session.requestReferenceSpace('viewer').then((referenceSpace) => {
							session.requestHitTestSource({space: referenceSpace}).then((source) => {
								this.hitTestSource = source;
							});
						});

						session.addEventListener('end', () => {
							this.hitTestSourceRequested = false;
							this.hitTestSource = null;
						});

						this.hitTestSourceRequested = true;
					}

					if (this.hitTestSource) {
						const hitTestResults = frame.getHitTestResults(this.hitTestSource);

						if (hitTestResults.length) {
							//console.log("hitTestResults", hitTestResults)
							const hit = hitTestResults[0];

							// coloque o objeto 3D na posição detectada pelo hit test
							this.reticle.visible = true;
							this.reticle.matrix.fromArray(hit.getPose(referenceSpace).transform.matrix);

							//this.janela.visible = true;
							//this.janela.matrix.fromArray(hit.getPose(referenceSpace).transform.matrix);
						} else {
							this.reticle.visible = false;
							//this.janela.visible = false;
						}
					}
				}

				//// Atualiza a origem e direção do raycaster
				//this.raycaster.setFromCamera(this.controller.position, this.camera);

				this.renderer.render(this.scene, this.camera);
			},
		},

		watch: {
			/*windowWidth() {
				this.rebuildScene();
			},
			windowHeight() {
				this.rebuildScene();
			},
			profileWidth() {
				this.rebuildScene();
			},
			panes: {
				handler() {
					this.rebuildScene();
				},
				deep: true,
			},*/
		},
	};
</script>

<template>
	<v-container ref="container" class="fill-height ma-0 pa-0" min-height="50px">
	</v-container>
</template>
