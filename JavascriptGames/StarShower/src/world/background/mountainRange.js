import { Mountain } from "./mountain.js";

export class MountainRange {
    constructor(worldWidth, worldHeight) {
        this.worldWidth = worldWidth;
        this.worldHeight = worldHeight;
        this.mountains = []
    }
    render(ctx) {
        this.mountains.forEach((mountain) => {
            mountain.render(ctx);
        });
    }
}

export class LightGrayMountainRange extends MountainRange {
    constructor(worldWidth, worldHeight) {
        super(worldWidth, worldHeight);
        this.colorValue = getRandomInt(10, 25);
        for (let i = 0; i <= getRandomInt(5, 15); i++) {
            let x1 = getRandomInt(-200, worldWidth - 100);
            let x2 = getRandomInt(x1 + 300, x1 + 1000);
            let h = getRandomInt(600, 800);
            this.mountains.push(new Mountain(x1, x2, worldHeight, h, this.colorValue));
        }
    }
}  
export class GraymountainRange extends MountainRange {
    constructor(worldWidth, worldHeight) {
        super(worldWidth, worldHeight);
        this.colorValue = getRandomInt(30, 45);
        for (let i = 0; i <= getRandomInt(3, 10); i++) {
            let x1 = getRandomInt(-300, worldWidth/4 * 3);
            let x2 = getRandomInt(x1 + 500, x1 + 1500);
            let h = getRandomInt(400, 600);
            this.mountains.push(new Mountain(x1, x2, worldHeight, h, this.colorValue));
        }
    }
}

export class DarkGrayMountainRange extends MountainRange {
    constructor(worldWidth, worldHeight) {
        super(worldWidth, worldHeight);
        this.colorValue = getRandomInt(50, 65);
        for (let i = 0; i <= getRandomInt(3, 10); i++) {
            let x1 = getRandomInt(-400, worldWidth/3 * 2);
            let x2 = getRandomInt(x1 + 1000, x1 + 1000);
            let h = getRandomInt(50, 300);
            this.mountains.push(new Mountain(x1, x2, worldHeight, h, this.colorValue));
        }
    }
}

function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min) + min);
}