import { map, collisionMap } from "./map.js";

export class World {
    constructor() {
        this.friction = 0.9;
        this.gravity = 3;

        this.columns = 20;
        this.rows = 15;
        this.tileSize = 20;

        this.map = map;
        this.collisionMap = collisionMap;


        this.width = this.columns * this.tileSize;
        this.height = this.rows * this.tileSize;
    }
    update() {

    }
    render(ctx) {
        this.renderMap(ctx);
    }
    renderMap(ctx) {
        ctx.save();
        for (let i = this.map.length - 1; i > -1; i--) {
            let value = this.map[i];
            let x = (i % this.columns) * this.tileSize;
            let y = Math.floor(i / this.columns) * this.tileSize;
            switch (value) {
                case 1:
                    ctx.fillStyle = "blue";
                    break;
                default:
                    ctx.fillStyle = "gray";
                    break;
            }
            ctx.fillRect(x, y, this.tileSize, this.tileSize);
        }
        ctx.restore();
    }
}