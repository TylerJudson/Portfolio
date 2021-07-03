

const canvas = document.querySelector("canvas");
const ctx = canvas.getContext("2d");

canvas.width = innerWidth;
canvas.height = innerHeight;

let stars, particleStars, backgroundStars, w, h, cx, cy, backgroundGrad, ticker, groundHeight




//#region Objects
class Star {
    constructor(x, y, radius, color) {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.color = color;
        this.velocity = {
            x: (Math.random() - 0.5) * 15,
            y: 3
        }
        this.friction = 0.8;
        this.gravity = 1;
    }
    shatter() {
        this.radius -= 3;
        for (let i = 0; i < getRandomInt(5, this.radius + 5); i ++) {
            particleStars.push(new StarParticle(this.x, this.y, (Math.random() * (3 - 0.5) + 0.5)));
        }
    }
    draw() {
        ctx.save();
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2, false);
        ctx.fillStyle = this.color;
        ctx.shadowColor = "#E3EAEF";
        ctx.shadowBlur = 20;
        ctx.fill();
        ctx.closePath();
        ctx.restore();
    }
    update() {
        this.draw();

        // When ball hits the ground
        if (this.y + this.radius + this.velocity.y > h - groundHeight) {
            this.velocity.y = -this.velocity.y * this.friction;
            this.shatter();
        }
        else {
            this.velocity.y += this.gravity;
        }

        // When ball hits the side of the screen
        if (this.x + this.radius + this.velocity.x > w || this.x - this.radius < 0){
            this.velocity.x = -this.velocity.x * this.friction;
            this.shatter();
        }
        this.x += this.velocity.x;
        this.y += this.velocity.y;
    }
}

class StarParticle {
    constructor(x, y, radius) {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.velocity = {
            x: getRandomInt(-10, 10),
            y: getRandomInt(-30, 30)
        }
        this.friction = 0.8;
        this.gravity = 2;
        this.ttl = 75; // time to live
        this.opacity = 1;
    }
    draw() {
        ctx.save();
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2, false);
        ctx.fillStyle = `rgba(227, 234, 239, ${this.opacity})`;
        ctx.shadowColor = "#E3EAEF";
        ctx.shadowBlur = 20;
        ctx.fill();
        ctx.closePath();
        ctx.restore();
    }
    update() {
        this.draw();

        // When ball hits the bottom of the screen
        if (this.y + this.radius + this.velocity.y > h - groundHeight) {
            this.velocity.y = -this.velocity.y * this.friction;
        }
        else {
            this.velocity.y += this.gravity;
        }
        this.y += this.velocity.y;
        this.x += this.velocity.x;
        this.ttl -= 1;
        this.opacity -= 1 / this.ttl;
    }
}

//#endregion


//#region Functions
function init() {
    w = canvas.width;
    h = canvas.height;
    cx = w/2;
    cy = h/2;
    ticker = 0;
    backgroundGrad = ctx.createLinearGradient(0, 0, 0, h);
    backgroundGrad.addColorStop(0, "#171e26");
    backgroundGrad.addColorStop(1, "#3f586b");
    groundHeight = 100;

    stars = [];
    particleStars = [];
    backgroundStars = [];

    for (let i = 0; i < 150; i++) {
        const x = getRandomInt(0, w);
        const y = getRandomInt(0, h);
        const r = Math.random() * 3;
        backgroundStars.push(new Star(x, y, r, "white"))
    }
}
function animate() {
    requestAnimationFrame(animate);
    clear();
    drawBackgroundStars();
    drawMountains();
    drawFloor();
    updateStars();
    updateParticleStars();
}
function clear() {
    ctx.save();
    ctx.fillStyle = backgroundGrad;
    ctx.fillRect(0, 0, w, h);
    ctx.restore();
}
function drawBackgroundStars() {
    backgroundStars.forEach(star => {
        star.draw();
    });
}
function drawMountains() {
    drawMountainRange(1, h - 50, "#384551");
    drawMountainRange(2, h - 100, "#2B3843");
    drawMountainRange(3, h - 300, "#26333E");
}
function drawMountainRange(amount, height, color) {
    for (let i = 0; i < amount; i++) {
        const mountainWidth = w / amount;

        ctx.beginPath();
        ctx.moveTo(i * mountainWidth, h);
        ctx.lineTo(i * mountainWidth + mountainWidth + 325, h);
        ctx.lineTo(i * mountainWidth + mountainWidth/2, h - height);
        ctx.lineTo(i * mountainWidth - 325, h);
        ctx.fillStyle = color;
        ctx.fill();
        ctx.closePath();
    }
}
function drawFloor() {
    ctx.fillStyle = "#182028";
    ctx.fillRect(0, h - groundHeight, w, groundHeight)
}
function updateStars() {
    stars.forEach(star => {
        star.update();
        if (star.radius <= 0) {
            remove(star, stars);
        }
    });

    ticker++

    if (ticker % getRandomInt(20, 100) == 0) {
        const r = (Math.random() * (12 - 6) + 6);
        const x = Math.max(r, Math.random() * w - r);
        stars.push(new Star(x, -100, r, "white"))
    }
}
function updateParticleStars() {
    particleStars.forEach(particle => {
        particle.update();
        if (particle.ttl <= 0) {
            remove(particle, particleStars)
        }
        
    });
}


function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min);
}

function remove(element, array) {
    array.splice(array.indexOf(element), 1)
}
//#endregion




init();
animate();























addEventListener("resize", () => {
    canvas.width = innerWidth;
    canvas.height = innerHeight;

    init();
})