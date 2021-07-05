import { Ship } from "./ship.js";
import { Enemy }from "./enemy.js";
import { InputHandler } from "./input.js";
import { getRandomNum } from "./getRandom.js";
import { getElapsedTime } from "./getElapsedTime.js";
import { Projectile } from "./projectile.js";
import { Life } from "./life.js";
import { Clear } from "./clear.js";

export class Game {
  constructor(gameSize, ctx) {
    this.gameSize = gameSize;
    this.gameObjects = [];

    this.lives = 3;
    this.clears = 2;
    this.ship = new Ship(this);
    this.projectile = null;
    new InputHandler(this.ship, this, ctx);
    this.pause = false;
    this.date = new Date();
    this.seconds = 0;
  }
  start() {
    this.gameObjects = [this.ship];
  }
  update(ctx) {
    if (this.lives === 0) {
      this.showGameOverScreen(ctx);
    } else if (this.pause) {
      this.showPauseScreen(ctx);
    } 
    else {
      this.summonEnemies(ctx);
      this.summonLife();
      this.summonClear();
      this.drawScore(ctx);
      [...this.gameObjects].forEach((object) => object.update(ctx));
      this.gameObjects = this.gameObjects.filter((object) => !object.markedForDeletion);
    }
    this.draw(ctx);
  }
  fire(ctx) {
    if (this.projectile === null) {
        this.projectile = new Projectile(this.ship, this, ctx)
        this.gameObjects.push(this.projectile);
    }
  }
  clear(ctx) {
    if (this.clears > 0) {
        this.clears--;
        this.gameObjects = [this.ship];
    }
  }
  draw(ctx) {
      this.drawLives(ctx);
      this.drawClears(ctx);
  }
  drawLives(ctx) {
    for (let i = 1; i < this.lives; i++) {
      let img = document.getElementById("ship_Img");
      ctx.drawImage(img, i * 20 - 15, this.gameSize - 50, 15, 15);
    }
  }
  drawClears(ctx) {
    for (let i = 0; i < this.clears; i++) {
        let img = document.getElementById("star_Img");
        ctx.drawImage(img, this.gameSize - 22 - i*20, this.gameSize - 50, 20, 20)
    }
  }
  showGameOverScreen(ctx) {
    ctx.rect(0, 0, this.gameSize, this.gameSize);
    ctx.fillStyle = "rgba(0, 0, 0)";
    ctx.fill();

    ctx.font = "30px Arial";
    ctx.fillStyle = "white";
    ctx.textAlign = "center";
    ctx.fillText("GAME OVER", this.gameSize / 2, this.gameSize / 2);

    ctx.font = "20px Arial";
    ctx.fillText(
      "You survived: " + this.seconds + " Seconds!",
      this.gameSize / 2,
      this.gameSize / 2 + 40
    );
  }
  summonEnemies(ctx) {
    this.seconds = getElapsedTime(this.date);
    let sec = this.seconds / 30;
    if (getRandomNum(1, 30 + sec) > 29) {
      this.gameObjects.push(new Enemy(this, getRandomNum(sec * 3, 3 * sec + 5)));
    }
  }
  drawScore(ctx) {
    ctx.font = "20px Arial";
    ctx.fillStyle = "white";
    ctx.textAlign = "center";
    ctx.fillText("Score: " + this.seconds, 50, 40);
  }
  summonLife(ctx) {
    if (getRandomNum(1, 300) > 299) {
      this.gameObjects.push(new Life(this));
    }
  }
  summonClear() {
    if (getRandomNum(1, 500) > 499) {
      this.gameObjects.push(new Clear(this));
    }
  }
  togglePause() {
    if (this.pause === true) {
      this.pause = false;
    }
    else {
      this.pause = true;
    }
  }
  showPauseScreen(ctx) {
    ctx.rect(0, 0, this.gameSize, this.gameSize);
    ctx.fillStyle = "rgba(0, 0, 0)";
    ctx.fill();

    ctx.font = "30px Arial";
    ctx.fillStyle = "white";
    ctx.textAlign = "center";
    ctx.fillText("Paused", this.gameSize / 2, this.gameSize / 2);

    ctx.font = "20px Arial";
    ctx.fillText(
      "You've survived: " + this.seconds + " Seconds!",
      this.gameSize / 2,
      this.gameSize / 2 + 40
    );
  }
}
