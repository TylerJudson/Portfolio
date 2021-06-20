import { Engine } from "./engine.js";
import { Display } from "./display.js";
import { World } from "./world/world.js";

let world;
let display;
let engine;
let controller;

function init() {
    world = new World();
    display = new Display(world.width, world.height);

    engine = new Engine(update, render);
    render();
}
function update() {

}
function render() {
    world.render(display.ctx);
}





onload = init;





