import { Particle } from "./particle.js";

export class Explosion {
    constructor(x, y, color, amount) {
        this.x = x;
        this.y = y;
        this.color = color;
        this.amount = amount;
        this.friction = .95;
        this.particles = []
        this.createParticles();

    }
    update() {
        this.particles.forEach(particle => {
            particle.update();

            if (particle.alpha <= 0) {
                this.remove(particle, this.particles);
            }
            else {
                particle.velocity.x *= this.friction;
                particle.velocity.y *= this.friction;
            }
        });
    }
    render(ctx) {
        this.particles.forEach(particle => {
            particle.render(ctx);
        })
    }
    createParticles() {
        for (let i = this.amount; i > 0; i--) {
            this.particles.push(new Particle(
                this.x,
                this.y,
                (Math.random() - 0.5) * Math.random() * this.amount/2,
                (Math.random() - 0.5) * Math.random() * this.amount/2,
                this.color
                ));
        }
    }
    remove(element, array) {
        var index = array.indexOf(element);
        setTimeout(() => {
            array.splice(index, 1);
        }, 0);
    }
}