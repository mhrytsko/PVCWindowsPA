<script>
	//import 'https://unpkg.com/es-module-shims@1.6.3/dist/es-module-shims.js';

	import * as THREE from 'three';
	import {ARButton} from 'three/examples/jsm/webxr/ARButton.js';
	//import {XRButton} from 'three/examples/jsm/webxr/XRButton.js';
	import {XRPlanes} from 'three/examples/jsm/webxr/XRPlanes.js';

	export default {
		data() {
			return {
				renderer: undefined,
				scene: undefined,
				planes: undefined,
				camera: undefined,
				light: undefined,
				planesAdded: new Set(),
			};
		},

		mounted() {
			console.log('THREE mounted!');
			this.create3DWindow();
			this.animate();
		},

		methods: {
			create3DWindow() {
				console.log('THREE Start!');

				// Inicializar o Renderer
				this.renderer = new THREE.WebGLRenderer({antialias: true/*, alpha: false*/});
				this.renderer.setPixelRatio(window.devicePixelRatio);
				this.renderer.setSize(/*window.innerWidth, window.innerHeight*/ window.innerWidth, 75);

				this.renderer.xr.enabled = true;

				this.$refs.container.appendChild(this.renderer.domElement);

				this.$refs.container.appendChild(
					ARButton.createButton(this.renderer, {
						requiredFeatures: ['hit-test', 'dom-overlay', 'plane-detection'],
					})
				);

				/*this.$refs.container.appendChild(
					XRButton.createButton(this.renderer, {
						requiredFeatures: ['plane-detection'],
					})
				);*/

				// Inicializar Scene
				this.scene = new THREE.Scene();

				// Inicializar o Planes
				this.planes = new XRPlanes(this.renderer);
				this.scene.add(this.planes);

				// Inicializar a Camera
				this.camera = new THREE.PerspectiveCamera(70, window.innerWidth / window.innerHeight, 0.01, 20);

				// Inicializar a Luz
				this.light = new THREE.HemisphereLight(0xffffff, 0xbbbbff, 1);
				this.light.position.set(0.5, 1, 0.25);
				this.scene.add(this.light);

				// Handler para o Resize da janela
				window.addEventListener('resize', this.onWindowResize);

				// Handler para eventos do plane detection
				this.renderer.xr.addEventListener('sessionstart', function () {
					//this.camera.position.set(0, 0, 0);
				});

				this.renderer.xr.addEventListener('planeadded', function (e) {
					console.log('plane added', e.data);
				});

				this.renderer.xr.addEventListener('planeremoved', function (e) {
					console.log('plane removed', e.data);
				});

				this.renderer.xr.addEventListener('planechanged', function (e) {
					console.log('plane changed', e.data);
				});

				this.renderer.xr.addEventListener('planesdetected', function (e) {
					const detectedPlanes = e.data;
					const referenceSpace = this.renderer.xr.getReferenceSpace();

					console.log(`Detected ${detectedPlanes.size} planes`);

					detectedPlanes.forEach((plane) => {
						if (this.planesAdded.has(plane)) return;

						this.planesAdded.add(plane);
						const frame = this.renderer.xr.getFrame();
						const planePose = frame.getPose(plane.planeSpace, referenceSpace);
						//const planeGeometry = new THREE.BufferGeometry();
						const polygon = plane.polygon;

						let minX = Number.MAX_SAFE_INTEGER;
						let maxX = Number.MIN_SAFE_INTEGER;
						let minZ = Number.MAX_SAFE_INTEGER;
						let maxZ = Number.MIN_SAFE_INTEGER;

						polygon.forEach((point) => {
							minX = Math.min(minX, point.x);
							maxX = Math.max(maxX, point.x);
							minZ = Math.min(minZ, point.z);
							maxZ = Math.max(maxZ, point.z);
						});

						const width = maxX - minX;
						const height = maxZ - minZ;

						const boxMesh = new THREE.Mesh(
							new THREE.BoxGeometry(width, 0.01, height),
							new THREE.MeshBasicMaterial({color: 0xffffff * Math.random()})
						);
						boxMesh.matrixAutoUpdate = false;
						boxMesh.matrix.fromArray(planePose.transform.matrix);
						this.scene.add(boxMesh);
					});
				});
			},

			onWindowResize() {
				this.camera.aspect = window.innerWidth / window.innerHeight;
				this.camera.updateProjectionMatrix();

				//this.renderer.setSize(window.innerWidth, window.innerHeight);
			},

			render() {
				this.renderer.render(this.scene, this.camera);
			},

			animate() {
				this.renderer.setAnimationLoop(this.render);
			},
		},
	};
</script>

<template>
	<div>
		<v-row>
			<v-col class="align-left text-left">
				<v-responsive min-height="50px">
					<div ref="container" style="position: relative; min-height: 50px;"></div>
				</v-responsive>
			</v-col>
		</v-row>
	</div>
</template>
