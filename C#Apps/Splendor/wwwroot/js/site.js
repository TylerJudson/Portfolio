let TakingTokens = {};
let Tokens = {
    Emerald: document.getElementById("EmeraldTokenValue").innerHTML,
    Sapphire: document.getElementById("SapphireTokenValue").innerHTML,
    Ruby: document.getElementById("RubyTokenValue").innerHTML,
    Diamond: document.getElementById("DiamondTokenValue").innerHTML,
    Onyx: document.getElementById("OnyxTokenValue").innerHTML,
    Gold: document.getElementById("GoldTokenValue").innerHTML
};



function ToggleScreen(screenID) {
    var x = document.getElementById(screenID);
    if (x.style.display === "none") {
        x.style.display = "block";
    } else {
        x.style.display = "none";
    }
}

function ValidateTokens(token) {

    // Check to make sure we didn't take more than 3 tokens
    if (CountTakingTokens() >= 3) {
        return false;
    }

    // Check to make sure we didn't take more than 2 tokens from one stack
    for (key in TakingTokens) {
        if (TakingTokens[key] >= 2) {
            return false;
        }
    }

    

    // Check to make sure we won't accidently take more than 2 tokens from one stack
    if (CountTakingTokens() >= 2 && TakingTokens[token] > 0) {
        return false;
    }

    // Can only take 3 if greator than 4
    if (TakingTokens[token] > 0 && Tokens[token] < 3) {
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

function renderGameBoard(gameBoard) {
    location.reload();
}

function getReserveButton(IsCurrentPlayer, LessThan3, ImageName) {
    ret = `
            <div class="col p-0 ps-3">
                <div class="p-0 mx-auto" style="width: 100%;">
            <button `;

    if (!IsCurrentPlayer || Tokens.Gold <= 0 || !LessThan3)
            {
                ret += "disabled ";
            }

           
            ret +=    `class="mx-auto btn btn-darkPurple btn-lg" style="width: 100%;" onclick='reserve("` + ImageName + `")'>Reserve</button>
                </div>
             </div >`;
    return ret;
}

function getSelectPurchaseButton(IsCurrentPlayer, purchaseable, HaveGold, ImageName) {
    ret = ``;
    if (IsCurrentPlayer && HaveGold && purchaseable)
    {
        ret += `<div class="col-6 p-0 pe-3">
                    <div class="purple p-0 mx-auto" style="width: 100%;">
                        <button class="mx-auto btn btn-outline-purple btn-lg" style="width: 100%;" onclick='ToggleScreen("CardScreen")'>Select Purchase</button>
                    </div>
                </div>`;
    } 
    return ret;
}

function getPurchaseButton(IsCurrentPlayer, purchaseable, HaveGold, ImageName) {
    ret = `
            <div class="col p-0 `

    if (HaveGold && purchaseable && IsCurrentPlayer) {
        ret += "ps-3";
    }

    ret +=                       `">
                <div class="p-0 mx-auto" style="width: 100%;">
                    <button `;

    if (!IsCurrentPlayer || !purchaseable)
    {
        ret += "disabled ";
    }

    ret +=            `class="mx-auto btn btn-purple btn-lg" style="width: 100%;" onclick='purchase("` + ImageName + `")'>Purchase</button>
                </div>
             </div>
            `;
    return ret;
}

function getSelectPurchaseReserveButton(IsCurrentPlayer, purchaseable, HaveGold, ImageName) {
    ret = ``;
    if (IsCurrentPlayer && HaveGold && purchaseable) {
        ret += `<div class="col-6 p-0 ps-3">
                    <div class="purple p-0 mx-auto" style="width: 100%;">
                        <button class="mx-auto btn btn-outline-purple btn-lg" style="width: 100%;" onclick='ToggleScreen("CardScreen")'>Select Purchase</button>
                    </div>
                </div>`;
    }
    return ret;
}

function getPurchaseReserveButton(IsCurrentPlayer, purchaseable, HaveGold, ImageName) {
    ret = `
            <div class="col p-0 `


    ret += `">
                <div class="p-0 mx-auto" style="width: 100%;">
                    <button `;

    if (!IsCurrentPlayer || !purchaseable) {
        ret += "disabled ";
    }

    ret += `class="mx-auto btn btn-purple btn-lg" style="width: 100%;" onclick='purchase("` + ImageName + `")'>Purchase</button>
                </div>
             </div>
            `;
    return ret;
}

function SetTokens() {
    document.getElementById("EmeraldTakingTokenValue").innerHTML = TakingTokens.Emerald == undefined ? 0 : TakingTokens.Emerald;
    document.getElementById("EmeraldTokenValue").innerHTML = Tokens.Emerald;
    if (TakingTokens.Emerald == 0) {
        document.getElementById("EmeraldTakingToken").style.opacity = 0;
        document.getElementById("EmeraldTakingToken").style.cursor = "default";
        delete TakingTokens.Emerald;
    }

    document.getElementById("SapphireTakingTokenValue").innerHTML = TakingTokens.Sapphire == undefined ? 0 : TakingTokens.Sapphire;
    document.getElementById("SapphireTokenValue").innerHTML = Tokens.Sapphire;
    if (TakingTokens.Sapphire == 0) {
        document.getElementById("SapphireTakingToken").style.opacity = 0;
        document.getElementById("SapphireTakingToken").style.cursor = "default";
        delete TakingTokens.Sapphire;
    }

    document.getElementById("RubyTakingTokenValue").innerHTML = TakingTokens.Ruby == undefined ? 0 : TakingTokens.Ruby;
    document.getElementById("RubyTokenValue").innerHTML = Tokens.Ruby;
    if (TakingTokens.Ruby == 0) {
        document.getElementById("RubyTakingToken").style.opacity = 0;
        document.getElementById("RubyTakingToken").style.cursor = "default";
        delete TakingTokens.Ruby;
    }

    document.getElementById("DiamondTakingTokenValue").innerHTML = TakingTokens.Diamond == undefined ? 0 : TakingTokens.Diamond;
    document.getElementById("DiamondTokenValue").innerHTML = Tokens.Diamond;
    if (TakingTokens.Diamond == 0) {
        document.getElementById("DiamondTakingToken").style.opacity = 0;
        document.getElementById("DiamondTakingToken").style.cursor = "default";
        delete TakingTokens.Diamond;
    }

    document.getElementById("OnyxTakingTokenValue").innerHTML = TakingTokens.Onyx == undefined ? 0 : TakingTokens.Onyx;
    document.getElementById("OnyxTokenValue").innerHTML = Tokens.Onyx;
    if (TakingTokens.Onyx == 0) {
        document.getElementById("OnyxTakingToken").style.opacity = 0;
        document.getElementById("OnyxTakingToken").style.cursor = "default";
        delete TakingTokens.Onyx;
    }

    document.getElementById("GoldTokenValue").innerHTML = Tokens.Gold;
}

function TokenClick(token) {

    // If we click on the taken tokens
    switch (token) {
        case "TakingEmerald":
            if (TakingTokens.Emerald > 0) {
                Tokens.Emerald++;
                TakingTokens.Emerald--;
                SetTokens();
            }
            return;
        case "TakingSapphire":
            if (TakingTokens.Sapphire > 0) {
                Tokens.Sapphire++;
                TakingTokens.Sapphire--;
                SetTokens();
            }
            return;

        case "TakingRuby":
            if (TakingTokens.Ruby > 0) {
                Tokens.Ruby++;
                TakingTokens.Ruby--;
                SetTokens();
            }
            return;
        case "TakingDiamond":
            if (TakingTokens.Diamond > 0) {
                Tokens.Diamond++;
                TakingTokens.Diamond--;
                SetTokens();
            }
            return;
        case "TakingOnyx":
            if (TakingTokens.Onyx > 0) {
                Tokens.Onyx++;
                TakingTokens.Onyx--;
                SetTokens();
            }
            return;
    }

    // Validate before we take
    if (!ValidateTokens(token)) {
       return;
    }


    // If we click on the tokens
    switch (token) {
        case "Emerald":
            if (Tokens.Emerald > 0) {

                Tokens.Emerald--;
                document.getElementById("EmeraldTakingToken").style.opacity = 7.5;
                document.getElementById("EmeraldTakingToken").style.cursor = "pointer";

                if ("Emerald" in TakingTokens) {
                    TakingTokens.Emerald++;
                } else {
                    TakingTokens.Emerald = 1;
                }
            }
            break;
        
        case "Sapphire":
            if (Tokens.Sapphire > 0) {

                Tokens.Sapphire--;
                document.getElementById("SapphireTakingToken").style.opacity = 7.5;
                document.getElementById("SapphireTakingToken").style.cursor = "pointer";

                if ("Sapphire" in TakingTokens) {
                    TakingTokens.Sapphire++;
                } else {
                    TakingTokens.Sapphire = 1;
                }
            }
            break;
        
        case "Ruby":
            if (Tokens.Ruby > 0) {

                Tokens.Ruby--;
                document.getElementById("RubyTakingToken").style.opacity = 7.5;
                document.getElementById("RubyTakingToken").style.cursor = "pointer";

                if ("Ruby" in TakingTokens) {
                    TakingTokens.Ruby++;
                } else {
                    TakingTokens.Ruby = 1;
                }
            }
            break;
        
        case "Diamond":
            if (Tokens.Diamond > 0) {

                Tokens.Diamond--;
                document.getElementById("DiamondTakingToken").style.opacity = 7.5;
                document.getElementById("DiamondTakingToken").style.cursor = "pointer";

                if ("Diamond" in TakingTokens) {
                    TakingTokens.Diamond++;
                } else {
                    TakingTokens.Diamond = 1;
                }
            }
            break;
        
        case "Onyx":
            if (Tokens.Onyx > 0) {

                Tokens.Onyx--;
                document.getElementById("OnyxTakingToken").style.opacity = 7.5;
                document.getElementById("OnyxTakingToken").style.cursor = "pointer";

                if ("Onyx" in TakingTokens) {
                    TakingTokens.Onyx++;
                } else {
                    TakingTokens.Onyx = 1;
                }
            }
            break;

        
    }


    SetTokens();

}


function getNoble(ImageName) {
    alert("GETTING NOBLE");
    GetNobleScreen(ImageName);
}