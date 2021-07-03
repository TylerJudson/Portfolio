export function getElapsedTime(startTime) {
    let time = new Date();
    var timeDiff = time - startTime; //in ms
    // strip the ms
    timeDiff /= 1000;
    // get seconds 
    return Math.round(timeDiff);
    
}