import { BackgroundStar } from "./backgroundStar.js";
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
        this.backgroundStars = []
        this.createBackgroundStars();
    }
    render(ctx) {
        ctx.save();
        this.renderSky(ctx);

        this.backgroundStars.forEach((star) => {
            star.render(ctx);
        });

        this.mountainRanges.forEach((mountainRange) => {
            mountainRange.render(ctx);
        })

        ctx.restore();
    }
    renderSky(ctx) {
        ctx.save();

        let gradient = ctx.createRadialGradient(this.center.x, this.height, 300, this.center.x, this.center.y, this.width);
        gradient.addColorStop(0, "rgb(0, 0, 10)");
        gradient.addColorStop(1, "rgb(0, 0, 60)");


        ctx.fillStyle = gradient;
        ctx.fillRect(0, 0, this.width, this.height);

        ctx.restore();
    }
    createBackgroundStars() {
        for (let i = 0; i <= getRandomInt(200, 500); i++) {
            this.backgroundStars.push(new BackgroundStar(getRandomInt(0, this.width), getRandomInt(0, this.height), getRandomInt(2, 15)));
        }
    }
}
function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min) + min);
}