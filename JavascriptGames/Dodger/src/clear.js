import { getRandomNum } from "./getRandom.js";

export class Clear {
    constructor(game) {
        this.game = game;
        this.lives = this.game.lives;

        this.size = 30;
        this.position = {
            x: getRandomNum(0, this.game.gameSize - 100),
            y: -this.size
        };
        this.img = document.getElementById("star_Img");        
    }
    update(ctx) {
        this.position.y += 4;
        this.position.x += 4 * Math.sin(this.position.y * Math.PI/180);
        ctx.drawImage(this.img, this.position.x, this.position.y, this.size, this.size);
        this.detectCollisionWithShip();
    }
    detectCollisionWithShip() {
        if (this.game.ship.positions.top <= this.position.y + this.size && 
            // Left side of ship to the right of the screen
            this.game.ship.positions.leftSide <= this.position.x + this.size &&
            // Right of the ship to the left of the screen
            this.game.ship.positions.rightSide >= this.position.x &&
            this.game.ship.positions.bottom >= this.position.y
            ) {
                this.game.clears++;
                this.markedForDeletion = true;
        }

    }
}