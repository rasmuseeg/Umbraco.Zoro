const path = require('path');
const tailwindcss = require('tailwindcss');
const ExtractTextPlugin = require('extract-text-webpack-plugin')
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
    entry: './assets/scss/styles.scss',
    mode: process.env.NODE_ENV,
    devtool: "source-map",
    module: {
        rules: [{
            test: /\.scss$/,
            use: [
                    { loader: 'style-loader', options: { sourceMap: true } },
                    { loader: 'css-loader', options: { sourceMap: true } },
                    {
                        loader: 'postcss-loader'
                    },
                    { loader: "sass-loader", options: { sourceMap: true } }
            ]
        }]
    },
    plugins: [
        new ExtractTextPlugin('styles.scss', {
            disable: process.env.NODE_ENV === 'development',
        }),
        new MiniCssExtractPlugin({
            // Options similar to the same options in webpackOptions.output
            // both options are optional
            filename: "[name].css",
            chunkFilename: "[id].css"
        }),
        require('autoprefixer')
    ]
};
