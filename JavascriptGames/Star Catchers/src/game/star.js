import { CollidableObject } from "./collidableObject.js";

export class Star extends CollidableObject {
    constructor(x, y, radius) {
        super(x, y, radius);
        this.x = x;
        this.y = y;
        this.radius = radius;

        this.radians = Math.random() * Math.PI * 2
        this.img = new Image();
        this.color = Math.random() > 0.66 ? Math.random() > .5 ? "blue" : "red" : "yellow";
        if (this.color === "blue") {
            this.img.src = "./img/blueStar.png";
        }
        else if (this.color === "red") {
            this.img.src = "./img/redStar.png";
        }
        else {
            this.img.src = "./img/star.png";
        }
        this.type = Math.random() > .75 ? "fading" : "normal";
        this.isfaded = false;
        this.alpha = 1;
    }
    update() {
        this.radians += 0.01;    
        this.update1(this.x, this.y, this.computeVertices());
    }
    computeVertices() {
        return [
            {x: this.x - Math.cos(Math.PI/2 + this.radians) * this.radius, y: this.y - Math.sin(Math.PI/2 + this.radians) * this.radius},
            {x: this.x - Math.cos(Math.PI * 9/10 + this.radians) * this.radius, y: this.y - Math.sin(Math.PI * 9/10 + this.radians) * this.radius},
            {x: this.x - Math.cos(Math.PI * 13/10 + this.radians) * this.radius, y: this.y - Math.sin(Math.PI * 13/10 + this.radians) * this.radius},
            {x: this.x - Math.cos(Math.PI * 17/10 + this.radians) * this.radius, y: this.y - Math.sin(Math.PI * 17/10 + this.radians) * this.radius},
            {x: this.x - Math.cos(Math.PI/10 + this.radians) * this.radius, y: this.y - Math.sin(Math.PI/10 + this.radians) * this.radius},
        ];
    }
    render(ctx) {
        ctx.save();
        if (this.type === "fading" && this.alpha > 0.01) {
            ctx.globalAlpha = this.alpha;
            this.alpha -= 0.005;
        }
        else if (this.type === "fading") {
            ctx.globalAlpha = 0;
            this.isfaded = true;
        }
        ctx.translate(this.x, this.y);
        ctx.rotate(this.radians);

        ctx.drawImage(this.img, -this.radius, -this.radius, this.radius * 2, this.radius * 1.85);
        ctx.restore();
    }
    
}