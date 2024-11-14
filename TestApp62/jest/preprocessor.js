/**
 * Your own [temporary?] transform for React Native
 */
const generate = require('@babel/generator').default;
const transformer = require('metro-react-native-babel-transformer');
const createCacheKeyFunction = require('fbjs-scripts/jest/createCacheKeyFunction');
const metroBabelRegister = require('metro-babel-register');

metroBabelRegister([]);

module.exports = {
  process(src, file) {
    const { ast } = transformer.transform({
      filename: file,
      options: {
        ast: true,
        dev: true,
        enableBabelRuntime: false,
        experimentalImportSupport: false,
        hot: false,
        inlineRequires: false,
        minify: false,
        platform: '',
        projectRoot: '',
        retainLines: true,
        sourceType: 'unambiguous',
      },
      src,
      plugins: metroBabelRegister.config.plugins,
    });

    return generate(
      ast,
      {
        code: true,
        comments: false,
        compact: false,
        filename: file,
        retainLines: true,
        sourceFileName: file,
        sourceMaps: true,
      },
      src,
    ).code;
  },

  getCacheKey: createCacheKeyFunction([
    __filename,
    require.resolve('metro-react-native-babel-transformer'),
    require.resolve('@babel/core/package.json'),
  ]),
};
