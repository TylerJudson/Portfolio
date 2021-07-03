export class Controller {
    constructor() {
        this.up = new Button();
        this.right = new Button();
        this.left = new Button();
    }
    keyDownUp(type, keyCode) {
        var down = type === "keydown" ? true : false;
        switch (keyCode) {
            case "w":
            case "ArrowUp":
                this.up.changeState(down);
                break;
            case "d":
            case "ArrowRight":
                this.right.changeState(down);
                break;
            case "a":
            case "ArrowLeft":
                this.left.changeState(down);
                break;
            default:
                break;
        }
    }
}

class Button {
    constructor() {
        this.active = false;
    }
    changeState(down) {
        this.active = down;
    }
}


