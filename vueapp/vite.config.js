import vuetify, { transformAssetUrls } from 'vite-plugin-vuetify'
import { defineConfig, splitVendorChunkPlugin } from "vite";
import vue from "@vitejs/plugin-vue";
import path from "path";

//import AutoImport from "unplugin-auto-import/vite";
//import Components from "unplugin-vue-components/vite";

//import fs from "fs";

const baseFolder =
  process.env.APPDATA !== undefined && process.env.APPDATA !== ""
    ? `${process.env.APPDATA}/ASP.NET/https`
    : `${process.env.HOME}/.aspnet/https`;

const certificateArg = process.argv
  .map((arg) => arg.match(/--name=(?<value>.+)/i))
  .filter(Boolean)[0];
const certificateName = certificateArg ? certificateArg.groups.value : "vueapp";

if (!certificateName) {
  console.error(
    "Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly."
  );
  process.exit(-1);
}

const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

const pathSrc = path.resolve(__dirname, "src");

// https://vitejs.dev/config/
export default defineConfig({
  base: './',//process.env.NODE_ENV === "production" ? "/ProjetoAcademico/" : "/",
  resolve: {
    alias: {
      "@/": `${pathSrc}/`,
    },
  },
  // css: {
  //   preprocessorOptions: {
  //     scss: {
  //       additionalData: `@use "~/styles/element/index.scss" as *;`,
  //     },
  //   },
  // },
  build: {
    sourcemap: process.env.NODE_ENV === "production" ? false : true,
    rollupOptions: {
      output: {
        //manualChunks: {
        //    vue: ['vue'],
        //    lodash: ['lodash-es']
        //}
      },
    },
  },
  optimizeDeps: {
    include: ["vue", "lodash-es", "unocss"],
    // Limpar o código não utilizado
    treeshake: true,
  },
  plugins: [
    vue({ 
      template: { transformAssetUrls },
      customElement: ['model-viewer']
    }),
    // https://github.com/vuetifyjs/vuetify-loader/tree/next/packages/vite-plugin
    vuetify({
      autoImport: true,
    }),
    splitVendorChunkPlugin(),
  ],
  server: {
    //https: {
    //    key: fs.readFileSync(keyFilePath),
    //    cert: fs.readFileSync(certFilePath),
    //},
    proxy: {
      "/api": {
        target: "https://localhost:44336",
        /*ws: true,*/
        changeOrigin: true,
        secure: false, // Desative a verificação de certificado SSL | estamos usar um certificado SSL autoassinado em um ambiente de desenvolvimento local
        rewrite: (urlPath) => urlPath, //.replace(/^\/api/, '')
      },
    },
    open: true,
    port: 5002,
  },
});
