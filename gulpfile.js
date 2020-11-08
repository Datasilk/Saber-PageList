var gulp = require('gulp'),
    less = require('gulp-less');

var release = 'bin/Release/netcoreapp3.1/';
var publish = 'bin/Publish/PageList/'

gulp.task('publish', function () {
    var p = gulp.src('pagelist.less')
        .pipe(less())
        .pipe(gulp.dest(publish, { overwrite: true }));
    p = gulp.src(['container.html', 'page.html', release + 'Saber.Vendor.PageList.dll'])
        .pipe(gulp.dest(publish, { overwrite: true }));
    return p;
});