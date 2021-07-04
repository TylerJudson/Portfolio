import { Friction, Gravity, Movement } from "../utils/movement.js";
import { Point } from "../utils/point.js";
import { Player } from "./player.js";

export class World {
    constructor() {
        this.friction = new Friction();
        this.gravity = new Gravity();

        this.width = 700;
        this.height = 400;

        this.player = new Player(new Point(this.width/2, this.height/2), 10, "red");
    }
    update() {
        this.gravity.apply(this.player.velocity);
        this.friction.apply(this.player.velocity);

        this.player.update();

        this.checkCollisions();
    }
    //#region Rendering
    render(ctx) {
        this.player.render(ctx);
    }
    showReferenceLines(ctx) {
        ctx.save();
        ctx.beginPath();

        ctx.moveTo(this.width, this.height);
        ctx.lineTo(0, 0)
        ctx.moveTo(this.width, 0);
        ctx.lineTo(0, this.height);

        ctx.moveTo(this.width/2, this.height);
        ctx.lineTo(this.width/2, 0);
        ctx.moveTo(this.width, this.height/2);
        ctx.lineTo(0, this.height/2);

        ctx.stroke();
        ctx.restore();
    }
    //#endregion

    checkCollisions() {
        if (this.player.position.y + this.player.radius >= this.height) {
            this.player.position.y = this.height - this.player.radius;
            this.player.velocity.y = 0;
        }
        if (this.player.position.x + this.player.radius >= this.width) {
            this.player.position.x = this.width - this.player.radius;
            this.player.velocity.x = 0;
        }
    }
}


