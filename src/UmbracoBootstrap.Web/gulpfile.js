/// <binding ProjectOpened='watch-scss' />
'use strict';
require('es6-promise').polyfill();

var gulp = require('gulp');
var sass = require('gulp-sass');
var sourcemaps = require("gulp-sourcemaps");
var autoprefixer = require('gulp-autoprefixer');
var svgstore = require('gulp-svgstore');
var svgmin = require('gulp-svgmin');
var path = require('path');

var autoprefixerOptions = {
    browsers: ['last 2 versions', '> 5%', 'Firefox ESR']
};

gulp.task('svgstore', function () {
    return gulp
        .src(['assets/icons/*.svg', '!assets/icons/icons.svg'])
        .pipe(svgmin(function (file) {
            var prefix = path.basename(file.relative, path.extname(file.relative));
            return {
                plugins: [{
                    cleanupIDs: {
                        prefix: prefix + '-',
                        minify: false
                    }
                }]
            }
        }))
        .pipe(svgstore())
        .pipe(gulp.dest('assets/icons'));
});

gulp.task('scss', function () {
    return gulp.src(['assets/scss/**/*.scss'])
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        .pipe(sourcemaps.write('.', {
            includeContent: false,
            sourceRoot: '/assets/scss'
        }))
        //.pipe(autoprefixer(autoprefixerOptions))
        .pipe(gulp.dest('assets/css'));
});

gulp.task("watch-scss", function () {
    return gulp.watch("assets/scss/**/*.scss", ["scss"]);
});

gulp.task("default", ["scss", "watch-scss"]);