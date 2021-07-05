import { CollidableObject } from "./collidableObject.js";

export class Player extends CollidableObject {
    constructor(x, y, width, height, color) {
        super(x, y, width > height ? width/2 : height/2);
        this.width = width * 3;
        this.height = height;
        this.x = x;
        this.y = y;
        this.velocity = {
            x: 0,
            y: 0
        };
        this.radians = 0;
        this.shipImg = new Image()

        this.score = 0;

        if (color === "blue") {
            this.shipImg.src = "./img/BlueRocket.png";
        }
        else {
            this.shipImg.src = "./img/RedRocket.png";
        }
    }   
    update() {
        this.x += this.velocity.x;
        this.y += this.velocity.y;
        this.update1(this.x, this.y, this.computeVertices());
    }
    computeVertices() {
        return [
            {x: this.x - Math.cos(Math.PI/3 + this.radians) * this.width/3, y: this.y - Math.sin(Math.PI/3 + this.radians) * this.height/3}, 
            {x: this.x - Math.cos(Math.PI/2 + this.radians) * this.width/2, y: this.y - Math.sin(Math.PI/2 + this.radians) * this.height/2},
            {x: this.x - Math.cos(2 * Math.PI/3 + this.radians) * this.width/3, y: this.y - Math.sin(2 * Math.PI/3 + this.radians) * this.height/3}, 
            {x: this.x - Math.cos(17 * Math.PI/12 + this.radians) * this.width/2, y: this.y - Math.sin(17 * Math.PI/12 + this.radians) * this.height/2},
            {x: this.x - Math.cos(19 * Math.PI/12 + this.radians) * this.width/2, y: this.y - Math.sin(19 * Math.PI/12 + this.radians) * this.height/2}
        ];
    }
    render(ctx) {
        ctx.save();
        
        ctx.translate(this.x, this.y);
        ctx.rotate(this.radians);
        ctx.drawImage(this.shipImg, -this.width/2, -this.height/2, this.width, this.height);

        ctx.restore();
    }
    bounce() {
        this.velocity.x = -this.velocity.x;
        this.velocity.y = -this.velocity.y;
    }
    rotateLeft() {
        this.radians -= .1;
    }
    rotateRight() {
        this.radians += .1;
    }
    accelerate() {
        this.velocity.y -= Math.cos(this.radians);
        this.velocity.x += Math.sin(this.radians);
    }
    deaccelerate() {
        this.velocity.y += Math.cos(this.radians);
        this.velocity.x -= Math.sin(this.radians);
    }
}