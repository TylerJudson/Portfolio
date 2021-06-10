import { BackgroundStar } from "./particles/backgroundStar.js";
import { Explosion } from "./particles/explosion.js";
import { Player } from "./player.js";
import { Star } from "./star.js";


export class World {
    constructor() {
        this.width = 1800;
        this.height = 900;
        this.friction = 0.95;

        this.backgroundStars = [];
        this.stars = [];
        this.explosions = [];

        this.player1 = new Player(300, this.height/2, 33, 100, "red");
        this.player2 = new Player(1500, this.height/2, 33, 100, "blue");
        this.createBackgroundStars();
    }
    update() {
        this.checkForCollisions();

        this.wrapPlayer();

        this.updateBackgroundStars();
        this.updateStars();
        this.updateExplosions();

        this.player1.update();
        this.player2.update();
        
        this.player1.velocity.y *= this.friction;
        this.player1.velocity.x *= this.friction;
        this.player2.velocity.y *= this.friction;
        this.player2.velocity.x *= this.friction;
    }
    render(ctx) {
        this.renderBackgroundStars(ctx);
        this.renderStars(ctx);
        this.renderExplosions(ctx);

        this.player1.render(ctx);
        this.player2.render(ctx);
    }
    

    //#region Stars
    createStars() {
        let star = new Star(Math.random() * this.width, Math.random() * this.height, (Math.random() + .5) * 30);
        star.update();
        this.stars.push(star);
        //this.stars.push(new Star(this.width/2, this.height/2, 100))
        setTimeout(() => {
            this.createStars()
        }, Math.random() * (3000 - 500) + 500);
    }
    updateStars() {
        this.stars.forEach((star) => {
            star.update();
            if (star.isfaded) {
                this.remove(star, this.stars);
            }
        });
    }
    renderStars(ctx) {
        this.stars.forEach((star) => {
            star.render(ctx);
        })
    }
    //#endregion

    //#region BackgroundStars
    createBackgroundStars() {
        for (let i = 0; i < 200; i++) {
            let x = Math.random() * this.width;
            let y = Math.random() * this.height;

            this.backgroundStars.push(new BackgroundStar(x, y, 2, .4));
        }
    }
    updateBackgroundStars() {
        this.backgroundStars.forEach((star) => {
            star.update();
        });
    }
    renderBackgroundStars(ctx) {
        this.backgroundStars.forEach((star) => {
            star.render(ctx);
        });
    }
    //#endregion

    updateExplosions() {
        this.explosions.forEach(explosion => {
            explosion.update();
        });
    }
    renderExplosions(ctx) {
        this.explosions.forEach(explosion => {
            explosion.render(ctx);
        });
    }
    checkForCollisions() {
        if (this.player1.didCollide(this.player2)) {
            this.player1.bounce();
            this.player2.bounce();
        }

        //let starsToRemove = [];
        this.stars.forEach(star => {
          //  if (star.isfaded) {
          //      starsToRemove.push(star);
          //  } 
          //  else {
                if (star.didCollide(this.player1)) {
                    if (star.color === "red") {
                        this.player1.score += 3;
                    }
                    else if (star.color === "blue") {
                        this.player1.score--;
                    }
                    else {
                        this.player1.score++;
                    }
                    this.destroyStar(star);
                }
                if (star.didCollide(this.player2)) {
                    if (star.color === "blue") {
                        this.player2.score += 3;
                    }
                    else if (star.color === "red") {
                        this.player2.score--;
                    }
                    else {
                        this.player2.score++;
                    }
                    this.destroyStar(star);
                }
            //} 
        });

        //starsToRemove.forEach(star => { this.remove(star, this.stars); });
    }
    wrapPlayer() {
        if (this.player1.x <= 0) {
            this.player1.x = this.width;
        }
        else if (this.player1.x >= this.width) {
            this.player1.x = 0;
        }
        if (this.player1.y <= 0) {
            this.player1.y = this.height;
        }
        else if (this.player1.y >= this.height) {
            this.player1.y = 0;
        }

        if (this.player2.x <= 0) {
            this.player2.x = this.width;
        }
        else if (this.player2.x >= this.width) {
            this.player2.x = 0;
        }
        if (this.player2.y <= 0) {
            this.player2.y = this.height;
        }
        else if (this.player2.y >= this.height) {
            this.player2.y = 0;
        }
    }
    
    
    showReferenceLines(ctx) {
        ctx.save();
        ctx.beginPath();
        ctx.strokeStyle = "white";

        ctx.moveTo(this.width, this.height);
        ctx.lineTo(0, 0)
        ctx.moveTo(this.width, 0);
        ctx.lineTo(0, this.height);

        ctx.moveTo(this.width/2, this.height);
        ctx.lineTo(this.width/2, 0);
        ctx.moveTo(this.width, this.height/2);
        ctx.lineTo(0, this.height/2);

        ctx.stroke();
        ctx.restore();
    }
    destroyStar(star) {
        this.explosions.push(new Explosion(star.x, star.y, star.color, star.radius));
        this.remove(star, this.stars);
    }
    remove(element, array) {
        var index = array.indexOf(element);
        setTimeout(() => {
            array.splice(index, 1);
        }, 0);
    }
}