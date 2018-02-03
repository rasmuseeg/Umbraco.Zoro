/// <binding ProjectOpened='watch' />
'use strict';
require('es6-promise').polyfill();

var gulp = require('gulp');
var sass = require('gulp-sass');
var sourcemaps = require("gulp-sourcemaps");
var autoprefixer = require('gulp-autoprefixer');

var autoprefixerOptions = {
    browsers: ['last 2 versions', '> 5%', 'Firefox ESR']
};

gulp.task('sass', function () {
    return gulp.src('assets/scss/bootstrap.scss')
        .pipe(sourcemaps.init())
        .pipe(sass().on('error', sass.logError))
        .pipe(sourcemaps.write())
        .pipe(autoprefixer(autoprefixerOptions))
        .pipe(gulp.dest('assets/css'));
});

gulp.task("watch", function () {
    return gulp.watch("assets/scss/**/*.scss", ["sass"]);
});

gulp.task("default", ["sass", "watch"]);