export class BackgroundStar {
    constructor(x, y, radius, alpha) {
        this.x = x;
        this.y = y;
        this.radius = radius;
        this.alpha = alpha;
        this.velocity = {
            x: Math.random() * 0.15 - 0.15,
            y: Math.random() * 0.15 - 0.15
        };
    }
    update() {
        this.x += this.velocity.x;
        this.y += this.velocity.y;
    }
    render(ctx) {
        ctx.save();
        ctx.beginPath();
        ctx.fillStyle = "white";
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
        ctx.restore();
    }
}