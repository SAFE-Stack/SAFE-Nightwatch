var path = require('path')
var fable = require('rollup-plugin-fable')

function resolve(filePath) {
  return path.resolve(__dirname, filePath)
}

export default {
  entry: resolve('Nightwatch.fsproj'),
  dest: resolve('out/Nightwatch.js'),
  format: 'es', // 'amd', 'cjs', 'es', 'iife', 'umd'
  //sourceMap: 'inline',
  plugins: [fable()],
  external: [
    "buffer",
    "react",
    "react-native",
    "react-native-onesignal",
    "react-native-image-picker"
  ]
};