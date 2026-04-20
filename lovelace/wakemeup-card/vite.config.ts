import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/index.ts",
      formats: ["es"],
      fileName: () => "wakemeup-card.js",
    },
    outDir: "dist",
    sourcemap: false,
    emptyOutDir: true,
  },
});

