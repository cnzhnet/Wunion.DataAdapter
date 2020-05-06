/// <binding AfterBuild='default' Clean='clean' />

const gulp = require('gulp');
const del = require('del');

var paths = {
    scripts: [
        'Scripts/**/*.js',
        'Scripts/**/*.ts',
        'Scripts/**/*.map'
        //'!Scripts/**/*.d.ts'
    ],
    lib: []
};

// 执行清理任务.
gulp.task('clean', function () {
    return del(['wwwroot/js/**/*', 'Scripts/**/*.js', 'Scripts/**/*.map']);
});

//gulp.task('lib', function (done) {
//    gulp.src(paths.lib).pipe(gulp.dest("wwwroot/lib/crypto-js"));
//    done(); 
//});

gulp.task('default', function (done) {
    gulp.src(paths.scripts).pipe(gulp.dest('wwwroot/js'));
    //del("Scripts/**/*.js");
    //del("Scripts/**/*.map");
    done();
});