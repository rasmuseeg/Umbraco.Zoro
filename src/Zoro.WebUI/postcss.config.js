const tailwindcss = require('tailwindcss');
module.exports = {
    sourceMaps: true,
    plugins: [
        tailwindcss('./tailwind.config.js'),
        require('postcss-import'),
        require('postcss-preset-env'),
        require('autoprefixer')
    ]
};
