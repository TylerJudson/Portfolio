import { Background } from "./background/background.js";

export class World {
    constructor() {
        this.width = 1900;
        this.height = 1000;
        this.center = {
            x: this.width/2,
            y: this.height/2
        }
        this.background = new Background(this.width, this.height);

    }
    update() {

    }
    render(ctx) {

    }
    renderBackground(ctx) {
        this.background.render(ctx);
    }
}