import { Point } from "./point.js";

export class Movement {
    constructor(magnitude) {
        this.magnitude = magnitude;
    }
    apply(velocity) {
        velocity.x += this.magnitude.x;
        velocity.y += this.magnitude.y;
    }
}
export class Jump extends Movement {
    constructor() {
        super(new Point(0, -20));
    }
    apply(velocity) {
        if (velocity.y >= 0 && Math.round(velocity.y) === 0) {
            velocity.x += this.magnitude.x;
            velocity.y += this.magnitude.y;
        }
    }
}
export class Gravity extends Movement {
    constructor() {
        super(new Point(0, 1));
    }
}
export class Friction extends Movement {
    constructor() {
        super(new Point(0.9, 0.9));
    }
    apply(velocity) {
        velocity.x *= this.magnitude.x;
        velocity.y *= this.magnitude.y;
    }
}