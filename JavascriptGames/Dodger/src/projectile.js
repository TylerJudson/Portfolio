export class Projectile {
  constructor(ship, game) {
    this.game = game;
    this.size = 20;
    this.position = {
      x: ship.position.x + ship.size / 2 - this.size / 2,
      y: ship.position.y - this.size
    };
    this.img = document.getElementById("circle_Img");
    this.speed = 10;
    this.markedForDeletion = false;
    this.positions = {
      top: this.position.y,
      bottom: this.position.y + this.size,
      rightSide: this.position.x + this.size,
      leftSide: this.position.x
    };
  }
  update(ctx) {
    this.positions = {
      top: this.position.y,
      bottom: this.position.y + this.size,
      rightSide: this.position.x + this.size,
      leftSide: this.position.x
    };
    ctx.drawImage(
      this.img,
      this.position.x,
      this.position.y,
      this.size,
      this.size
    );
    this.position.y -= this.speed;
    if (this.position.y <= -5) {
      this.markedForDeletion = true;
      this.game.projectile = null;
    }
  }
}
