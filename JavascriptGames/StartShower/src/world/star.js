export class Star {
    constructor(x, y, r, vx, vy) {
        this.x = x;
        this.y = y;
        this.r = r;
        this.velocity = {
            x: vx,
            y: vy
        }
    }
    update() {
        this.x += this.velocity.x;
        this.y += this.velocity.y;
    }
    render(ctx) {
        ctx.save();

        ctx.beginPath();
        ctx.arc(this.x, this.y, this.r, 0, Math.PI * 2);
        ctx.fillStyle = "white";
        ctx.fill();

        ctx.restore();
    }
}