import { getRandomNum } from "./getRandom.js";


export class Enemy {
    constructor(game, speed) {
        this.game = game;
        this.width = getRandomNum(5, 25);
        this.height = getRandomNum(40, 70);
        this.position = {
            x: getRandomNum(0 + this.width, game.gameSize - this.width),
            y: -this.height
        };
        this.speed = 0;
        this.maxSpeed = speed;
        this.markedForDeletion = false;
    }
    update(ctx) {
        this.draw(ctx);
        this.position.y += this.maxSpeed;
        this.detectCollisionWithShip();
        this.detectCollisionWithProjectile();
        if (this.position.y >= this.game.gameSize) {
            this.markedForDeletion = true;
        }
    }
    draw(ctx) {
        ctx.fillStyle = "white";
        ctx.fillRect(this.position.x, this.position.y, this.width, this.height);

    }
    detectCollisionWithShip() {
        if (this.game.ship.positions.top <= this.position.y + this.height && 
            // Left side of ship to the right of the screen
            this.game.ship.positions.leftSide <= this.position.x + this.width &&
            // Right of the ship to the left of the screen
            this.game.ship.positions.rightSide >= this.position.x &&
            this.game.ship.positions.bottom >= this.position.y
            ) {
                this.game.lives--;
                this.markedForDeletion = true;
        }

    }
    detectCollisionWithProjectile() {
        if (this.game.projectile != null) {
            if (this.game.projectile.positions.top <= this.position.y + this.height && 
                // Left side of ship to the right of the screen
                this.game.projectile.positions.leftSide <= this.position.x + this.width &&
                // Right of the ship to the left of the screen
                this.game.projectile.positions.rightSide >= this.position.x &&
                this.game.projectile.positions.bottom >= this.position.y
                ) {
                    this.markedForDeletion = true;
                    this.game.projectile.markedForDeletion = true;
                    this.game.projectile = null;
            }
        }
       

    }

}