import { Game } from "./game.js";


// Create the canvas
let canvas = document.getElementById("gameScreen");
let ctx = canvas.getContext("2d");

// Initialize the gamesize 
const GAME_SIZE = 500;

let game = new Game(GAME_SIZE, ctx);
game.start();

// Repeats
function gameLoop()  {
  // Clear the screen
  ctx.clearRect(0,0, GAME_SIZE, GAME_SIZE);
  game.update(ctx);
  requestAnimationFrame(gameLoop);
}

// Repeats
requestAnimationFrame(gameLoop);

