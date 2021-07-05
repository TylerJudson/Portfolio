export class Engine {
    constructor(update, render, timeStep=1000/30) {
        this.accumulatedTime = 0;
        this.animationFrameRequest = undefined;
        this.time = undefined;
        this.timeStep = timeStep;

        this.updated = false;

        this.update = update;
        this.render = render;

        this.handleRun = (timeStep) => { 
            this.run(timeStep); 
        };
    }
    run(timeStamp) {
        this.animationFrameRequest = window.requestAnimationFrame(this.handleRun);
        this.accumulatedTime += timeStamp - this.time;
        this.time = timeStamp;

        if (this.accumulatedTime >= this.timeStep * 3) {
            this.accumulatedTime = this.timeStep;
        }

        while (this.accumulatedTime >= this.timeStep) {
            this.accumulatedTime -= this.timeStep;
            this.update(timeStamp);
            this.updated = true;
        }

        if (this.updated) {
            this.updated = false;
            this.render(timeStamp);
        }
    }
    start() {
        this.accumulatedTime = this.timeStep;
        this.time = window.performance.now();
        this.animationFrameRequest = window.requestAnimationFrame(this.handleRun);
    }
    stop() { 
        window.cancelAnimationFrame(this.animationFrameRequest); 
    }
}
  