module.exports = {
    sourceMaps: true,
    plugins: [
        require('tailwindcss')('./tailwind.config.js'),
        require('postcss-import')(),
        require('autoprefixer')
    ],
}
