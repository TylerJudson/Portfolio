import { Point } from "./point.js";

export class Movement {
    static stop = 0;
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
        if (velocity.y >= Movement.stop && Math.round(velocity.y) === Movement.stop) {
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