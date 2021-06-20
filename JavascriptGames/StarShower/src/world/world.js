import { Background } from "./background/background.js";
import { Star } from "./star.js";

export class World {
    constructor() {
        this.width = 1900;
        this.height = 1000;
        this.center = {
            x: this.width/2,
            y: this.height/2
        }
        this.background = new Background(this.width, this.height);
        this.stars = [];
        this.createStars();
    }
    update() {
        this.stars.forEach((star) => {
            star.update();
        })
    }
    render(ctx) {
        this.stars.forEach((star) => {
            star.render(ctx);
        });
    }
    renderBackground(ctx) {
        this.background.render(ctx);
    }
    createStars() {
        setInterval(() => {
            this.stars.push(new Star(getRandomInt(0, this.width), -20, getRandomInt(3, 10), getRandomInt(-5, 5), getRandomInt(10, 20)))
        }, getRandomInt(2000, 4000));
    }
}

function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min) + min);
}