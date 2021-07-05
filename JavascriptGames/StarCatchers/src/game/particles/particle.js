export class Particle {
    constructor(x, y, vx, vy, color) {
        this.x = x;
        this.y = y;
        this.velocity = {
            x: vx,
            y: vy
        };
        this.color = color;
        this.radius = 2;
        this.alpha = 1;
    }
    update() {
        this.x += this.velocity.x;
        this.y += this.velocity.y;
        this.alpha -= .05;
    }
    render(ctx) {
        ctx.save();
        ctx.globalAlpha = this.alpha <= 0 ? 0 : this.alpha;
        ctx.beginPath();
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.fillStyle = this.color;

        ctx.closePath();
        ctx.fill();
        ctx.restore();
    }
}