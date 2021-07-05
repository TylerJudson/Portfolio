import { Engine } from "./engine.js";
import { Display } from "./display.js";
import { World } from "./world/world.js";
import { Controller } from "./controller.js";
import { Jump, Movement } from "./utils/movement.js";
import { Point } from "./utils/point.js";

let world;
let display;
let engine;
let controller;

function init() {
    let playerImg = new Image();
    playerImg.src = "./img/Ball.png";
    world = new World(700, 400, 20, playerImg);

    display = new Display(world.width, world.height);
    controller = new Controller();

    engine = new Engine(update, render);
    engine.start();
}

function update() {
    updateController();
    world.update();
}

function updateController() {
    const playerRightSpeed = 0.5;
    const playerLeftSpeed = -0.5;

    if (controller.up.active) {
        world.player.move(new Jump());
    }
    if (controller.right.active) {
        world.player.move(new Movement(new Point(playerRightSpeed, 0)));
    }
    if (controller.left.active) {
        world.player.move(new Movement(new Point(playerLeftSpeed, 0)));
    }
}

function render() {
    display.clear();
    world.render(display.ctx);
}






onload = init;


addEventListener("resize", () => {
    if (display != null) {
        display.resize();
    }
})
addEventListener("keydown", (event) => {
    if (controller != null) {
        controller.keyDownUp(event.type, event.key);
    }
})
addEventListener("keyup", (event) => {
    if (controller != null) {
        controller.keyDownUp(event.type, event.key);
    }
})
document.getElementById("leftButton").ontouchstart = () => {
    controller.left.active = true;
}
document.getElementById("leftButton").ontouchend = () => {
    controller.left.active = false;
}
document.getElementById("rightButton").ontouchstart = () => {
    controller.right.active = true;
}
document.getElementById("rightButton").ontouchend = () => {
    controller.right.active = false;
}
document.getElementById("jumpButton").ontouchstart = () => {
    controller.up.active = true;
}
document.getElementById("jumpButton").ontouchend = () => {
    controller.up.active = false;
}

