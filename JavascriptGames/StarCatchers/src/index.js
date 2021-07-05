import { Controller } from "./controller.js";
import { Display } from "./display.js";
import { Engine } from "./engine.js";
import { World } from "./game/world.js";


//#region Varibles
let world;
let engine;
let display;
let controller;
let timeLeft;
let timer;
//#endregion




//#region Functions
function init() {
    world = new World();
    engine = new Engine(update, render);
    display = new Display(world.width, world.height);
    controller = new Controller();

    timeLeft = 3;
    engine.start();
    startBeginningTimer();
}

function update() {
    updateController();
    world.update();
    display.setScore(world.player1.score, world.player2.score);
}
function render() {
    display.clear();

    world.render(display.ctx);
}
function updateController() {
    if (controller.upRed.active) {
        world.player1.accelerate();
    }
    if (controller.leftRed.active) {
        world.player1.rotateLeft();
    }
    if (controller.downRed.active) {
        world.player1.deaccelerate();
    }
    if (controller.rightRed.active) {
        world.player1.rotateRight();
    }
    if (controller.upBlue.active) {
        world.player2.accelerate();
    }
    if (controller.leftBlue.active) {
        world.player2.rotateLeft();
    }
    if (controller.downBlue.active) {
        world.player2.deaccelerate();
    }
    if (controller.rightBlue.active) {
        world.player2.rotateRight();
    }
}
//#region Timers
function startTimer() {
    timer = setInterval(countDown, 1000);
}
function startBeginningTimer() {
    display.setTimer(timeLeft);
    timer = setInterval(beginningCountDown, 1000);
}
function countDown() {
    timeLeft--;
    display.setTimer(timeLeft);
    if (timeLeft === 0) {
        clearInterval(timer);
        engine.stop();
        display.showEndScreen();
    }
}
function beginningCountDown() {
    timeLeft--;
    display.setTimer(timeLeft);
    if (timeLeft === 0) {
        world.createStars();
    }
    else if (timeLeft < 0) {
        clearInterval(timer);
        timeLeft = 100;
        display.setTimer(timeLeft);
        startTimer();
        
    }

}
//#endregion

//#endregion



//#region EventListeners
addEventListener("resize", () => {
    display.resize();
});
addEventListener("keydown", (event) => {
    controller.keyDownUp(event.type, event.key);
})
addEventListener("keyup", (event) => {
    controller.keyDownUp(event.type, event.key);
})



// StartButton
document.querySelector("#startButton").onclick = () => {
    init();
    display.showGameScreen();
}
document.querySelector("#back").onclick = () => {
    display.showStartScreen();
}
// How to play button 
document.querySelector("#howToPlayButton").onclick = () => {
    display = new Display(1, 1);
    display.showHelpScreen();
}
// Play Again Button 
document.querySelector("#playAgainButton").onclick = () => {
    display.showStartScreen();
}
//#endregion