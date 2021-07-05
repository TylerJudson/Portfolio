export class Controller {
    constructor() {
        this.upRed = new Button();
        this.rightRed = new Button();
        this.downRed = new Button();
        this.leftRed = new Button();
        
        this.upBlue = new Button();
        this.rightBlue = new Button();
        this.downBlue = new Button();
        this.leftBlue = new Button();
    }
    keyDownUp(type, keyCode) {
        var down = type === "keydown" ? true : false;
        switch (keyCode) {
            case "w":
                this.upRed.changeState(down);
                break;
            case "a":
                this.leftRed.changeState(down);
                break;
            case "s":
                this.downRed.changeState(down);
                break;
            case "d":
                this.rightRed.changeState(down);
                break;
            case "ArrowUp":
                this.upBlue.changeState(down);
                break;
            case "ArrowLeft":
                this.leftBlue.changeState(down);
                break;
            case "ArrowDown":
                this.downBlue.changeState(down);
                break;
            case "ArrowRight":
                this.rightBlue.changeState(down);
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
  