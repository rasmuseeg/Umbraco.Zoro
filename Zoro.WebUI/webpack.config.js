/// <binding ProjectOpened='Watch - Development' />
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
    entry: __dirname + '/assets/scss/styles.scss',
    output: {
        filename: '[name].js',
        chunkFilename: "[id].js",
        path: __dirname + '/assets/dist'
    },
    mode: process.env.NODE_ENV,
    devtool: "source-map",
    module: {
        rules: [
            {
                test: /\.css$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    "css-loader",
                    "postcss-loader"
                ]
            },
            {
                test: /\.scss$/,
                use: [
                    MiniCssExtractPlugin.loader,
                    { loader: 'css-loader', options: { importLoaders: 1, sourceMap: true } },
                    { loader: 'postcss-loader' },
                    { loader: "sass-loader", options: { sourceMap: true } }
                ]
            },
            {
                test: /\.m?js$/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader",
                    options: {
                        presets: [
                            [
                                "@babel/preset-env",
                                {
                                    "targets": {
                                        "ie": "11"
                                    }
                                }
                            ]
                        ]
                    }
                }
            }]
    },
    plugins: [
        new ExtractTextPlugin('styles.css', {
            disable: process.env.NODE_ENV === 'development'
        }),
        new MiniCssExtractPlugin({
            // Options similar to the same options in webpackOptions.output
            // both options are optional
            filename: "[name].css",
            chunkFilename: "[id].css"
        }),
        require('autoprefixer'),
        new webpack.ProvidePlugin({
            Globalize: "globalize"
        })
    ]
};
