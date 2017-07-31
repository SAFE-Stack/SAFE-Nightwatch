var path = require('path');

function resolve(filePath) {
  return path.resolve(__dirname, filePath);
}

var isProduction = process.argv.indexOf("-w") === -1;
console.log("Compiling for " + (isProduction ? "production" : "development") + "...");

module.exports = {
    entry: "src/Nightwatch.fsproj",
    outDir: "out",
    babel: {
    //   presets: [["es2015", { modules: false }]],
      sourceMaps: true,
    },
    fable: {
      define: isProduction ? ["PRODUCTION"] : ["DEBUG"],
      fableCore: resolve("../Fable/build/fable-core")
    }
  }