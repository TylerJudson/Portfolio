export default class Display {
    constructor(width, height) {
        this.canvas = document.querySelector("canvas");
        this.ctx = this.canvas.getContext("2d");
        this.width = width;
        this.height = height;
        this.widthHeightRatio = this.width/this.height;
        this.scale = 1;
        this.strokeColor = "black";
        this.paintLineWidth = 1;
        this.earseLineWidth = 10;
        this.paintCap = "butt";
        this.eraseCap = "butt";
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
    earseLine(x1, y1, x2, y2) {
        this.ctx.strokeStyle = "white";
        this.ctx.lineWidth = this.earseLineWidth;
        this.ctx.lineCap = this.eraseCap;
        this.ctx.beginPath();
        this.ctx.moveTo(x1, y1);
        this.ctx.lineTo(x2, y2);
        this.ctx.stroke();
    }
    renderLine(x1, y1, x2, y2) {
        this.ctx.strokeStyle = this.strokeColor;
        this.ctx.lineWidth = this.paintLineWidth;
        this.ctx.lineCap = this.paintCap;
        this.ctx.beginPath();
        this.ctx.moveTo(x1, y1);
        this.ctx.lineTo(x2, y2);
        this.ctx.stroke();
    }
    clear() {
        this.ctx.clearRect(0, 0, this.width, this.height);
    }
}


