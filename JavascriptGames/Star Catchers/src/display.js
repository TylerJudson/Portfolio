export class Display {
    constructor(width, height) {
        this.width = width;
        this.height = height;
        this.widthHeightRatio = width/height;
        this.scale = 1;

        this.canvas = document.querySelector("canvas");
        this.ctx = this.canvas.getContext("2d");
        this.ctx.fillStyle = "white";
        this.ctx.strokeStyle = "white";

        this.redScore = document.querySelector("#redScore");
        this.blueScore = document.querySelector("#blueScore");
        this.redScoreText = 0;
        this.blueScoreText = 0;
        this.timer = document.querySelector("#timer");

        this.startScreen = document.querySelector("#startScreen");
        this.gameScreen = document.querySelector("#gameScreen");
        this.endScreen = document.querySelector("#endScreen");
        this.helpScreen = document.querySelector("#helpScreen");

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

        var rect = this.canvas.getBoundingClientRect();
        this.redScore.style.top = rect.top + "px";
        this.redScore.style.left = rect.left + 100 + "px";
        this.blueScore.style.top = rect.top + "px";
        this.blueScore.style.left = rect.right - 300 + "px";
        this.timer.style.top = rect.top + "px";
        this.timer.style.transform = "translate(-50%, 0%)";
        this.timer.style.color = "white";
        this.timer.style.fontSize = "4rem";

    }
    clear() {
        this.ctx.save();
        this.ctx.fillStyle = "rgba(0, 0, 0, .5)";
        this.ctx.fillRect(0, 0, this.width, this.height);
        this.ctx.restore();
    }
    setScore(redScore, blueScore) {
        this.redScoreText = redScore;
        this.blueScoreText = blueScore;
        this.redScore.innerHTML = `Score: ${redScore}`;
        this.blueScore.innerHTML = `Score: ${blueScore}`;
    }
    hideStartScreen() {
        this.startScreen.style.visibility = "hidden";
    }
    showStartScreen() {
        this.hideHelpScreen();
        this.hideGameScreen();
        this.hideEndScreen();
        this.startScreen.style.visibility = "visible";
    }

    showGameScreen() {
        this.hideHelpScreen();
        this.hideStartScreen();
        this.hideEndScreen();
        this.gameScreen.style.visibility = "visible";
    }
    hideGameScreen() {
        this.gameScreen.style.visibility = "hidden";
    }

    showEndScreen() {
        this.hideHelpScreen();
        this.hideStartScreen();
        if (this.redScoreText === this.blueScoreText) {
            document.querySelector("#endTitle").style.color = "black";
            document.querySelector("#endTitle").innerHTML = "It's a Draw!!";
        }
        else {
            document.querySelector("#endTitle").innerHTML = this.redScoreText > this.blueScoreText ? "Congrats Red!" : "Congrats Blue!";
            document.querySelector("#endTitle").style.color = this.redScoreText > this.blueScoreText ? "red" : "blue";
        }
       
        this.endScreen.style.visibility = "visible";
    }
    hideEndScreen() {
        this.endScreen.style.visibility = "hidden";
    }
    showHelpScreen() {
        this.hideGameScreen();
        this.hideStartScreen();
        this.hideEndScreen();
        
        this.helpScreen.style.visibility = "visible";
    }
    hideHelpScreen() {
        this.helpScreen.style.visibility = "hidden";
    }
    setTimer(num) {
        if (num > 3) {
            var rect = this.canvas.getBoundingClientRect();
            this.timer.style.top = rect.top + "px";
            this.timer.style.transform = "translate(-50%, 0%)";
            this.timer.style.color = "white";
            this.timer.style.fontSize = "4rem";
        }
        else if (num <= 3) {
            this.timer.style.top = "50%";
            this.timer.style.left = "50%";
            this.timer.style.transform = "translate(-50%, -50%)";
            this.timer.style.color = "blue";
            this.timer.style.fontSize = "20rem";
        }
        if (num === 3) {
            this.ctx.save();
            this.ctx.fillStyle = "rgba(255, 255, 255, .25)";
            this.ctx.fillRect(0, 0, this.width, this.height);
            this.ctx.restore();
        }
        if (num === 2) {
            this.ctx.save();
            this.ctx.fillStyle = "rgba(255, 255, 255, .25)";
            this.ctx.fillRect(0, 0, this.width, this.height);
            this.ctx.restore();
            this.timer.style.color = "red";
        }
        if (num === 1) {
            this.ctx.save();
            this.ctx.fillStyle = "rgba(255, 255, 255, .25)";
            this.ctx.fillRect(0, 0, this.width, this.height);
            this.ctx.restore();
            this.timer.style.color = "yellow";
        }
        if (num === 0) {this.ctx.save();
            this.ctx.fillStyle = "rgba(255, 255, 255, .25)";
            this.ctx.fillRect(0, 0, this.width, this.height);
            this.ctx.restore();
            this.timer.style.color = "white";
            num = "GO!";
        }
        this.timer.innerHTML = num;
    }
}