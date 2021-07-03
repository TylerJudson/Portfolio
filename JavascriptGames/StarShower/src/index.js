

const canvas = document.querySelector("canvas");
const ctx = canvas.getContext("2d");

canvas.width = innerWidth;
canvas.height = innerHeight;

let stars, particleStars, w, h, cx, cy




//#region Objects
class Star {
    constructor(x, y, radius, color) {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.color = color;
        this.velocity = {
            x: 0,
            y: 3
        }
        this.friction = 0.8;
        this.gravity = 1;
    }
    shatter() {
        this.radius -= 3;
        for (let i = 0; i < 8; i ++) {
            particleStars.push(new StarParticle(this.x, this.y, 2))
        }
    }
    draw() {
        ctx.save();
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2, false);
        ctx.fillStyle = this.color;
        ctx.fill();
        ctx.closePath();
    }
    update() {
        this.draw();

        // When ball hits the bottom of the screen
        if (this.y + this.radius + this.velocity.y > h) {
            this.velocity.y = -this.velocity.y * this.friction;
            this.shatter();
        }
        else {
            this.velocity.y += this.gravity;
        }
        this.y += this.velocity.y;
    }
}

class StarParticle {
    constructor(x, y, radius, color) {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.color = color;
        this.velocity = {
            x: getRandomInt(-5, 5),
            y: getRandomInt(-15, 15)
        }
        this.friction = 0.8;
        this.gravity = 0.25;
        this.ttl = 100; // time to live
        this.opacity = 1;
    }
    draw() {
        ctx.save();
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2, false);
        ctx.fillStyle = `rgba(255, 0, 0, ${this.opacity})`;
        ctx.fill();
        ctx.closePath();
    }
    update() {
        this.draw();

        // When ball hits the bottom of the screen
        if (this.y + this.radius + this.velocity.y > h) {
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

    stars = [];
    particleStars = [];

    for (let i = 0; i < 1; i++) {
        stars.push(new Star(cx, 30, 30, "blue"));
    }
}
function animate() {
    requestAnimationFrame(animate);
    clear();
    updateStars();
    updateParticleStars();
}
function clear() {
    ctx.clearRect(0, 0, w, h);
}
function updateStars() {
    stars.forEach(star => {
        star.update();
        if (star.radius <= 0) {
            remove(star, stars);
        }
    });
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
function distance(x1, y1, x2, y2) {
    const deltaX = x2 - x1;
    const deltaY = y2 - y1;

    return Math.sqrt(Math.pow(deltaX, 2) + Math.pow(deltaY, 2));
}
function remove(element, array) {
    array.splice(array.indexOf(element), 1)
}
init();
animate();























addEventListener("resize", () => {
    canvas.width = innerWidth;
    canvas.height = innerHeight;

    init();
})