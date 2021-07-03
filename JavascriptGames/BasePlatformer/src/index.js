import { Engine } from "./engine.js";
import { Display } from "./display.js";
import { World } from "./world/world.js";
import { Controller } from "./controller.js";

let world;
let display;
let engine;
let controller;

function init() {
    world = new World();
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
    if (controller.up.active) {
        world.player.jump();
    }
    if (controller.right.active) {
        world.player.moveRight();
    }
    if (controller.left.active) {
        world.player.moveLeft();
    }
}
function render() {
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

