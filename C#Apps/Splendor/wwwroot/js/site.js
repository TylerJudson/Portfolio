
// The tokens the player is trying to take on a given turn
let TakingTokens = {};

// The tokens available for a player to take on a give turn
let Tokens = {
    Emerald:  document.getElementById("EmeraldTokenValue").innerHTML,
    Sapphire: document.getElementById("SapphireTokenValue").innerHTML,
    Ruby:     document.getElementById("RubyTokenValue").innerHTML,
    Diamond:  document.getElementById("DiamondTokenValue").innerHTML,
    Onyx:     document.getElementById("OnyxTokenValue").innerHTML,
    Gold:     document.getElementById("GoldTokenValue").innerHTML
};


// The tokens the player is trying to return on a given turn
let ReturningTokens = {};

// THe tokens available for a player to return
let PlayerTokens = {
    Emerald:  document.getElementById("PlayerEmeraldTokenValue").innerHTML,
    Sapphire: document.getElementById("PlayerSapphireTokenValue").innerHTML,
    Ruby:     document.getElementById("PlayerRubyTokenValue").innerHTML,
    Diamond:  document.getElementById("PlayerDiamondTokenValue").innerHTML,
    Onyx:     document.getElementById("PlayerOnyxTokenValue").innerHTML,
    Gold:     document.getElementById("PlayerGoldTokenValue").innerHTML
}


// Wether or not another screen is visible
let OtherScreen = false;

let ReservedCard = "";

// When the user scrolls the page
window.onscroll = function () { OnScroll() };


// Get the user's card
var card = document.getElementById("0-card");

// Get the offset position of the card
var sticky = card.offsetTop;

// Add the sticky class to the player's card when you reach its scroll position. Remove "sticky" when you leave the scroll position
function OnScroll() {
    // When you reach the position of the card
    if (window.pageYOffset > sticky && window.innerWidth < 992 && !OtherScreen) {
        card.classList.add("sticky");
        document.body.style.paddingTop = (card.offsetHeight + 16).toString() + "px"; // add the neccessary padding to the screen to make scrolling smooth
    } else {
        card.classList.remove("sticky");
        document.body.style.paddingTop = "0rem"; // remove that padding

    }
}

// Toggles a screen
function ToggleScreen(screenID) {
    var x = document.getElementById(screenID);
    if (x.style.display == "none") {
        x.style.display = "block";
        OtherScreen = true;
    } else {
        x.style.display = "none";
        OtherScreen = false;
    }
    OnScroll();
    
}

function merge(obj1, obj2) {
    ret = {};

    // Loop through all the properties in the first object and add them to ret
    for (prop in obj1) {
        ret[prop] = obj1[prop];
    }


    // Loop through all the properties in the second object and add them to ret
    for (prop in obj2) {
        // if the property already exists in ret sum the two
        if (ret.hasOwnProperty(prop)) {
            ret[prop] += obj2[prop];
        } else {
            ret[prop] = obj2[prop];
        }
    }

    return ret;
}
function ValidateTokens(token) {

    // Check to make sure we didn't take more than 3 tokens
    if (CountTokens(TakingTokens) >= 3) {
        return false;
    }

    // Check to make sure we didn't take more than 2 tokens from one stack
    for (key in TakingTokens) {
        if (TakingTokens[key] >= 2) {
            return false;
        }
    }

    

    // Check to make sure we won't accidently take more than 2 tokens from one stack
    if (CountTokens(TakingTokens) >= 2 && TakingTokens[token] > 0) {
        return false;
    }

    // Can only take 3 if greator than 4
    if (TakingTokens[token] > 0 && Tokens[token] < 3) {
        return false;
    }

    return true;
}

function CountTokens(obj) {
    let sum = 0;
    for (el in obj) {
        if (obj.hasOwnProperty(el)) {
            sum += parseInt(obj[el]);
        }
    }
    return sum;
}

function renderGameBoard() {
    location.reload();

}

function getReserveButton(IsCurrentPlayer, LessThan3, ImageName) {
    ret = `
            <div class="col p-0 ps-3">
                <div class="p-0 mx-auto" style="width: 100%;">
            <button `;

    if (!IsCurrentPlayer || !LessThan3 || IsChoosingNobles)
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
    if (IsCurrentPlayer && HaveGold && purchaseable && !IsChoosingNobles)
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

    if (!IsCurrentPlayer || !purchaseable || IsChoosingNobles)
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
    if (IsCurrentPlayer && HaveGold && purchaseable && !IsChoosingNobles) {
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

    if (!IsCurrentPlayer || !purchaseable || IsChoosingNobles) {
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

    if (IsChoosingNobles) {
        return;
    }

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

function SetReturningTokens() {
    document.getElementById("EmeraldReturningTokenValue").innerHTML = ReturningTokens.Emerald == undefined ? 0 : ReturningTokens.Emerald;
    document.getElementById("PlayerEmeraldTokenValue").innerHTML = PlayerTokens.Emerald;
    if (ReturningTokens.Emerald == 0) {
        document.getElementById("EmeraldReturningToken").style.opacity = 0;
        document.getElementById("EmeraldReturningToken").style.cursor = "default";
        delete ReturningTokens.Emerald;
    }

    document.getElementById("SapphireReturningTokenValue").innerHTML = ReturningTokens.Sapphire == undefined ? 0 : ReturningTokens.Sapphire;
    document.getElementById("PlayerSapphireTokenValue").innerHTML = PlayerTokens.Sapphire;
    if (ReturningTokens.Sapphire == 0) {
        document.getElementById("SapphireReturningToken").style.opacity = 0;
        document.getElementById("SapphireReturningToken").style.cursor = "default";
        delete ReturningTokens.Sapphire;
    }

    document.getElementById("RubyReturningTokenValue").innerHTML = ReturningTokens.Ruby == undefined ? 0 : ReturningTokens.Ruby;
    document.getElementById("PlayerRubyTokenValue").innerHTML = PlayerTokens.Ruby;
    if (ReturningTokens.Ruby == 0) {
        document.getElementById("RubyReturningToken").style.opacity = 0;
        document.getElementById("RubyReturningToken").style.cursor = "default";
        delete ReturningTokens.Ruby;
    }

    document.getElementById("DiamondReturningTokenValue").innerHTML = ReturningTokens.Diamond == undefined ? 0 : ReturningTokens.Diamond;
    document.getElementById("PlayerDiamondTokenValue").innerHTML = PlayerTokens.Diamond;
    if (ReturningTokens.Diamond == 0) {
        document.getElementById("DiamondReturningToken").style.opacity = 0;
        document.getElementById("DiamondReturningToken").style.cursor = "default";
        delete ReturningTokens.Diamond;
    }

    document.getElementById("OnyxReturningTokenValue").innerHTML = ReturningTokens.Onyx == undefined ? 0 : ReturningTokens.Onyx;
    document.getElementById("PlayerOnyxTokenValue").innerHTML = PlayerTokens.Onyx;
    if (ReturningTokens.Onyx == 0) {
        document.getElementById("OnyxReturningToken").style.opacity = 0;
        document.getElementById("OnyxReturningToken").style.cursor = "default";
        delete ReturningTokens.Onyx;
    }

    document.getElementById("GoldReturningTokenValue").innerHTML = ReturningTokens.Gold == undefined ? 0 : ReturningTokens.Gold;
    document.getElementById("PlayerGoldTokenValue").innerHTML = PlayerTokens.Gold;
    if (ReturningTokens.Gold == 0) {
        document.getElementById("GoldReturningToken").style.opacity = 0;
        document.getElementById("GoldReturningToken").style.cursor = "default";
        delete ReturningTokens.Gold;
    }
}

function ReturnTokenClick(token, tokensToReturn) {

    // If we click on the returning tokens
    switch (token) {
        case "ReturningEmerald":
            if (ReturningTokens.Emerald > 0) {
                PlayerTokens.Emerald++;
                ReturningTokens.Emerald--;
            }
            break;
        case "ReturningSapphire":
            if (ReturningTokens.Sapphire > 0) {
                PlayerTokens.Sapphire++;
                ReturningTokens.Sapphire--;
            }
            break;
        case "ReturningRuby":
            if (ReturningTokens.Ruby > 0) {
                PlayerTokens.Ruby++;
                ReturningTokens.Ruby--;
            }
            break;
        case "ReturningDiamond":
            if (ReturningTokens.Diamond > 0) {
                PlayerTokens.Diamond++;
                ReturningTokens.Diamond--;
            }
            break;
        case "ReturningOnyx":
            if (ReturningTokens.Onyx > 0) {
                PlayerTokens.Onyx++;
                ReturningTokens.Onyx--;
            }
            break;
        case "ReturningGold":
            if (ReturningTokens.Gold > 0) {
                PlayerTokens.Gold++;
                ReturningTokens.Gold--;
            }
            break;
    }

    if (tokensToReturn <= CountTokens(ReturningTokens)) {
        return;
    }


    // If we click on the tokens
    switch (token) {
        case "Emerald":
            if (PlayerTokens.Emerald > 0) {

                PlayerTokens.Emerald--;
                document.getElementById("EmeraldReturningToken").style.opacity = 7.5;
                document.getElementById("EmeraldReturningToken").style.cursor = "pointer";

                if ("Emerald" in ReturningTokens) {
                    ReturningTokens.Emerald++;
                } else {
                    ReturningTokens.Emerald = 1;
                }
            }
            break;

        case "Sapphire":
            if (PlayerTokens.Sapphire > 0) {

                PlayerTokens.Sapphire--;
                document.getElementById("SapphireReturningToken").style.opacity = 7.5;
                document.getElementById("SapphireReturningToken").style.cursor = "pointer";

                if ("Sapphire" in ReturningTokens) {
                    ReturningTokens.Sapphire++;
                } else {
                    ReturningTokens.Sapphire = 1;
                }
            }
            break;

        case "Ruby":
            if (PlayerTokens.Ruby > 0) {

                PlayerTokens.Ruby--;
                document.getElementById("RubyReturningToken").style.opacity = 7.5;
                document.getElementById("RubyReturningToken").style.cursor = "pointer";

                if ("Ruby" in ReturningTokens) {
                    ReturningTokens.Ruby++;
                } else {
                    ReturningTokens.Ruby = 1;
                }
            }
            break;

        case "Diamond":
            if (PlayerTokens.Diamond > 0) {

                PlayerTokens.Diamond--;
                document.getElementById("DiamondReturningToken").style.opacity = 7.5;
                document.getElementById("DiamondReturningToken").style.cursor = "pointer";

                if ("Diamond" in ReturningTokens) {
                    ReturningTokens.Diamond++;
                } else {
                    ReturningTokens.Diamond = 1;
                }
            }
            break;

        case "Onyx":
            if (PlayerTokens.Onyx > 0) {

                PlayerTokens.Onyx--;
                document.getElementById("OnyxReturningToken").style.opacity = 7.5;
                document.getElementById("OnyxReturningToken").style.cursor = "pointer";

                if ("Onyx" in ReturningTokens) {
                    ReturningTokens.Onyx++;
                } else {
                    ReturningTokens.Onyx = 1;
                }
            }
            break;
        case "Gold":
            if (PlayerTokens.Gold > 0) {

                PlayerTokens.Gold--;
                document.getElementById("GoldReturningToken").style.opacity = 7.5;
                document.getElementById("GoldReturningToken").style.cursor = "pointer";

                if ("Gold" in ReturningTokens) {
                    ReturningTokens.Gold++;
                } else {
                    ReturningTokens.Gold = 1;
                }
            }
            break;


    }


    SetReturningTokens();


    if (CountTokens(PlayerTokens) + CountTokens(TakingTokens) <= 10) {
        document.getElementById("ReturnButton").disabled = false;
    } else {
        document.getElementById("ReturnButton").disabled = true;
    }
}
