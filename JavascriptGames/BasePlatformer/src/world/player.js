import { Point } from "../utils/point.js";
import { Collider } from "./collider.js";

export class Player {
    constructor(position, radius) {
        this.position = position;
        this.radius = radius;
        this.diameter = radius * 2
        this.radians = 0
        this.velocity = new Point(0, 0);

        this.img = new Image();
        this.img.src = "./img/Ball.png";

        this.collider = new Collider(position.x, position.y, radius * 2, radius * 2);
    }
    update() {
        this.position.x += this.velocity.x;
        this.position.y += this.velocity.y;
        
        this.radians += this.velocity.x/10;

        this.collider.update(this.x, this.y);
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