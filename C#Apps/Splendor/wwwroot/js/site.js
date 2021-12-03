﻿let TakingTokens = {
    Emerald: 0,
    Sapphire: 0,
    Ruby: 0,
    Diamond: 0,
    Onyx: 0
};






function ToggleScreen(screenID) {
    var x = document.getElementById(screenID);
    if (x.style.display === "none") {
        x.style.display = "block";
    } else {
        x.style.display = "none";
    }
}

function CardScreen(ImageName, purchaseable) {

    let innerHtml = `
        <img class="mx-auto card mt-8
                    mt-md-7"
                    mt-lg-5" src="Images/` + ImageName + `" width="auto" style="max-height: 50%"/>
        <div class="container row mt-5 mx-auto style="width: 100%">
            <div class="col p-0">
            <div class="purple p-0 mx-auto" style="width: 10rem;">
                <button class="mx-auto btn btn-outline-light btn-lg" style="width: 10rem;" onclick='ToggleScreen("CardScreen")'>Back</button>
            </div>
            </div>
            <div class="col p-0">
            <div class="p-0 mx-auto" style="width: 10rem;">
                <button class="mx-auto btn btn-purple btn-lg" style="width: 10rem;" onclick='ToggleScreen("CardScreen")'>Purchase</button>
            </div>
            </div>
        </div>
    `;

    let CardScreen = document.getElementById("CardScreen")

    CardScreen.innerHTML = innerHtml;
    CardScreen.style.display = "block";
}

function ValidateTokens(token) {

    if (CountTakingTokens() >= 3) {
        return false;
    }


    for (key in TakingTokens) {
        if (TakingTokens[key] >= 2) {
            return false;
        }
    }

    if (CountTakingTokens() >= 2 && TakingTokens[token] > 0) {
        return false;
    }

    return true;
}

function CountTakingTokens() {
    let sum = 0;
    for (token in TakingTokens) {
        sum += TakingTokens[token];
    }
    return sum;
}


function TokenClick(token) {
    switch (token) {
        case "TakingEmerald":
            let EmeraldTakingToken = document.getElementById("EmeraldTakingTokenValue");
            if (EmeraldTakingToken.innerHTML > 0) {
                EmeraldTakingToken.innerHTML--;
                document.getElementById("EmeraldTokenValue").innerHTML++;
                if (EmeraldTakingToken.innerHTML == 0) {
                    document.getElementById("EmeraldTakingToken").style.opacity = 0;

                }
                TakingTokens.Emerald--;
            }
            return;
        case "TakingSapphire":
            let SapphireTakingToken = document.getElementById("SapphireTakingTokenValue");
            if (SapphireTakingToken.innerHTML > 0) {
                SapphireTakingToken.innerHTML--;
                document.getElementById("SapphireTokenValue").innerHTML++;
                if (SapphireTakingToken.innerHTML == 0) {
                    document.getElementById("SapphireTakingToken").style.opacity = 0;
                }
                TakingTokens.Sapphire--;
            }
            return;

        case "TakingRuby":
            let RubyTakingToken = document.getElementById("RubyTakingTokenValue");
            if (RubyTakingToken.innerHTML > 0) {
                RubyTakingToken.innerHTML--;
                document.getElementById("RubyTokenValue").innerHTML++;
                if (RubyTakingToken.innerHTML == 0) {
                    document.getElementById("RubyTakingToken").style.opacity = 0;
                }
                TakingTokens.Ruby--;
            }
            return;
        case "TakingDiamond":
            let DiamondTakingToken = document.getElementById("DiamondTakingTokenValue");
            if (DiamondTakingToken.innerHTML > 0) {
                DiamondTakingToken.innerHTML--;
                document.getElementById("DiamondTokenValue").innerHTML++;
                if (DiamondTakingToken.innerHTML == 0) {
                    document.getElementById("DiamondTakingToken").style.opacity = 0;
                }
                TakingTokens.Diamond--;
            }
            return;
        case "TakingOnyx":
            let OnyxTakingToken = document.getElementById("OnyxTakingTokenValue");
            if (OnyxTakingToken.innerHTML > 0) {
                OnyxTakingToken.innerHTML--;
                document.getElementById("OnyxTokenValue").innerHTML++;
                if (OnyxTakingToken.innerHTML == 0) {
                    document.getElementById("OnyxTakingToken").style.opacity = 0;
                }
                TakingTokens.Onyx--;
            }
            return;
    }







    if (!ValidateTokens(token)) {
        return;
    }



    switch (token) {
        case "Emerald":
            let EmeraldTokenValue = document.getElementById("EmeraldTokenValue");
            if (EmeraldTokenValue.innerHTML > 0) {
                EmeraldTokenValue.innerHTML--;
                document.getElementById("EmeraldTakingTokenValue").innerHTML++;
                document.getElementById("EmeraldTakingToken").style.opacity = 7.5;
                TakingTokens.Emerald++;
            }
            break;
        
        case "Sapphire":
            let SapphireTokenValue = document.getElementById("SapphireTokenValue");
            if (SapphireTokenValue.innerHTML > 0) {
                SapphireTokenValue.innerHTML--;
                document.getElementById("SapphireTakingTokenValue").innerHTML++;
                document.getElementById("SapphireTakingToken").style.opacity = 7.5;
                TakingTokens.Sapphire++;
            }
            break;
        
        case "Ruby":
            let RubyTokenValue = document.getElementById("RubyTokenValue");
            if (RubyTokenValue.innerHTML > 0) {
                RubyTokenValue.innerHTML--;
                document.getElementById("RubyTakingTokenValue").innerHTML++;
                document.getElementById("RubyTakingToken").style.opacity = 7.5;
                TakingTokens.Ruby++;
            }
            break;
        
        case "Diamond":
            let DiamondTokenValue = document.getElementById("DiamondTokenValue");
            if (DiamondTokenValue.innerHTML > 0) {
                DiamondTokenValue.innerHTML--;
                document.getElementById("DiamondTakingTokenValue").innerHTML++;
                document.getElementById("DiamondTakingToken").style.opacity = 7.5;
                TakingTokens.Diamond++;
            }
            break;
        
        case "Onyx":
            let OnyxTokenValue = document.getElementById("OnyxTokenValue");
            if (OnyxTokenValue.innerHTML > 0) {
                OnyxTokenValue.innerHTML--;
                document.getElementById("OnyxTakingTokenValue").innerHTML++;
                document.getElementById("OnyxTakingToken").style.opacity = 7.5;
                TakingTokens.Onyx++;
            }
            break;
        
        case "Gold":
            alert("GOLD IS NOT IMPLEMENTED");
    }

}