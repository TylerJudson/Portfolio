import { DarkGrayMountainRange, GraymountainRange, LightGrayMountainRange } from "./mountainRange.js";

export class Background {
    constructor(width, height, ctx) {
        this.width = width;
        this.height = height;
        this.center = {
            x: width/2,
            y: height/2
        }
        this.mountainRanges = [
            new DarkGrayMountainRange(this.width, this.height),
            new GraymountainRange(this.width, this.height),
            new LightGrayMountainRange(this.width, this.height)
        ]
    }
    render(ctx) {
        this.renderSky(ctx);

        this.mountainRanges.forEach((mountainRange) => {
            mountainRange.render(ctx);
        })
    }
    renderSky(ctx) {
        ctx.save();

        let gradient = ctx.createRadialGradient(this.center.x, this.height, 300, this.center.x, this.center.y, this.width);
        gradient.addColorStop(0, "rgb(0, 0, 0)");
        gradient.addColorStop(1, "rgb(0, 0, 50)");


        ctx.fillStyle = gradient;
        ctx.fillRect(0, 0, this.width, this.height);

        ctx.restore();
    }
}