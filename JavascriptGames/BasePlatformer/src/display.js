export class Display {
    static frame = 30;
    constructor(width, height) {
        this.width = width;
        this.height = height;
        this.widthHeightRatio = width/height;
        this.scale = 1;

        this.canvas = document.querySelector("canvas");
        this.ctx = this.canvas.getContext("2d");
        
        this.resize();
    }
    resize() {
        if (window.innerWidth / window.innerHeight > this.widthHeightRatio) {
            this.canvas.width = (window.innerHeight - Display.frame) * this.widthHeightRatio;
            this.canvas.height = window.innerHeight - Display.frame;
            this.scale = (window.innerHeight - Display.frame)/this.height;
            this.ctx.scale(this.scale, this.scale);
        }
        else {
            this.canvas.width = window.innerWidth - Display.frame;
            this.canvas.height = (window.innerWidth - Display.frame) / this.widthHeightRatio;
            this.scale = (window.innerWidth - Display.frame)/this.width;
            this.ctx.scale(this.scale, this.scale);
        }
    }
    clear() {
        this.ctx.clearRect(0, 0, this.width, this.height);
    }
}