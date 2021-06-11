let width = 1900;
let height = 985;

let canvas = document.querySelector('canvas');
let ctx = canvas.getContext("2d");

let centerX = width/2;
let centerY = height/2;

canvas.width = width;
canvas.height = height;

function drawBackground() {
    drawSky();
    drawStars();
    drawMountains();
}
function drawStars() {
    for (let i = 0; i <= getRandomInt(200, 500); i++) {
        drawStar(getRandomInt(0, width), getRandomInt(0, height), getRandomInt(2, 15));
    }
}
function drawStar(x, y, r) {
    ctx.save();

    let gradient = ctx.createRadialGradient(x, y, r/5, x, y, r);
    gradient.addColorStop(0, "white");
    gradient.addColorStop(.1, "rgba(255, 255, 255, .3)")
    gradient.addColorStop(1, "rgba(255, 255, 255, 0)")

    ctx.beginPath();
    ctx.arc(x, y, r, 0, Math.PI*2);
    ctx.fillStyle = gradient;
    ctx.fill();

    ctx.restore();
}
function drawSky() {
    ctx.save();

    let gradient = ctx.createRadialGradient(centerX, height, 300, centerX, centerY, width);
    gradient.addColorStop(0, "rgb(0, 0, 0)");
    gradient.addColorStop(1, "rgb(0, 0, 50)");


    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, width, height);
    ctx.restore();
}
function drawMountains() {
    ctx.save();

    let colorValue = getRandomInt(30, 40);
    let gradient = ctx.createLinearGradient(width/2, 0, width/2, height);
    gradient.addColorStop(0, `rgb(${colorValue}, ${colorValue}, ${colorValue})`);
    gradient.addColorStop(1, `rgb(${colorValue - 10}, ${colorValue - 10}, ${colorValue-10})`);

    for (let i = 0; i <= getRandomInt(1, 3); i++) {
        let x1 = getRandomInt(-400, width/3 * 2);
        let x2 = getRandomInt(x1 + 1000, x1 + 2000);
        let h = getRandomInt(50, 300);
        drawPointedMountain(x1, x2, h, gradient);
    }

    colorValue = getRandomInt(20, 30);
    gradient = ctx.createLinearGradient(width/2, 0, width/2, height);
    gradient.addColorStop(0, `rgb(${colorValue}, ${colorValue}, ${colorValue})`);
    gradient.addColorStop(1, `rgb(${colorValue - 10}, ${colorValue - 10}, ${colorValue - 10})`);
    for (let i = 0; i <= getRandomInt(3, 10); i++) {
        let x1 = getRandomInt(-300, width/4 * 3);
        let x2 = getRandomInt(x1 + 500, x1 + 1500);
        let h = getRandomInt(400, 600);
        drawPointedMountain(x1, x2, h, gradient);
    }

    colorValue = getRandomInt(10, 20);
    gradient = ctx.createLinearGradient(width/2, 0, width/2, height);
    gradient.addColorStop(0, `rgb(${colorValue}, ${colorValue}, ${colorValue})`);
    gradient.addColorStop(1, `rgb(${colorValue - 10}, ${colorValue - 10}, ${colorValue - 10})`);
    for (let i = 0; i <= getRandomInt(5, 15); i++) {
        let x1 = getRandomInt(-200, width - 100);
        let x2 = getRandomInt(x1 + 300, x1 + 1000)
        let h = getRandomInt(600, 800);
        drawPointedMountain(x1, x2, h, gradient);
    }


    ctx.restore();
}
function drawPointedMountain(x1, x2, h, color) {
    ctx.save();
    ctx.beginPath();
    ctx.moveTo(x1, height);
    ctx.lineTo((x1 + x2)/2, h);
    ctx.lineTo(x2, height);
    ctx.fillStyle = color;
    ctx.fill()

    ctx.restore();
}
function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min) + min);
}

function animate() {
    requestAnimationFrame(animate);
}
drawBackground();
animate();  