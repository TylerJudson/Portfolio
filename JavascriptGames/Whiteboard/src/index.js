import Display from "./display.js";


//#region Varibles
const display = new Display(1200, 800);
var mouseDown = false;
var previousMousePos;
var settingsVisibility = "hidden";
var exitButton = document.querySelector("#Exit");
var clearButton = document.querySelector("#ClearScreen");
var draw = true;

//#endregion

//#region Methods 
function init() {
    display.resize();
}
function paint(x, y) {
    if (mouseDown) {
        display.renderLine(previousMousePos.x, previousMousePos.y, x, y);
        previousMousePos = {
            x: x,
            y: y
        }
    }
}
function earse(x, y) {
    if (mouseDown) {
        display.earseLine(previousMousePos.x, previousMousePos.y, x, y);
        previousMousePos = {
            x: x,
            y: y
        }
    }
}
//#endregion



window.onload = init();





//#region Listeners
addEventListener("resize", () => {
    display.resize();
});
addEventListener("mousedown", (event) => {
    if (event.button === 0 && settingsVisibility === "hidden")
    {
        let marginX = (window.innerWidth - display.canvas.width)/2;
        let marginY = (window.innerHeight - display.canvas.height)/2;
        let x = (event.clientX - marginX) / display.scale;
        let y = (event.clientY - marginY) / display.scale;
        previousMousePos = {
            x: x,
            y: y
        }
        mouseDown = true;
    }
    else if (event.button === 2) {
        if (settingsVisibility === "visible")
        {
            document.querySelector("#SettingsUI").style.visibility = "hidden";
            settingsVisibility = "hidden";
        }
        else {
            document.querySelector("#SettingsUI").style.visibility = "visible";
            settingsVisibility = "visible";
        }
        
    }
    
});
addEventListener("mouseup", () => {
    mouseDown = false;
});
addEventListener("mousemove", (event) => {
    let marginX = (window.innerWidth - display.canvas.width)/2;
    let marginY = (window.innerHeight - display.canvas.height)/2;
    let x = (event.clientX - marginX) / display.scale;
    let y = (event.clientY - marginY) / display.scale;
    if (draw)
    {
        paint(x, y);
    }
    else {
        earse(x, y);
    }

});
addEventListener("dblclick", () => {
    if (draw)
    {
        draw = false;
        display.canvas.style.cursor = "not-allowed";
    }
    else {
        draw = true;
        display.canvas.style.cursor = "crosshair";
    }
})
addEventListener("contextmenu", () => {
    window.event.returnValue = false
})





exitButton.addEventListener("click", () => {
    document.querySelector("#SettingsUI").style.visibility = "hidden";
    settingsVisibility = "hidden";
});
clearButton.addEventListener("click", () => {
    display.clear();
}) ;
document.querySelector("#redButton").addEventListener("click", () => {
    display.strokeColor = "red";
});
document.querySelector("#orangeButton").addEventListener("click", () => {
    display.strokeColor = "orange";
});
document.querySelector("#yellowButton").addEventListener("click", () => {
    display.strokeColor = "yellow";
});
document.querySelector("#goldButton").addEventListener("click", () => {
    display.strokeColor = "gold";
});
document.querySelector("#greenYellowButton").addEventListener("click", () => {
    display.strokeColor = "greenyellow";
});
document.querySelector("#limeButton").addEventListener("click", () => {
    display.strokeColor = "lime";
});
document.querySelector("#cornFlowerBlueButton").addEventListener("click", () => {
    display.strokeColor = "cornflowerblue";
});
document.querySelector("#blueButton").addEventListener("click", () => {
    display.strokeColor = "blue";
});
document.querySelector("#darkBlueButton").addEventListener("click", () => {
    display.strokeColor = "darkblue";
});
document.querySelector("#rebPurpleButton").addEventListener("click", () => {
    display.strokeColor = "rebeccapurple";
});
document.querySelector("#blueVioButton").addEventListener("click", () => {
    display.strokeColor = "blueviolet";
});
document.querySelector("#violetButton").addEventListener("click", () => {
    display.strokeColor = "violet";
});
document.querySelector("#brownButton").addEventListener("click", () => {
    display.strokeColor = "saddlebrown";
});
document.querySelector("#greyButton").addEventListener("click", () => {
    display.strokeColor = "grey";
});
document.querySelector("#blackButton").addEventListener("click", () => {
    display.strokeColor = "black";
});




document.querySelector("#EarseButton").addEventListener("click", () => {
    draw = false;
    display.canvas.style.cursor = "not-allowed";
})
document.querySelector("#PaintButton").addEventListener("click", () => {
    draw = true;
    display.canvas.style.cursor = "crosshair";
});








document.querySelector("#e1").addEventListener("click", () => {
    display.earseLineWidth = 1;
});
document.querySelector("#e10").addEventListener("click", () => {
    display.earseLineWidth = 10;
});
document.querySelector("#e25").addEventListener("click", () => {
    display.earseLineWidth = 25;
});
document.querySelector("#e50").addEventListener("click", () => {
    display.earseLineWidth = 50;
});
document.querySelector("#e100").addEventListener("click", () => {
    display.earseLineWidth = 100;
});
document.querySelector("#e150").addEventListener("click", () => {
    display.earseLineWidth = 150;
});
document.querySelector("#e150").addEventListener("click", () => {
    display.earseLineWidth = 150;
});
document.querySelector("#e200").addEventListener("click", () => {
    display.earseLineWidth = 200;
});
document.querySelector("#e250").addEventListener("click", () => {
    display.earseLineWidth = 250;
});
document.querySelector("#e300").addEventListener("click", () => {
    display.earseLineWidth = 300;
});

document.querySelector("#p1").addEventListener("click", () => {
    display.paintLineWidth = 1;
});
document.querySelector("#p5").addEventListener("click", () => {
    display.paintLineWidth = 5;
});
document.querySelector("#p10").addEventListener("click", () => {
    display.paintLineWidth = 10;
});
document.querySelector("#p20").addEventListener("click", () => {
    display.paintLineWidth = 20;
});
document.querySelector("#p40").addEventListener("click", () => {
    display.paintLineWidth = 40;
});
document.querySelector("#p100").addEventListener("click", () => {
    display.paintLineWidth = 100;
});
document.querySelector("#p150").addEventListener("click", () => {
    display.paintLineWidth = 150;
});
document.querySelector("#p200").addEventListener("click", () => {
    display.paintLineWidth = 200;
});
document.querySelector("#p250").addEventListener("click", () => {
    display.paintLineWidth = 250;
});
document.querySelector("#p300").addEventListener("click", () => {
    display.paintLineWidth = 300;
});
document.querySelector("#p400").addEventListener("click", () => {
    display.paintLineWidth = 400;
});




document.querySelector("#pFlat").addEventListener("click", () => {
    display.paintCap = "butt";
})
document.querySelector("#pRound").addEventListener("click", () => {
    display.paintCap = "round";
})
document.querySelector("#eFlat").addEventListener("click", () => {
    display.eraseCap = "butt";
})
document.querySelector("#eRound").addEventListener("click", () => {
    display.eraseCap = "round";
})
//#endregion