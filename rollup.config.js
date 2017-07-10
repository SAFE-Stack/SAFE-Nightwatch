var path = require('path')
var fable = require('rollup-plugin-fable')
var babel = require('rollup-plugin-babel')

function resolve(filePath) {
  return path.resolve(__dirname, filePath)
}

var isProduction = process.argv.indexOf("-w") === -1;
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

var babelOptions = {
  presets: [["es2015", { "modules": false }]],
  plugins: ["external-helpers"]
};

export default {
  entry: resolve('src/Nightwatch.fsproj'),
  dest: resolve('out/Nightwatch.js'),
  format: 'es', // 'amd', 'cjs', 'es', 'iife', 'umd'
  //sourceMap: 'inline',
  plugins: [
    fable({
      babel: babelOptions,
      extra: { failOnError: true },
      define: isProduction ? ["PRODUCTION"] : ["DEBUG"]
    }),
    babel({
      exclude: 'node_modules/**',
      presets: babelOptions.presets,
      plugins: babelOptions.plugins,
      externalHelpers: true
    })
  ],
  external: [
    "buffer",
    "react",
    "react-native",
    "react-native-onesignal",
    "react-native-image-picker"
  ]
};