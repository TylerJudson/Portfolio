import { Collider } from "./collider.js";

export class Player {
    constructor(x, y, r, color) {
        this.x = x;
        this.y = y;
        this.r = r;
        this.color = color;

        this.collider = new Collider(x, y, r * 2, r * 2);

        this.velocity = {
            x: 0,
            y: 0
        }
        this.speed = 0.5;
        this.jumping = false;
    }
    //#region Getters and Setters
    /**
     * @param {number} top
     */
    set top(top) {
        this.y = top + this.r;
    }
    /**
     * @param {number} bottom
     */
    set bottom(bottom) {
        this.y = bottom - this.r;
    }
    /**
     * @param {any} left
     */
    set left(left) {
        this.x = left + this.r;
    }
    /**
     * @param {number} right
     */
    set right(right) {
        this.x = right - this.r;
    }
    //#endregion
    update() {
        this.x += this.velocity.x;
        this.y += this.velocity.y;

        this.collider.update(this.x, this.y);
    }
    render(ctx) {
        ctx.save();

        ctx.translate(this.x, this.y);

        ctx.beginPath();
        ctx.arc(0, 0, this.r, 0, Math.PI * 2);

        ctx.fillStyle = this.color;
        ctx.fill();
        
        ctx.restore();
    }
    jump() {
        if (!this.jumping) {
            this.jumping = true;
            this.velocity.y = -20;
        }
    }
    moveRight() {
        this.velocity.x += this.speed;
    }
    moveLeft() {
        this.velocity.x -= this.speed;
    }
}