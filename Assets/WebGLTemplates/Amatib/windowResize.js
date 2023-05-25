var container;
var canvas;
var timer = 0;

//初期化
function init() {
    canvas = document.getElementById('unity-canvas');

    container = document.createElement('div');
    container.style.width = window.innerWidth + 'px';
    container.style.height = window.innerHeight + 'px';
    canvas.style.width = container.style.width;
    canvas.style.height = container.style.height;
    container.style.overflow = 'hidden';
    container.appendChild(canvas);
    document.body.appendChild(container);
    document.body.style.margin = '0px'
}

//サイズ変更処理
function resize() {
    container.style.width = window.innerWidth + 'px';
    container.style.height = window.innerHeight + 'px';
    canvas.width = window.innerWidth * window.devicePixelRatio;
    canvas.height = window.innerHeight * window.devicePixelRatio;
    canvas.style.width = container.style.width;
    canvas.style.height = container.style.height;
}

window.onload = function () {
    init();
    resize();
};

//ブラウザの大きさが変わった時に行う処理
(function () {
    var timer = 0;
    window.onresize = function () {
        if (timer > 0) {
            clearTimeout(timer);
        }
        timer = setTimeout(function () {
            resize();
        }, 200);
    };
}());