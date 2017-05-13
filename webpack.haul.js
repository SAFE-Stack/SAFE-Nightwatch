const babelOptions = {
  presets: [['es2015', { modules: false }]],
  plugins: ['transform-runtime']
};

module.exports = ({ platform }, defaults) => {
  const conf = Object.assign({}, defaults);

  conf.module.rules = [
    {
      test: /\.fs(x|proj)?$/,
      use: {
        loader: 'fable-loader',
        options: { babel: babelOptions }
      }
    },
    ...conf.module.rules
  ];

  conf.entry = './src/Nightwatch.fsproj';

  return conf;
};
