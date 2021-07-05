import { Collider } from "./collider.js";

export class Player {
    constructor(position, size, initialVelocity, img, spinSpeed = 0.1) {
        this.position = position;
        this.radius = size/2;
        this.diameter = size;
        this.radians = 0;
        this.spinSpeed = spinSpeed;
        this.velocity = initialVelocity;

        this.img = img;

        this.collider = new Collider(position, this.radius);
    }
    update() {
        this.position.x += this.velocity.x;
        this.position.y += this.velocity.y;
        
        this.radians += this.velocity.x * this.spinSpeed; 

        this.collider.update(this.position);
    }
    render(ctx) {
        ctx.save();

        ctx.translate(this.position.x, this.position.y);
        ctx.rotate(this.radians);

        ctx.drawImage(this.img, -this.radius, -this.radius, this.diameter, this.diameter);
        
        ctx.restore();
    }
    move(movement) {
        movement.apply(this.velocity);
    }
}