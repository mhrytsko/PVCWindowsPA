<script>
	//import {markRaw} from 'vue'

	import * as THREE from 'three';
	import {ARButton} from 'three/examples/jsm/webxr/ARButton.js';
	import {OrbitControls} from 'three/examples/jsm/controls/OrbitControls.js';
	//import { TransformControls } from 'three/examples/jsm/controls/TransformControls.js';

	import ThreeMeshUI from 'three-mesh-ui';
	import FontJSON from '@/assets/Roboto-msdf.json';
	import FontImage from '@/assets/Roboto-msdf.png';

	import {nextTick} from 'vue';

	import _ from 'lodash-es';

	import {WindowOpeningType, WindowOpeningDirection} from '@/mixins/systemEnums.js';

	const MAX_ANCHORED_OBJECTS = 1;

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

				janelaX: 1,
				janelaY: 1,

				addAnchor: false,
			};
		},

		mounted() {
			console.log('THREE mounted!');

			this.paneObjects = new Set();

			this.hitTestSource = null;
			this.hitTestSourceRequested = false;

			this.applyZIndex = -15;

			this.all_previous_anchors = new Set();
			this.anchoredObjects = [];

			nextTick(() => {
				this.initThreeJS();
				this.animate();
			});
		},

		beforeUnmount() {
			console.log('THREE Unmount!');

			this.cleanupThreeJS();

			this.janela = null;
		},

		methods: {
			initThreeJS() {
				console.log('THREE Start!');

				const container = this.$refs.container.$el;
				this.clientWidth = container.clientWidth;
				this.clientHeight = container.clientHeight;

				console.log(
					'renderer size:',
					container.clientWidth,
					container.clientHeight,
					'racio:',
					window.devicePixelRatio
				);

				// Inicializar o Renderer
				this.renderer = new THREE.WebGLRenderer({antialias: true /*, alpha: false*/});
				this.renderer.setPixelRatio(window.devicePixelRatio);
				this.renderer.setSize(container.clientWidth, container.clientHeight);

				this.renderer.xr.enabled = true;

				// TODO: voltar ativar as sombras
				//this.renderer.shadowMap.enabled = true;

				container.appendChild(this.renderer.domElement);

				container.appendChild(
					ARButton.createButton(this.renderer, {
						requiredFeatures: ['dom-overlay', 'anchors'],
					})
				);

				// Inicializar Scene
				this.scene = new THREE.Scene();
				//this.scene.add( new THREE.GridHelper( 1000, 10, 0x888888, 0x444444 ) );

				// Inicializar a Camera
				this.camera = new THREE.PerspectiveCamera(
					50,
					container.clientWidth / container.clientHeight,
					0.01,
					20000
				);
				this.camera.position.set(0, 0, 10);

				// Handler para eventos do plane detection
				this.renderer.xr.addEventListener('sessionstart', (event) => {
					//this.janela.visible = false;
					this.scene.remove(this.janela);

					//this.janela.position.z = -15;

					this.camera.aspect = window.innerWidth / window.innerHeight;
					this.camera.updateProjectionMatrix();
					this.renderer.setSize(window.innerWidth, window.innerHeight);
					this.orbit.update();

					//this.camera.position.set(0, 0, 0);
					//this.janela.scale.set(0.3, 0.3, 0.3);

					const session = event.target.getSession();
					session.addEventListener('select', this.onSelect);
				});

				// TODO: https://threejs.org/examples/#misc_controls_transform
				// https://github.com/mrdoob/three.js/blob/master/examples/misc_controls_transform.html

				// TODO: https://tympanus.net/codrops/2021/10/27/creating-the-effect-of-transparent-glass-and-plastic-in-three-js/
				// https://codesandbox.io/s/10-40wj3?from-embed=&file=/src/index.js

				// TODO: Texture
				/*
					// Crie um carregador de textura
					var textureLoader = new THREE.TextureLoader();

					// Defina a URL da imagem. Essa URL pode ser um endereço no seu servidor
					var url = 'https://seuservidor.com/imagem.jpg';

					// Use o carregador de textura para carregar a imagem
					textureLoader.load(url, function(texture) {
					// Quando a imagem for carregada, crie um material com a textura
					var material = new THREE.MeshBasicMaterial({ map: texture });

					// Use o material para criar um objeto, como um Mesh
					var mesh = new THREE.Mesh(geometry, material);

					// Adicione o objeto à cena
					scene.add(mesh);
					}, undefined, function(error) {
					// Se houver algum erro ao carregar a imagem, log o erro
					console.error('Houve um erro ao carregar a textura:', error);
					});
				*/

				// Cria um ajudante de grade com tamanho e divisões
				/*var gridHelper = new THREE.GridHelper( 50, 50 );
				this.scene.add( gridHelper );

				// Cria um ajudante de eixos com um tamanho
				var axesHelper = new THREE.AxesHelper( 25 );
				this.scene.add( axesHelper );*/

				// Inicializar Controls
				this.orbit = new OrbitControls(this.camera, this.renderer.domElement);
				this.orbit.update();
				//this.orbit.addEventListener('change', this.render);

				/*this.control = new TransformControls(this.camera, this.renderer.domElement);
				//this.control.addEventListener( 'change', this.render );

				this.control.addEventListener( 'dragging-changed', ( event ) => {
					this.orbit.enabled = !event.value;
				});

				this.scene.add( this.control );*/

				// TODO: UI
				/*this.uiCcontainer = new ThreeMeshUI.Block({
					width: 1.7,
					height: 0.95,
					padding: 0.05,
					justifyContent: 'center',
					textAlign: 'center',
					fontFamily: FontJSON,
					fontTexture: FontImage,
					fontSize: 0.05,
					interLine: 0.05,
				});
				const text = new ThreeMeshUI.Text({
					fontSize: 0.15,
					content: 'Em modo AR, toca no tela para a janela aparecer',
				});
				this.uiCcontainer.add(text);

				this.uiCcontainer.position.set(0, -2, -0.5);
				this.uiCcontainer.rotation.x = -0.55;
				this.scene.add(this.uiCcontainer);*/

				// Inicializar a Luz
				//this.light = new THREE.HemisphereLight(0xffffff, 0xbbbbff, 1);
				//this.light.position.set(0.5, 10, 0.25);
				//this.light.castShadow = true;
				this.light = new THREE.DirectionalLight(0xffffff, 2);
				this.light.position.set(1, 1, 1); // posição da luz
				this.light.castShadow = true; // a luz lança sombras
				this.light.shadow.mapSize.width = this.windowWidth; //1024; // resolução das sombras
				this.light.shadow.mapSize.height = this.windowHeight; //1024;
				this.light.shadow.camera.near = 0.1; // distância mínima
				this.light.shadow.camera.far = 500; // distância máxima
				this.scene.add(this.light);

				// Handler para o Resize da janela
				window.addEventListener('resize', this.onWindowResize);

				// Handler para o click no ecrã
				//this.onClickHandler = () => this.onSelect()
				//window.addEventListener('click', this.onClickHandler);

				// Adicionar o controller para eventos (select)
				//this.controller = this.renderer.xr.getController(0);
				//this.controller.addEventListener('select', this.onSelect);
				//this.scene.add(this.controller);

				// Desenhar os objetos
				this.rebuildScene();
			},

			cleanupThreeJS() {
				window.removeEventListener('resize', this.onWindowResize);
				//window.addEventListener('click', this.onClickHandler);
			},

			onWindowResize() {
				const container = this.$refs.container.$el;
				this.clientWidth = container.clientWidth;
				this.clientHeight = container.clientHeight;

				console.log('On Resize', container.clientWidth, container.clientHeight);

				this.camera.aspect = container.clientWidth / container.clientHeight;
				this.camera.updateProjectionMatrix();
				this.renderer.setSize(container.clientWidth, container.clientHeight);
			},

			/*onSelect(event) {
				console.log('onSelect', event)

				if(this.janela)
				{
					if(this.janela.visible)
					{
						this.scene.remove(this.janela);
						this.janela = null;
					}
					else this.addAnchor = true;
				}
				else this.addAnchor = true;
			},*/

			addAnchoredObjectToScene(anchor) {
				console.debug('Anchor created');

				let janela = this._janela.clone(true);
				this.scene.add(janela);

				_.set(anchor, 'context', {
					sceneObject: janela,
				});

				janela._anchor = anchor;
				this.anchoredObjects.push(janela);

				// For performance reasons if we add too many objects start
				// removing the oldest ones to keep the scene complexity
				// from growing too much.
				try {
					if (this.anchoredObjects.length > MAX_ANCHORED_OBJECTS) {
						let objectToRemove = this.anchoredObjects.shift();
						this.scene.remove(objectToRemove);
						objectToRemove._anchor.delete();
					}
				} catch (e) {
					console.error('Delete anchor', e);
				}
			},

			onSelect(event) {
				const frame = event.frame;
				//const xrSession = frame.session;
				const inputSource = event.inputSource;
				const xrRefSpace = this.renderer.xr.getReferenceSpace();
				const inputPose = event.frame.getPose(inputSource.targetRaySpace, xrRefSpace);

				if (inputPose) {
					const position = inputPose.transform.position;
					const orientation = inputPose.transform.orientation;

					// Create a new position one meter along the negative Z axis from the current viewer position
					let newPosition = {
						x: position.x - this.janelaX / 2,
						y: position.y + this.janelaY / 2,
						z: position.z - Math.max(this.janelaX, this.janelaY) / 2,
						//w: position.w,
					};

					let anchorPose = new XRRigidTransform(newPosition, orientation /*{x: 0, y: 0, z: 0, w: 1}*/);

					// Create a free-floating anchor.
					frame.createAnchor(anchorPose, inputSource.targetRaySpace).then(
						(anchor) => {
							this.addAnchoredObjectToScene(anchor);
						},
						(error) => {
							console.error('Could not create anchor: ' + error);
						}
					);
				}
			},

			getObjectSize(obj3D) {
				// Calcular a caixa de delimitação
				let box = new THREE.Box3().setFromObject(obj3D);
				let size = new THREE.Vector3();
				box.getSize(size);

				return size;
			},

			comSombras(obj3D) {
				obj3D.castShadow = true; // o objeto lança sombras
				obj3D.receiveShadow = true; // o objeto recebe sombras
				return obj3D;
			},

			get3DJanela() {
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

				const pixelScale = 0.01; //this.pixelScale
				const larguraPerfil = 70 * pixelScale,
					perfilAuxBorder = 45 * pixelScale,
					larguraPerfilAux = 75 * pixelScale;
				const increaseFrameH = larguraPerfil,
					perfilZ = 0,
					auxPerfilZ = 0.15;

				const frameMaterial = new THREE.MeshBasicMaterial({color: 0xffffff, side: THREE.DoubleSide});
				const frameAuxMaterial = new THREE.MeshBasicMaterial({color: 0xf8f8ff, side: THREE.DoubleSide});
				const glassMaterial = new THREE.MeshBasicMaterial({
					color: 0x89cff0,
					opacity: 0.2,
					transparent: true,
					side: THREE.DoubleSide,
				});
				const glassFrostedMaterial = new THREE.MeshBasicMaterial({
					color: 0xffffff,
					opacity: 0.6,
					transparent: true,
					side: THREE.DoubleSide,
				});

				frameMaterial.receiveShadow = true;
				frameAuxMaterial.receiveShadow = true;

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

						// grupo para a folha inteira
						const folhaJanela = new THREE.Group();
						{
							// frames / border
							const frameHorizontal = new THREE.BoxGeometry(
								larguraFolha + increaseFrameH,
								larguraPerfil,
								larguraPerfil
							);
							const frameVertical = new THREE.BoxGeometry(larguraPerfil, alturaFolha, larguraPerfil);

							// objetos 3D da folha
							const frameCima = new THREE.Mesh(frameHorizontal, frameMaterial);
							const frameBaixo = new THREE.Mesh(frameHorizontal, frameMaterial);
							const frameEsquerdo = new THREE.Mesh(frameVertical, frameMaterial);
							const frameDireito = new THREE.Mesh(frameVertical, frameMaterial);

							// Possicionar as frames no sitio certo
							frameCima.position.set(xCentro, y - alturaFolha, perfilZ);
							folhaJanela.add(this.comSombras(frameCima));

							frameBaixo.position.set(xCentro, y, perfilZ);
							folhaJanela.add(this.comSombras(frameBaixo));

							frameEsquerdo.position.set(x, yCentro, perfilZ);
							folhaJanela.add(this.comSombras(frameEsquerdo));

							frameDireito.position.set(x + larguraFolha, yCentro, perfilZ);
							folhaJanela.add(this.comSombras(frameDireito));
						}

						// opening system
						if (pane.openingSystem !== WindowOpeningType.Fixed.value) {
							// frames / border
							const frameHorizontal = new THREE.BoxGeometry(
								larguraFolha + increaseFrameH - perfilAuxBorder * 2,
								larguraPerfilAux,
								larguraPerfilAux
							);
							const frameVertical = new THREE.BoxGeometry(
								larguraPerfilAux,
								alturaFolha - perfilAuxBorder * 2,
								larguraPerfilAux
							);

							// objetos 3D da folha
							const frameCima = new THREE.Mesh(frameHorizontal, frameAuxMaterial);
							const frameBaixo = new THREE.Mesh(frameHorizontal, frameAuxMaterial);
							const frameEsquerdo = new THREE.Mesh(frameVertical, frameAuxMaterial);
							const frameDireito = new THREE.Mesh(frameVertical, frameAuxMaterial);

							// Possicionar as frames no sitio certo
							frameCima.position.set(xCentro, y - alturaFolha + perfilAuxBorder, auxPerfilZ);
							folhaJanela.add(this.comSombras(frameCima));

							frameBaixo.position.set(xCentro, y - perfilAuxBorder, auxPerfilZ);
							folhaJanela.add(this.comSombras(frameBaixo));

							frameEsquerdo.position.set(x + perfilAuxBorder, yCentro, auxPerfilZ);
							folhaJanela.add(this.comSombras(frameEsquerdo));

							frameDireito.position.set(x + larguraFolha - perfilAuxBorder, yCentro, auxPerfilZ);
							folhaJanela.add(this.comSombras(frameDireito));
						}

						// mais para futuro...
						//folhaJanela.userData = pane;

						// Criar o vidro da janela
						const glassGeometry = new THREE.PlaneGeometry(larguraFolha, alturaFolha);
						const glass1 = new THREE.Mesh(
							glassGeometry,
							pane.frosted ? glassFrostedMaterial : glassMaterial
						);
						glass1.position.set(xCentro, yCentro, -0.1);

						const glass2 = new THREE.Mesh(glassGeometry, glassMaterial);
						glass2.position.set(xCentro, yCentro, 0.1);

						folhaJanela.add(glass1);
						folhaJanela.add(glass2);

						// colocar a folha no lugar certo
						janela.add(folhaJanela);
					});
				});

				return janela;
			},

			rebuildScene() {
				this._janela = this.get3DJanela();

				const size = this.getObjectSize(this._janela);
				this._janela.position.set((size.x / 2) * -1, size.y / 2, -2);

				this.janelaX = size.x;
				this.janelaY = size.y;

				this.camera.position.set(0, 0, Math.max(size.x, size.y) * 2);

				if (this.janela) this.scene.remove(this.janela);
				this.janela = this._janela.clone(true);

				this.scene.add(this.janela);

				//this.control.attach( this.janela );

				// Faça o objeto olhar para a câmera
				//this.janela.lookAt(this.camera.position);
			},

			animate() {
				this.renderer.setAnimationLoop(this.render);
				this.orbit.update();
			},

			// Função para atualizar a posição e a orientação do objeto
			/*updateObjectPositionAndOrientation() {
				// Obtém a matriz de transformação da câmera
				const cameraMatrix = this.camera.matrixWorld;

				// Extrai a posição e a orientação da câmera
				const cameraPosition = new THREE.Vector3();
				const cameraQuaternion = new THREE.Quaternion();
				const cameraScale = new THREE.Vector3();
				cameraMatrix.decompose(cameraPosition, cameraQuaternion, cameraScale);

				// Deslocamento para a frente da câmera
				const offset = new THREE.Vector3(0, 0, -15); 
				const objectPosition = cameraPosition.clone().add(offset);


				// Define a posição do objeto igual à posição da câmera
				this.janela.position.copy(objectPosition);

				// Orienta o objeto para a direção da câmera
				this.janela.quaternion.copy(cameraQuaternion);

				// Ajusta a rotação do objeto para que ele esteja voltado para a câmera
				//this.janela.lookAt(cameraPosition);
			},*/

			// Called every time a XRSession requests that a new frame be drawn.
			render(_, frame) {
				//this.updateObjectPositionAndOrientation()
				if (frame) {
					const xrRefSpace = this.renderer.xr.getReferenceSpace();
					//var a = this.renderer.xr.XRBoundedReferenceSpace();

					const tracked_anchors = frame.trackedAnchors;
					if (tracked_anchors) {
						this.all_previous_anchors.forEach((anchor) => {
							if (!tracked_anchors.has(anchor)) {
								this.scene.remove(anchor.sceneObject);
							}
						});

						tracked_anchors.forEach((anchor) => {
							try {
								const anchorPose = frame.getPose(anchor.anchorSpace, xrRefSpace);
								if (anchorPose) {
									//anchor.context.sceneObject.matrix = anchorPose.transform.matrix;
									
									let matrix = new THREE.Matrix4();
									matrix.fromArray(anchorPose.transform.matrix);

									// Atualizar a posição e rotação da janela com base na pose do anchor
									anchor.context.sceneObject.matrix.fromArray(anchorPose.transform.matrix);
									anchor.context.sceneObject.position.setFromMatrixPosition(matrix);
									anchor.context.sceneObject.quaternion.setFromRotationMatrix(matrix);
									anchor.context.sceneObject.rotation.set(0, 0, 0);

									anchor.context.sceneObject.visible = true;
								} else {
									anchor.context.sceneObject.visible = false;
								}
							} catch (e) {
								console.error('Obter a pose do anchor', e);
							}
						});

						this.all_previous_anchors = tracked_anchors;
					} else {
						this.all_previous_anchors.forEach((anchor) => {
							this.scene.remove(anchor.sceneObject);
						});

						this.all_previous_anchors = new Set();
					}

					/*if(this.addAnchor)
					{
						this.addAnchor = false;
						const pose = frame.getViewerPose(xrRefSpace);

						// Usa o frame para obter a pose da câmera
						if (pose)
						{
							// Create a new position one meter along the negative Z axis from the current viewer position
							let newPosition = {
								x: pose.transform.position.x - (this.janelaX / 2),
								y: pose.transform.position.y + (this.janelaY / 2),
								z: pose.transform.position.z - (Math.max(this.janelaX, this.janelaY) / 2),
							};

							// Cria um novo anchor com a pose da câmera
							//const anchorPose = new XRRigidTransform(pose.transform.position, pose.transform.orientation);
							// Create a new XRRigidTransform using the new position and the orientation from the viewer pose
							let anchorPose = new XRRigidTransform(
								newPosition, 
								pose.transform.orientation
							);

							frame.trackedAnchors.forEach((_anchor) => {
									if(!this.janela || _anchor != this.janela._anchor)
										_anchor.delete();
								});
							

							frame.createAnchor(anchorPose, xrRefSpace).then((anchor) => {
								// Armazena o anchor para uso posterior
								if(this.janela)
									this.scene.remove(this.janela)

								this.janela = this._janela.clone(true);
								this.janela._anchor = anchor;
								this.scene.add(this.janela);

							}, (error) => {
								console.error("Could not create anchor: " + error);
							});
						}
					}

					if(this.janela && this.janela.visible && this.janela._anchor)
					{
						try
						{
							// Obter a pose do anchor
							const anchorPose = frame.getPose(this.janela._anchor.anchorSpace, xrRefSpace);
							if (anchorPose) {

								let matrix = new THREE.Matrix4();
								matrix.fromArray(anchorPose.transform.matrix);
								
								//this.janela.matrixAutoUpdate = false;

								// Atualizar a posição e rotação da janela com base na pose do anchor
								this.janela.matrix.fromArray(anchorPose.transform.matrix); 
								this.janela.position.setFromMatrixPosition(matrix);
								this.janela.quaternion.setFromRotationMatrix(matrix);

								//this.janela.lookAt(0, 0, 0)
								this.janela.rotation.set(0, 0, 0);

								//console.log(this.janela.position)
							}
						}
						catch(e)
						{
							console.error("Obter a pose do anchor", e);
						}
					}*/
				}

				//ThreeMeshUI.update();

				this.renderer.render(this.scene, this.camera);
			},
		},
	};
</script>

<template>
	<v-container ref="container" class="fill-height ma-0 pa-0" min-height="50px"> </v-container>
</template>
