import { Friction, Gravity, Movement } from "../utils/movement.js";
import { Point } from "../utils/point.js";
import { Player } from "./player.js";

export class World {
    static playerInitialVelocity = new Point(0, 0);
    constructor(width, height, playerSize, playerImg) {
        this.width = width;
        this.height = height;
        this.centerX = width/2;
        this.centerY = height/2; 

        this.friction = new Friction();
        this.gravity = new Gravity();

        const initialPlayerPosition = new Point(this.centerX, this.centerY);
        this.player = new Player(initialPlayerPosition, playerSize, World.playerInitialVelocity, playerImg);
    }
    update() {
        this.gravity.apply(this.player.velocity);
        this.friction.apply(this.player.velocity);

        this.player.update();

        this.resolveCollisions();
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

        ctx.moveTo(this.centerX, this.height);
        ctx.lineTo(this.centerX, 0);
        ctx.moveTo(this.width, this.centerY);
        ctx.lineTo(0, this.centerY);

        ctx.stroke();
        ctx.restore();
    }
    //#endregion

    resolveCollisions() {
        if (this.player.position.y + this.player.radius >= this.height) {
            this.player.position.y = this.height - this.player.radius;
            this.player.velocity.y = Movement.stop;
        }
        if (this.player.position.x + this.player.radius >= this.width) {
            this.player.position.x = this.width - this.player.radius;
            this.player.velocity.x = Movement.stop;
        }
    }
}


