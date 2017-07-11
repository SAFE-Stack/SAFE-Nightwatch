var path = require('path')
var fable = require('rollup-plugin-fable')

function resolve(filePath) {
  return path.resolve(__dirname, filePath)
}

var isProduction = process.argv.indexOf("-w") === -1;
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");

export default {
  entry: resolve('src/Nightwatch.fsproj'),
  dest: resolve('out/Nightwatch.js'),
  format: 'es', // 'amd', 'cjs', 'es', 'iife', 'umd'
  //sourceMap: 'inline',
  plugins: [fable({
    extra: { failOnError: true, useCache: true },
    define: isProduction ? ["PRODUCTION"] : ["DEBUG"]
  })],
  external: [
    "buffer",
    "react",
    "react-native",
    "react-native-onesignal",
    "react-native-image-picker"
  ]
};
