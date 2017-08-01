var path = require('path')

function resolve(filePath) {
  return path.resolve(__dirname, filePath)
}

var define = [];
var idx = process.argv.indexOf("--define");

// console.log(process.argv);

if (idx > -1) {
  console.log("define found");
  define = [process.argv[idx + 1]];
}

console.log("Bundling for " + define + "...");

module.exports = {
    entry: resolve("src/Nightwatch.fsproj"),
    outDir: resolve("out"),
    babel: {
    //   presets: [["es2015", { modules: false }]],
    //   sourceMaps: true,
    },
    fable: {
      define: define
    }
  }