export class Display {
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
            this.canvas.width = (window.innerHeight - 30) * this.widthHeightRatio;
            this.canvas.height = window.innerHeight - 30;
            this.scale = (window.innerHeight - 30)/this.height; 
            this.ctx.scale(this.scale, this.scale);
        }
        else {
            this.canvas.width = window.innerWidth - 30;
            this.canvas.height = (window.innerWidth - 30) / this.widthHeightRatio;
            this.scale = (window.innerWidth - 30)/this.width;
            this.ctx.scale(this.scale, this.scale);
        }
    }
}