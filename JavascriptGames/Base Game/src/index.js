const canvas = document.querySelector("canvas");
const scoreEL = document.querySelector("#scoreEL");
const startGameBtn = document.querySelector("#startGameBtn");
const bigScoreEL = document.querySelector("#bigScoreEL")
const UI = document.querySelector("#UI");
const ctx = canvas.getContext("2d");
canvas.width = window.innerWidth - 4;
canvas.height = window.innerHeight - 4;
var centerX;
var centerY;
const WIDTH = canvas.width;
const HEIGHT = canvas.height;
const mouse = {
    down: false,
    x: undefined,
    y: undefined
}
const powerUpImg = new Image();
var backgroundParticles = [];
powerUpImg.src = "img/lightningBolt.png";
var frame = 0;
var friction;
var projectiles;
var enemies;
var particles;
var player;
var score;
var animationId;
var powerUps;




//#region Classes
// Create player class
class Player {
    constructor(x, y, radius, color) {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.color = color;
        this.velocity = {
            x: 0,
            y: 0
        }
        this.powerUp = ""
    }
    draw() {
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fillStyle = this.color;
        ctx.fill();
    }
    update() {
        this.draw();

        this.velocity.x *= friction;
        this.velocity.y *= friction;

        this.y += this.velocity.y;

        if (player.x - this.radius + this.velocity.x > 0 && player.x + this.radius + this.velocity.x < canvas.width ) {
            this.x += this.velocity.x;
        }
        else {
            this.velocity.x = 0;
        }
        if (player.y - this.radius + this.velocity.y > 0 && player.y + this.radius + this.velocity.y < canvas.height ) {
            this.y += this.velocity.y;
        }
        else {
            this.velocity.y = 0;
        }

    }
    shoot(mouse) {
        const angle = Math.atan2(mouse.y - this.y, mouse.x - this.x)
        const velocity = {
            x: Math.cos(angle) * 5,
            y: Math.sin(angle) * 5
        }
        projectiles.push(new Projectile(this.x, this.y, 5, this.color, velocity))
    }
}
// Create Projectile class
class Projectile {
    constructor(x, y, radius, color, velocity) {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.color = color;
        this.velocity = velocity;
    }
    draw() {
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fillStyle = this.color;
        ctx.fill();
    }
    update() {
        this.draw();
        this.x += this.velocity.x;
        this.y += this.velocity.y;
    }
}
// Create Enemy class
class Enemy {
    constructor(x, y, radius, color, velocity) {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.color = color;
        this.velocity = velocity;
        this.type = "linear"
        this.center = {
            x: x,
            y: y
        };
        this.radians = 0
        
        if (Math.random() < 0.25) {
            this.type = "homing";
            if (Math.random() < 0.5) {
                this.type = "spinning";
                if (Math.random() < 0.25) {
                    this.type = "homingSpinning"
                }
            }
        }
    }
    draw() {
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fillStyle = this.color;
        ctx.fill();
    }
    update() {
        this.draw();

        if (this.type === "homing")
        {
            const angle = Math.atan2(player.y - this.y, player.x - this.x);
            this.velocity = {
                x: Math.cos(angle),
                y: Math.sin(angle)
            };
            this.x += this.velocity.x;
            this.y += this.velocity.y;
        }
        else if (this.type === "spinning") {
            this.radians += 0.05;
            this.center.x = this.center.x + this.velocity.x;
            this.center.y = this.center.y + this.velocity.y;

            this.x = this.center.x + Math.cos(this.radians) * 100;
            this.y = this.center.y + Math.sin(this.radians) * 100;
        }
        else if (this.type === "homingSpinning") {
            // Home
            const angle = Math.atan2(player.y - this.y, player.x - this.x);
            this.velocity = {
                x: Math.cos(angle),
                y: Math.sin(angle)
            };

            // Spin
            this.radians += 0.05;
            this.center.x = this.center.x + this.velocity.x;
            this.center.y = this.center.y + this.velocity.y;

            this.x = this.center.x + Math.cos(this.radians) * 100;
            this.y = this.center.y + Math.sin(this.radians) * 100;
        }
        else if (this.type === "linear") {
            this.x += this.velocity.x;
            this.y += this.velocity.y;
        }
        

        
    }
}
// Create Particle Class
class Particle {
    constructor(x, y, radius, color, velocity) {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.color = color;
        this.velocity = velocity;
        this.alpha = 1;
    }
    draw() {
        ctx.save();
        ctx.globalAlpha = this.alpha;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fillStyle = this.color;
        ctx.fill();
        ctx.restore();
    }
    update() {
        this.draw();
        this.velocity.x *= friction;
        this.velocity.y *= friction;
        this.x += this.velocity.x;
        this.y += this.velocity.y;
        this.alpha -= 0.01;
    }
}
// Create Backgroud Particle Class
class BackgroundParticle {
    constructor(x, y, radius, color) {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.color = color;
        this.alpha = .05;
        this.initialAlpha = this.alpha;
    }
    draw() {
        ctx.save();
        ctx.globalAlpha = this.alpha;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fillStyle = this.color;
        ctx.fill();
        ctx.restore();
    }
    update() {
        this.draw();
    }
}
class PowerUp {
    constructor(x, y, image, velocity) {
        this.x = x;
        this.y = y;
        this.image = image;
        this.velocity = velocity;
        this.width = 50;
        this.height = 50;
        this.radians = 0;
    }
    draw() {
        ctx.save();
        ctx.translate(this.x + this.width/2, this.y + this.height/2);
        ctx.rotate(this.radians);
        ctx.translate(-this.x - this.width/2, -this.y - this.height/2);
        ctx.drawImage(this.image, this.x, this.y, this.width, this.height);
        ctx.restore();
    }
    update() {
        this.radians += 0.003;
        this.draw();
        this.x += this.velocity.x;
        this.y += this.velocity.y;
    }
}
//#endregion


function init() {
    const centerX = canvas.width / 2;
    const centerY = canvas.height / 2;
    friction = 0.99
    projectiles = [];
    enemies = [];
    particles = [];
    powerUps = [];
    player = new Player(centerX, centerY, 10, "white");
    frame = 0;
    
}


function spawnEnemies() {
    setInterval( function() {
        const radius = Math.random() * (30 - 4) + 4;
        let x;
        let y;
        if (Math.random() < 0.5)
        {
            x = Math.random() < 0.5 ? 0 - radius : canvas.width + radius;
            y = Math.random() * canvas.height;
        }
        else
        {
            x = Math.random() * canvas.width;
            y = Math.random() < 0.5 ? 0 - radius : canvas.height + radius;
            
        }
        const color = `hsl(${Math.random() * 360}, 50%, 50%)`;
        const angle = Math.atan2(canvas.height/2 - y, canvas.width/2 - x)
        const velocity = {
            x: Math.cos(angle),
            y: Math.sin(angle)
        }
        enemies.push(new Enemy(x, y, radius, color, velocity));
    }, 1000);
}
function spawnPowerUps() {
    setInterval(() => {
        let x;
        let y;
        if (Math.random() < 0.5)
        {
            x = Math.random() < 0.5 ? 0 - 10 : canvas.width + 10;
            y = Math.random() * canvas.height;
        }
        else
        {
            x = Math.random() * canvas.width;
            y = Math.random() < 0.5 ? 0 - 10 : canvas.height + 10;
            
        }
        const angle = Math.atan2(canvas.height/2 - y, canvas.width/2 - x)
        const velocity = {
            x: Math.cos(angle),
            y: Math.sin(angle)
        }
        powerUps.push(new PowerUp(x, y, powerUpImg, velocity));
    }, 5000);
    
}
function createScoreLabel(projectile, score) {

    const scoreLabel = document.createElement("label");

    scoreLabel.innerHTML = score;
    scoreLabel.style.position = "absolute";
    scoreLabel.style.color = "white";
    scoreLabel.style.userSelect = "none";
    scoreLabel.style.left = `${projectile.x}px`;
    scoreLabel.style.top = `${projectile.y}px`;
    
    document.body.appendChild(scoreLabel);

    gsap.to(scoreLabel, {
        opacity: 0,
        duration: 1,
        y: -30,
        onComplete: () => {
            scoreLabel.parentNode.removeChild(scoreLabel);
        }
    })
               
}
function createBackgroundParticles() {
    backgroundParticles = [];
    for (let x = 0; x < canvas.width; x+=30) {
        for (let y = 0; y < canvas.height; y+=30)
        {
            backgroundParticles.push(new BackgroundParticle(x, y, 3, "blue"));
        }
    }
}
function updateBackgroundParticles() {
    backgroundParticles.forEach((backgroundParticle) => {
        const dist = Math.hypot(player.x - backgroundParticle.x, player.y - backgroundParticle.y);
        const hideRadius = 100;
        if (dist < hideRadius) {
            if (dist < 70)
            {
                backgroundParticle.alpha = 0;
            }
            else {
                backgroundParticle.alpha = .25;
            }
        }
        else if (dist >= hideRadius && backgroundParticle.alpha < backgroundParticle.initialAlpha) {
            backgroundParticle.alpha += .01;
        }
        else if (dist >= hideRadius && backgroundParticle.alpha > backgroundParticle.initialAlpha) {
            backgroundParticle.alpha -= .01;
        }

        backgroundParticle.update();
    });
}

function animate() {
    frame++;
    animationId = requestAnimationFrame(animate);
    ctx.fillStyle = "rgba(0, 0, 0, 0.1)";
    ctx.fillRect(0,0, canvas.width, canvas.height);

    // Draw Player
    player.update();

    if (player.powerUp === "Automatic" && mouse.down) {
        if (frame % 4 === 0) {
            player.shoot(mouse);
        }
    }

    // Draw powerups
    powerUps.forEach((powerUp, index) => {
        const dist = Math.hypot(player.x - powerUp.x, player.y - powerUp.y);
        if (dist - player.radius - powerUp.width / 2 < 1)
        {
            player.powerUp = "Automatic";
            player.color = "yellow";
            powerUps.splice(index, 1);
            setTimeout(() => {
                player.powerUp = "";
                player.color = "white";
            }, 5000);
        }
        else {
            powerUp.update()
        }
    });
    // Draw Particles
    particles.forEach((particle, index) => {
        if (particle.alpha <= 0)
        {
            particles.splice(index)
        }
        else {
            particle.update();
        }
    });

    // Draw projectiles
    projectiles.forEach( function(projectile, index) {
        projectile.update();


        // remove from edges of the screen
        if (projectile.x + projectile.radius < 0 || projectile.x - projectile.radius > canvas.width ||
            projectile.y + projectile.radius < 0 || projectile.y - projectile.radius > canvas.height) {
            setTimeout(() => {
                projectiles.splice(index, 1);
            }, 0);
        }
    });

    enemies.forEach(function(enemy, index) {
        enemy.update();

        // If player touch an enemy
        const dist = Math.hypot(player.x - enemy.x, player.y - enemy.y);
        if (dist - enemy.radius - player.radius < 1) {
            cancelAnimationFrame(animationId);
            bigScoreEL.innerHTML = score;
            UI.style.display = "flex";
        }

        
        projectiles.forEach((projectile, projectileIndex) => {
            // If projectile touches an enemy
            const dist = Math.hypot(projectile.x - enemy.x, projectile.y - enemy.y);
            if (dist - enemy.radius - projectile.radius < 0) {
                // create expolsion Particles
                for (let i = 0; i < enemy.radius * 2; i ++) {
                    particles.push(new Particle(projectile.x,
                         projectile.y,
                          Math.random() * 2,
                           enemy.color,
                            {
                                x: (Math.random() - 0.5) * (Math.random() * 6),
                                y: (Math.random() - 0.5) * (Math.random() * 6)
                            }));
                }

                if (enemy.radius - 10 > 5) {
                    // Increae score
                    score += 100;
                    scoreEL.innerHTML = score;
                    createScoreLabel(projectile, 100);

                    gsap.to(enemy, {radius: enemy.radius-10});
                    setTimeout(() => {
                        projectiles.splice(projectileIndex, 1);
                    }, 0);
                }
                else {
                    score += 250;
                    scoreEL.innerHTML = score;

                    createScoreLabel(projectile, 250);

                    setTimeout(() => {
                        enemies.splice(index, 1);
                        projectiles.splice(projectileIndex, 1);
                    }, 0);
                }
            }
        });
    });



}


//#region Event Listeners
addEventListener("mousemove", (event) => {
    mouse.x = event.clientX;
    mouse.y = event.clientY;
});
addEventListener("mousedown", (event) => {
    mouse.x = event.clientX;
    mouse.y = event.clientY;
    mouse.down = true;
});
addEventListener("mouseup", () => {
    mouse.down = false;
});
addEventListener("click", function(event) {
    mouse.x = event.clientX;
    mouse.y = event.clientY;
    player.shoot(mouse);
});
startGameBtn.addEventListener("click", () => {
    score = 0;
    bigScoreEL.innerHTML = score;
    scoreEL.innerHTML = score;
    init();
    spawnEnemies();
    spawnPowerUps();
    animate();
    UI.style.display = "none";
});
addEventListener("keydown", (event) => {
    switch(event.keyCode) {
        case 87:
        case 38:
            player.velocity.y--;
            break;
        case 68:
        case 39:
            player.velocity.x++;
            break;
        case 83:
        case 40:
            player.velocity.y++;
            break;
        case 65:
        case 37:
            player.velocity.x--;
            break;
        default:
            break;
    }
});
addEventListener("resize", () => {
    canvas.width = window.innerWidth - 4;
    canvas.height = window.innerHeight - 4;
    init();
});

addEventListener("touchstart", (event) => {
    mouse.x = event.touches[0].clientX;
    mouse.y = event.touches[0].clientY;
        
    mouse.down = true;
});
addEventListener("touchmove", (event) => {
    mouse.x = event.touches[0].clientX;
    mouse.y = event.touches[0].clientY;
});
addEventListener("touchend", (event) => {
    mouse.down = false;
});

//#endregion