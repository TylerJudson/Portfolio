import { map, collisionMap } from "./map.js";
import { Player } from "./player.js";

export class World {
    constructor() {
        this.friction = 0.9;
        this.gravity = 1;

        this.columns = 20;
        this.rows = 15;
        this.tileSize = 20;

        this.map = map;
        this.collisionMap = collisionMap;
        this.collisionSystem = new CollisionSystem();


        this.width = this.columns * this.tileSize; // 400
        this.height = this.rows * this.tileSize; // 300

        this.player = new Player(this.width/2, this.height/2, 20, "red");
    }
    update() {

        this.player.velocity.y += this.gravity;

        this.player.velocity.x *= this.friction;
        this.player.velocity.y *= this.friction;

        this.player.update();


        if (this.player.velocity.x < 0.1 && this.player.velocity.x > -0.1 && this.player.velocity.y < 0.1 && this.player.velocity.y > -0.1)
          return

        this.checkCollision(this.player);
    }
    //#region Rendering
    render(ctx) {
        this.renderMap(ctx);

        this.player.render(ctx);
    }
    renderMap(ctx) {
        ctx.save();
        for (let i = this.map.length - 1; i > -1; i--) {
            let value = this.map[i];
            let x = (i % this.columns) * this.tileSize;
            let y = Math.floor(i / this.columns) * this.tileSize;
            switch (value) {
                case 1:
                    ctx.fillStyle = "blue";
                    break;
                default:
                    ctx.fillStyle = "gray";
                    break;
            }
            ctx.fillRect(x, y, this.tileSize, this.tileSize);
        }
        ctx.restore();
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
    //#endregion


    //#region Collisions
    checkCollision(object) {

        console.log(`Create collision begin T: ${object.collider.getTop()} R: ${object.collider.getRight()} B: ${object.collider.getBottom()} L: ${object.collider.getLeft()} `);
        let top, right, bottom, left, value;

        top = Math.floor(object.collider.getTop() / this.tileSize);
        left = Math.floor(object.collider.getLeft() / this.tileSize);
        value = this.collisionMap[top * this.columns + left];
        this.collisionSystem.collide(value, object, left * this.tileSize, top * this.tileSize, this.tileSize);

        top = Math.floor(object.collider.getTop() / this.tileSize);
        right = Math.floor(object.collider.getRight() / this.tileSize);
        value = this.collisionMap[top * this.columns + right];
        this.collisionSystem.collide(value, object, right * this.tileSize, top * this.tileSize, this.tileSize);

        bottom = Math.floor(object.collider.getBottom() / this.tileSize);
        right = Math.floor(object.collider.getRight() / this.tileSize);
        value = this.collisionMap[bottom * this.columns + right];
        this.collisionSystem.collide(value, object, right * this.tileSize, bottom * this.tileSize, this.tileSize);

        bottom = Math.floor(object.collider.getBottom() / this.tileSize);
        left = Math.floor(object.collider.getLeft() / this.tileSize);
        value = this.collisionMap[bottom * this.columns + left];
        this.collisionSystem.collide(value, object, left * this.tileSize, bottom * this.tileSize, this.tileSize);

        console.log("create collision end")
    }
    //#endregion
}



class CollisionSystem {
    collide(value, object, tx, ty, tsize) {
        switch (value) {
            case 1:
                this.collidePlatformLeft(object, tx);
                break;
            case 2:
                this.collidePlatformBottom(object, ty + tsize);
                break;
            case 3:
                break;
            case 4:
                this.collidePlatformRight(object, tx + tsize)
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                this.collidePlatformTop(object, ty);
                break;
            default:
                break;
        }
    }
    
    collidePlatformTop(object, tileTop) {
        if (object.collider.getBottom() > tileTop /* && object.collider.getOldBottom() <= tileTop */) {
            object.velocity.y = 0;
            object.bottom = tileTop;
            object.jumping = false;
            console.log("collide top");
            return true;
        }
        return false;
    }
    collidePlatformRight(object, tileRight) {
        if (object.collider.getLeft() < tileRight && object.collider.getOldLeft() >= tileRight) {
            object.velocity.x = 0;
            object.left = tileRight;
            console.log("collide right");
            return true;
        }
        return false;
    }
    collidePlatformLeft(object, tileLeft) {
        if (object.collider.getRight() > tileLeft && object.collider.getOldRight() <= tileLeft) {
            object.right = tileLeft;
            object.velocity.x = 0;
            console.log("collide left");
            return true;
        }
        return false;
    }
    collidePlatformBottom(object, tileBottom) {
        if (object.collider.getTop() < tileBottom /* && object.collider.getOldTop() >= tileBottom */) {
            object.velocity.y = 0;
            object.top = tileBottom;
            console.log("collide bottom");
            return true;
        }
        return false
    }    
}