import { Direction } from "./direction.js";

export class Ship {
    constructor(game) {
        
        this.game = game;
        this.speed = {
            x: 0,
            y: 0
        };
        this.maxSpeed = {
            x: 10,
            y: 10
        };
        this.img = document.getElementById("ship_Img");
        this.size = 50;
        this.position = {
            x: this.game.gameSize/2 - this.size/2,
            y: this.game.gameSize - this.size - 30
        };
        this.positions = {
            top: this.position.y,
            bottom: this.position.y + this.size,
            rightSide: this.position.x + this.size,
            leftSide: this.position.x
        }
        this.constraints = {
            x: [0, this.game.gameSize],
            y: [this.game.gameSize, 0]
        };
    }
    update(ctx) {
        
        this.position.x += this.speed.x;
        this.position.y += this.speed.y
        this.positions = {
            top: this.position.y,
            bottom: this.position.y + this.size,
            rightSide: this.position.x + this.size,
            leftSide: this.position.x
        }
        this.checkConstrants()
        this.draw(ctx)
        
    }
    draw(ctx) {
        ctx.drawImage(this.img, this.position.x, this.position.y, this.size, this.size);
    }
    move(direction) {
        if (direction === Direction.LEFT) {
            this.speed.x = -this.maxSpeed.x;
        }
        else if (direction === Direction.RIGHT) {
            this.speed.x = this.maxSpeed.x;
        }
        else if (direction === Direction.UP) {
            this.speed.y = -this.maxSpeed.y;
        }
        else if (direction === Direction.DOWN) {
            this.speed.y = this.maxSpeed.y;
        }
    }
    checkConstrants() {
        if (this.position.x <= this.constraints.x[0]) {
            this.position.x = this.constraints.x[0];
        }
        else if (this.position.x + this.size >= this.constraints.x[1]) {
            this.position.x = this.constraints.x[1] - this.size;
        }
        
        if (this.position.y + this.size >= this.constraints.y[0]) {
            this.position.y = this.constraints.y[0] - this.size;
        }
        else if (this.position.y <= this.constraints.y[1]) {
            this.position.y = this.constraints.y[1];
        }
    }
    stopx() {
        this.speed.x = 0;
    }
    stopy() {
        this.speed.y = 0;
    }
}