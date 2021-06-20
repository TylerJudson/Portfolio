export class BackgroundStar {
    constructor(x, y, r) {
        this.x = x;
        this.y = y;
        this.r = r;
    }
    render(ctx) {
        ctx.save();

        let gradient = ctx.createRadialGradient(this.x, this.y, this.r/5, this.x, this.y, this.r);
        gradient.addColorStop(0, "white");
        gradient.addColorStop(.1, "rgba(255, 255, 255, .3)")
        gradient.addColorStop(1, "rgba(255, 255, 255, 0)")

        ctx.beginPath();
        ctx.arc(this.x, this.y, this.r, 0, Math.PI*2);
        ctx.fillStyle = gradient;
        ctx.fill();

        ctx.restore(); 
    }
}