/*
This file is the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. https://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var print = require('gulp-print').default;

gulp.task('default', ['fonts']);


// Fonts
gulp.task('fonts', function () {
    return gulp.src([
        'node_modules/@fortawesome/fontawesome-free/webfonts/**.*'])
        .pipe(print())
        .pipe(gulp.dest('fonts'));
});
