import { Direction } from "./direction.js";

export class InputHandler {
  constructor(ship, game, ctx) {
    document.addEventListener("keydown", (event) => {
      switch (event.keyCode) {
        case 37:
          ship.move(Direction.LEFT);
          break;
        case 39:
          ship.move(Direction.RIGHT);
          break;
        case 38:
            ship.move(Direction.UP);
            break;
        case 40:
            ship.move(Direction.DOWN);
            break;
        case 70:
            game.fire(ctx);
            break;
        case 27:
          game.togglePause();
          break;
        case 80:
            game.togglePause();
            break;
        case 32:
          game.clear(ctx);
          break;
        default:
      }
    });
    document.addEventListener("keyup", (event) => {
        switch (event.keyCode) {
            case 37:
              ship.stopx();
              break;
            case 39:
              ship.stopx();
              break;
            case 38:
                ship.stopy();
                break;
            case 40:
                ship.stopy();
                break;
            default:
          }
      });
  }
}
