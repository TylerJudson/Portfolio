export class Player {
    constructor(x, y, r, color) {
        this.x = x;
        this.y = y;
        this.r = r;
        this.color = color;

        this.velocity = {
            x: 0,
            y: 0
        }
        this.speed = 0;
    }
    update() {
        this.x += this.velocity.x;
        this.y += this.velocity.y
    }
    render(ctx) {
        
    }
}