export class Collider {
    constructor(x, y, w, h) {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        this.old = {
            x: x,
            y: y
        }
    }
    getTop() {
        return this.y - this.h/2;
    }
    getRight() {
        return this.x + this.w/2;
    }
    getBottom() {
        return this.y + this.h/2;
    }
    getLeft() {
        return this.x - this.w/2;
    }
    
    getOldTop() {
        return this.old.y - this.h/2;
    }
    getOldRight() {
        return this.old.x + this.w/2 - 5;
    }
    getOldBottom() {
        return this.old.y + this.h/2;
    }
    getOldLeft() {
        return this.old.x - this.w/2;
    }

    update(x, y) {
        this.old.x = this.x;
        this.old.y = this.y;
        this.x = x;
        this.y = y;
    }
    render(ctx) {
        ctx.save();

        ctx.translate(this.x - this.w/2, this.y - this.h/2);

        ctx.strokeStyle = "white";
        ctx.strokeRect(0, 0, this.w, this.h);

        ctx.restore();
    }
}