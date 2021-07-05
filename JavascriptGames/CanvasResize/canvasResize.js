const canvas = document.querySelector("canvas");
const ctx = canvas.getContext("2d");
const width = 1200;
const height = 600;
const widthHeightRatio = width/height;
var scale = 1;


function drawCenter() {
    ctx.strokeRect(550, 250, 100, 100);
    ctx.strokeRect(600, 300, 1, 1);
}

function drawCircle(x, y) {
    ctx.beginPath();
    ctx.arc(x, y, 3, 0, Math.PI * 2);
    ctx.closePath();
    ctx.fill();
}


function resize() {
    if (window.innerWidth / window.innerHeight > widthHeightRatio) {
        canvas.width = (window.innerHeight - 30) * widthHeightRatio;
        canvas.height = window.innerHeight - 30;
        scale = (window.innerHeight - 30)/height; 
        ctx.scale(scale, scale);
    }   
    else {
        canvas.width = window.innerWidth - 30;
        canvas.height = (window.innerWidth - 30 )/ widthHeightRatio;
        scale = (window.innerWidth - 30)/width;
        ctx.scale(scale, scale);
    }
    drawCenter();
    
}
resize();



addEventListener("resize", resize);
addEventListener("click", (event) => {
    let marginX = (window.innerWidth - canvas.width)/2;
    let marginY = (window.innerHeight - canvas.height)/2;
    let x = (event.clientX - marginX) / scale;
    let y = (event.clientY - marginY) / scale;
    drawCircle(x, y);
});