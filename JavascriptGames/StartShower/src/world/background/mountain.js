export class Mountain {
    constructor(x1, x2, y, h, colorValue) {
        this.x1 = x1;
        this.x2 = x2;
        this.y = y;
        this.h = h;
        this.colorValue = colorValue;
    }
    render(ctx) {
        ctx.save();

        let gradient = ctx.createLinearGradient((this.x1 + this.x2)/2, 0, (this.x1 + this.x2)/2, this.h);
        gradient.addColorStop(0, `rgb(${this.colorValue - 10}, ${this.colorValue - 10}, ${this.colorValue - 10})`);
        gradient.addColorStop(1, `rgb(${this.colorValue}, ${this.colorValue}, ${this.colorValue})`);

        ctx.beginPath();
        ctx.moveTo(this.x1, this.y);
        ctx.lineTo((this.x1 + this.x2)/2, this.h);
        ctx.lineTo(this.x2, this.y);
        ctx.fillStyle = gradient;
        ctx.fill();

        ctx.restore();
    }
}