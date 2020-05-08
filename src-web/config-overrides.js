const { override, fixBabelImports, addLessLoader, addWebpackPlugin } = require('customize-cra');
const AntdDayjsWebpackPlugin = require('antd-dayjs-webpack-plugin');

module.exports = override(
    fixBabelImports('antd', {
        libraryDirectory: 'es',
        style: true,
    }),
    addLessLoader({
        lessOptions: {
            javascriptEnabled: true,
            modifyVars: {},
        },
    }),
    addWebpackPlugin(new AntdDayjsWebpackPlugin())
);