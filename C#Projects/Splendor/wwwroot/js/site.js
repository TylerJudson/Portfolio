
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

// The merged view of PlayerTokens + TakingTokens shown during token return
let DisplayTokens = {};


// Wether or not another screen is visible
let OtherScreen = false;

let ReservedCard = "";

// Template for the taking tokens row (used in Return Tokens screen)
let takingTokensRow = `
    <tr>
        <td id="EmeraldTakingToken" class="takingToken" style="opacity: 0;"><img class="p-0" style="width: 100%" src="https://raw.githubusercontent.com/TylerJudson/Portfolio/main/C%23Projects/Splendor/wwwroot/Images/Emerald.png" alt="EmeraldToken"/><h1 id="EmeraldTakingTokenValue">0</h1></td>
        <td id="SapphireTakingToken" class="takingToken" style="opacity: 0;"><img class="p-0" style="width: 100%" src="https://raw.githubusercontent.com/TylerJudson/Portfolio/main/C%23Projects/Splendor/wwwroot/Images/Sapphire.png" alt="SapphireToken"/><h1 id="SapphireTakingTokenValue">0</h1></td>
        <td id="RubyTakingToken" class="takingToken" style="opacity: 0;"><img class="p-0" style="width: 100%" src="https://raw.githubusercontent.com/TylerJudson/Portfolio/main/C%23Projects/Splendor/wwwroot/Images/Ruby.png" alt="RubyToken"/><h1 id="RubyTakingTokenValue">0</h1></td>
        <td id="DiamondTakingToken" class="takingToken" style="opacity: 0;"><img class="p-0" style="width: 100%" src="https://raw.githubusercontent.com/TylerJudson/Portfolio/main/C%23Projects/Splendor/wwwroot/Images/Diamond.png" alt="DiamondToken"/><h1 id="DiamondTakingTokenValue">0</h1></td>
        <td id="OnyxTakingToken" class="takingToken" style="opacity: 0;"><img class="p-0" style="width: 100%" src="https://raw.githubusercontent.com/TylerJudson/Portfolio/main/C%23Projects/Splendor/wwwroot/Images/Onyx.png" alt="OnyxToken"/><h1 id="OnyxTakingTokenValue">0</h1></td>
        <td id="GoldTakingToken" class="takingToken" style="opacity: 0;"><img class="p-0" style="width: 100%" src="https://raw.githubusercontent.com/TylerJudson/Portfolio/main/C%23Projects/Splendor/wwwroot/Images/Gold.png" alt="GoldToken"/><h1 id="GoldTakingTokenValue">0</h1></td>
    </tr>`;

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
        // Use flex for game-overlay screens to maintain centering, block for others
        x.style.display = x.classList.contains("game-overlay") ? "flex" : "block";
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

function getPurchaseButton(IsCurrentPlayer, purchaseable, HaveGold, ImageName) {
    ret = `
            <div class="col p-0">
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

// Helper to update empty state on a token cell
function updateTokenEmptyState(tokenName) {
    let cell = document.querySelector(`td.token[onclick*="'${tokenName}'"]`);
    if (cell) {
        if (Tokens[tokenName] <= 0) {
            cell.classList.add('token--empty');
        } else {
            cell.classList.remove('token--empty');
        }
    }
}

// Token type abbreviation map for parsing card image names
const TokenTypeMap = { D: 'Diamond', S: 'Sapphire', E: 'Emerald', R: 'Ruby', O: 'Onyx' };

/**
 * Parse card price from image name
 * Format: Level1-D-0P-1S-2E-1R-1O.jpg
 * @param {string} imageName - The card's image name
 * @returns {Object} Price object with token counts
 */
function parseCardPrice(imageName) {
    const prices = { Diamond: 0, Sapphire: 0, Emerald: 0, Ruby: 0, Onyx: 0 };
    // Extract everything after the prestige points marker (e.g., "0P-")
    const priceSection = imageName.replace(/Level\d-[A-Z]-\d+P-?/, '').replace('.jpg', '');
    if (!priceSection) return prices;

    const matches = priceSection.match(/(\d)([DSEOR])/g) || [];
    matches.forEach(match => {
        const count = parseInt(match[0]);
        const type = TokenTypeMap[match[1]];
        if (type) prices[type] = count;
    });
    return prices;
}

/**
 * Check if player can afford a card WITHOUT using gold tokens
 * @param {Object} cardPrice - The card's price { Diamond: 2, Ruby: 3, ... }
 * @param {Object} playerTokens - Player's current tokens
 * @param {Object} playerBonuses - Player's card bonuses
 * @param {Object} additionalTokens - Tokens being taken (TakingTokens)
 * @returns {boolean}
 */
function canAffordCardWithoutGold(cardPrice, playerTokens, playerBonuses, additionalTokens) {
    const tokenTypes = ['Diamond', 'Sapphire', 'Emerald', 'Ruby', 'Onyx'];

    for (const type of tokenTypes) {
        const cost = cardPrice[type] || 0;
        const bonus = playerBonuses[type] || 0;
        const owned = parseInt(playerTokens[type] || 0) + parseInt(additionalTokens[type] || 0);

        // Effective cost after applying card bonuses
        const effectiveCost = Math.max(0, cost - bonus);

        if (owned < effectiveCost) {
            return false;
        }
    }
    return true;
}

/**
 * Check if player can afford a card using gold as wildcards
 * @param {Object} cardPrice - The card's price
 * @param {Object} playerTokens - Player's current tokens
 * @param {Object} playerBonuses - Player's card bonuses
 * @param {Object} additionalTokens - Tokens being taken
 * @returns {boolean}
 */
function canAffordCardWithGold(cardPrice, playerTokens, playerBonuses, additionalTokens) {
    const tokenTypes = ['Diamond', 'Sapphire', 'Emerald', 'Ruby', 'Onyx'];
    let goldNeeded = 0;
    const availableGold = parseInt(playerTokens.Gold || 0) + parseInt(additionalTokens.Gold || 0);

    for (const type of tokenTypes) {
        const cost = cardPrice[type] || 0;
        const bonus = playerBonuses[type] || 0;
        const owned = parseInt(playerTokens[type] || 0) + parseInt(additionalTokens[type] || 0);

        const effectiveCost = Math.max(0, cost - bonus);
        const shortfall = Math.max(0, effectiveCost - owned);
        goldNeeded += shortfall;
    }

    return goldNeeded <= availableGold;
}

/**
 * Determine the highlight class for a card based on affordability
 * Priority: Green (purchasable now) > Purple (purchasable after selection) > Yellow (needs gold) > Gray
 * @param {string} imageName - The card's image name
 * @returns {string} CSS class name
 */
function getCardHighlightClass(imageName) {
    const cardPrice = parseCardPrice(imageName);
    const hasSelectedTokens = CountTokens(TakingTokens) > 0;

    // Use ViewingPlayerTokens (the player viewing the page) instead of PlayerTokens
    // (which contains current turn player's tokens from the DOM)

    // 1. Green: Can buy now without gold
    if (canAffordCardWithoutGold(cardPrice, ViewingPlayerTokens, PlayerCardBonuses, {})) {
        return 'card--purchasable';
    }

    // 2. Purple: Can buy after taking selected tokens (without gold)
    if (hasSelectedTokens && canAffordCardWithoutGold(cardPrice, ViewingPlayerTokens, PlayerCardBonuses, TakingTokens)) {
        return 'card--purchasable-potential';
    }

    // 3. Yellow: Can buy with gold (now OR after taking tokens)
    if (canAffordCardWithGold(cardPrice, ViewingPlayerTokens, PlayerCardBonuses, {})) {
        return 'card--purchasable-gold';
    }
    if (hasSelectedTokens && canAffordCardWithGold(cardPrice, ViewingPlayerTokens, PlayerCardBonuses, TakingTokens)) {
        return 'card--purchasable-gold';
    }

    // 4. Gray: Cannot afford
    return 'card--unavailable';
}

/**
 * Update all card highlights based on current token selection
 */
function updateCardHighlights() {
    // Shop cards - update all
    const shopCards = document.querySelectorAll('.shop-cards-table img.card');

    // Reserved cards - only current player's (ID starts with PlayerId-Reserved:)
    const reservedCards = document.querySelectorAll('.player-reserved-grid img.card');
    const myReservedCards = Array.from(reservedCards).filter(img =>
        img.id && img.id.startsWith(PlayerId + '-Reserved:')
    );

    [...shopCards, ...myReservedCards].forEach(img => {
        // For reserved cards, extract image name from ID (format: PlayerId-Reserved:ImageName)
        const imageName = img.id && img.id.includes('-Reserved:')
            ? img.id.split('-Reserved:')[1]
            : (img.alt || img.id);
        if (!imageName || !imageName.startsWith('Level')) return;

        // Remove all highlight classes
        img.classList.remove('card--purchasable', 'card--purchasable-potential',
                             'card--purchasable-gold', 'card--unavailable');

        // Add the appropriate class
        const highlightClass = getCardHighlightClass(imageName);
        img.classList.add(highlightClass);
    });
}

function SetTokens() {
    document.getElementById("EmeraldTakingTokenValue").innerHTML = TakingTokens.Emerald == undefined ? 0 : TakingTokens.Emerald;
    document.getElementById("EmeraldTokenValue").innerHTML = Tokens.Emerald;
    updateTokenEmptyState('Emerald');
    if (TakingTokens.Emerald == 0) {
        document.getElementById("EmeraldTakingToken").style.opacity = 0;
        document.getElementById("EmeraldTakingToken").style.cursor = "default";
        delete TakingTokens.Emerald;
    }

    document.getElementById("SapphireTakingTokenValue").innerHTML = TakingTokens.Sapphire == undefined ? 0 : TakingTokens.Sapphire;
    document.getElementById("SapphireTokenValue").innerHTML = Tokens.Sapphire;
    updateTokenEmptyState('Sapphire');
    if (TakingTokens.Sapphire == 0) {
        document.getElementById("SapphireTakingToken").style.opacity = 0;
        document.getElementById("SapphireTakingToken").style.cursor = "default";
        delete TakingTokens.Sapphire;
    }

    document.getElementById("RubyTakingTokenValue").innerHTML = TakingTokens.Ruby == undefined ? 0 : TakingTokens.Ruby;
    document.getElementById("RubyTokenValue").innerHTML = Tokens.Ruby;
    updateTokenEmptyState('Ruby');
    if (TakingTokens.Ruby == 0) {
        document.getElementById("RubyTakingToken").style.opacity = 0;
        document.getElementById("RubyTakingToken").style.cursor = "default";
        delete TakingTokens.Ruby;
    }

    document.getElementById("DiamondTakingTokenValue").innerHTML = TakingTokens.Diamond == undefined ? 0 : TakingTokens.Diamond;
    document.getElementById("DiamondTokenValue").innerHTML = Tokens.Diamond;
    updateTokenEmptyState('Diamond');
    if (TakingTokens.Diamond == 0) {
        document.getElementById("DiamondTakingToken").style.opacity = 0;
        document.getElementById("DiamondTakingToken").style.cursor = "default";
        delete TakingTokens.Diamond;
    }

    document.getElementById("OnyxTakingTokenValue").innerHTML = TakingTokens.Onyx == undefined ? 0 : TakingTokens.Onyx;
    document.getElementById("OnyxTokenValue").innerHTML = Tokens.Onyx;
    updateTokenEmptyState('Onyx');
    if (TakingTokens.Onyx == 0) {
        document.getElementById("OnyxTakingToken").style.opacity = 0;
        document.getElementById("OnyxTakingToken").style.cursor = "default";
        delete TakingTokens.Onyx;
    }

    document.getElementById("GoldTokenValue").innerHTML = Tokens.Gold;
    updateTokenEmptyState('Gold');

    // Update card highlights based on new token selection
    updateCardHighlights();
}

// Initialize empty state on page load
function initTokenEmptyStates() {
    ['Emerald', 'Sapphire', 'Ruby', 'Diamond', 'Onyx', 'Gold'].forEach(tokenName => {
        updateTokenEmptyState(tokenName);
    });
}

// Run on DOM ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initTokenEmptyStates);
} else {
    initTokenEmptyStates();
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

// Helper to update empty state on player token cell in return screen
function updatePlayerTokenEmptyState(tokenName, displayValue) {
    let cell = document.getElementById('Player' + tokenName + 'Token');
    if (cell) {
        if (displayValue <= 0) {
            cell.classList.add('token--empty');
        } else {
            cell.classList.remove('token--empty');
        }
    }
}

function SetReturningTokens() {
    document.getElementById("EmeraldReturningTokenValue").innerHTML = ReturningTokens.Emerald == undefined ? 0 : ReturningTokens.Emerald;
    // Show DisplayTokens - ReturningTokens (the tokens player will have after returning)
    let emeraldDisplay = (DisplayTokens.Emerald || 0) - (ReturningTokens.Emerald || 0);
    document.getElementById("PlayerEmeraldTokenValue").innerHTML = emeraldDisplay;
    updatePlayerTokenEmptyState('Emerald', emeraldDisplay);
    if (ReturningTokens.Emerald == 0) {
        document.getElementById("EmeraldReturningToken").style.opacity = 0;
        document.getElementById("EmeraldReturningToken").style.cursor = "default";
        delete ReturningTokens.Emerald;
    }

    document.getElementById("SapphireReturningTokenValue").innerHTML = ReturningTokens.Sapphire == undefined ? 0 : ReturningTokens.Sapphire;
    let sapphireDisplay = (DisplayTokens.Sapphire || 0) - (ReturningTokens.Sapphire || 0);
    document.getElementById("PlayerSapphireTokenValue").innerHTML = sapphireDisplay;
    updatePlayerTokenEmptyState('Sapphire', sapphireDisplay);
    if (ReturningTokens.Sapphire == 0) {
        document.getElementById("SapphireReturningToken").style.opacity = 0;
        document.getElementById("SapphireReturningToken").style.cursor = "default";
        delete ReturningTokens.Sapphire;
    }

    document.getElementById("RubyReturningTokenValue").innerHTML = ReturningTokens.Ruby == undefined ? 0 : ReturningTokens.Ruby;
    let rubyDisplay = (DisplayTokens.Ruby || 0) - (ReturningTokens.Ruby || 0);
    document.getElementById("PlayerRubyTokenValue").innerHTML = rubyDisplay;
    updatePlayerTokenEmptyState('Ruby', rubyDisplay);
    if (ReturningTokens.Ruby == 0) {
        document.getElementById("RubyReturningToken").style.opacity = 0;
        document.getElementById("RubyReturningToken").style.cursor = "default";
        delete ReturningTokens.Ruby;
    }

    document.getElementById("DiamondReturningTokenValue").innerHTML = ReturningTokens.Diamond == undefined ? 0 : ReturningTokens.Diamond;
    let diamondDisplay = (DisplayTokens.Diamond || 0) - (ReturningTokens.Diamond || 0);
    document.getElementById("PlayerDiamondTokenValue").innerHTML = diamondDisplay;
    updatePlayerTokenEmptyState('Diamond', diamondDisplay);
    if (ReturningTokens.Diamond == 0) {
        document.getElementById("DiamondReturningToken").style.opacity = 0;
        document.getElementById("DiamondReturningToken").style.cursor = "default";
        delete ReturningTokens.Diamond;
    }

    document.getElementById("OnyxReturningTokenValue").innerHTML = ReturningTokens.Onyx == undefined ? 0 : ReturningTokens.Onyx;
    let onyxDisplay = (DisplayTokens.Onyx || 0) - (ReturningTokens.Onyx || 0);
    document.getElementById("PlayerOnyxTokenValue").innerHTML = onyxDisplay;
    updatePlayerTokenEmptyState('Onyx', onyxDisplay);
    if (ReturningTokens.Onyx == 0) {
        document.getElementById("OnyxReturningToken").style.opacity = 0;
        document.getElementById("OnyxReturningToken").style.cursor = "default";
        delete ReturningTokens.Onyx;
    }

    document.getElementById("GoldReturningTokenValue").innerHTML = ReturningTokens.Gold == undefined ? 0 : ReturningTokens.Gold;
    let goldDisplay = (DisplayTokens.Gold || 0) - (ReturningTokens.Gold || 0);
    document.getElementById("PlayerGoldTokenValue").innerHTML = goldDisplay;
    updatePlayerTokenEmptyState('Gold', goldDisplay);
    if (ReturningTokens.Gold == 0) {
        document.getElementById("GoldReturningToken").style.opacity = 0;
        document.getElementById("GoldReturningToken").style.cursor = "default";
        delete ReturningTokens.Gold;
    }
}

function ReturnTokenClick(token, tokensToReturn) {
    // If we click on the returning tokens (to un-select them)
    switch (token) {
        case "ReturningEmerald":
            if (ReturningTokens.Emerald > 0) {
                ReturningTokens.Emerald--;
            }
            break;
        case "ReturningSapphire":
            if (ReturningTokens.Sapphire > 0) {
                ReturningTokens.Sapphire--;
            }
            break;
        case "ReturningRuby":
            if (ReturningTokens.Ruby > 0) {
                ReturningTokens.Ruby--;
            }
            break;
        case "ReturningDiamond":
            if (ReturningTokens.Diamond > 0) {
                ReturningTokens.Diamond--;
            }
            break;
        case "ReturningOnyx":
            if (ReturningTokens.Onyx > 0) {
                ReturningTokens.Onyx--;
            }
            break;
        case "ReturningGold":
            if (ReturningTokens.Gold > 0) {
                ReturningTokens.Gold--;
            }
            break;
    }

    // Stop accepting more token selections once we've selected enough to return
    if (CountTokens(ReturningTokens) >= tokensToReturn) {
        SetReturningTokens();
        return;
    }


    // If we click on the player's tokens (to select them for return)
    // Check if there are tokens available to return (DisplayTokens - already selected ReturningTokens)
    let availableEmerald = (DisplayTokens.Emerald || 0) - (ReturningTokens.Emerald || 0);
    let availableSapphire = (DisplayTokens.Sapphire || 0) - (ReturningTokens.Sapphire || 0);
    let availableRuby = (DisplayTokens.Ruby || 0) - (ReturningTokens.Ruby || 0);
    let availableDiamond = (DisplayTokens.Diamond || 0) - (ReturningTokens.Diamond || 0);
    let availableOnyx = (DisplayTokens.Onyx || 0) - (ReturningTokens.Onyx || 0);
    let availableGold = (DisplayTokens.Gold || 0) - (ReturningTokens.Gold || 0);

    switch (token) {
        case "Emerald":
            if (availableEmerald > 0) {
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
            if (availableSapphire > 0) {
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
            if (availableRuby > 0) {
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
            if (availableDiamond > 0) {
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
            if (availableOnyx > 0) {
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
            if (availableGold > 0) {
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

    // Enable return button once we've selected exactly the right number of tokens
    let totalReturned = CountTokens(ReturningTokens);
    let totalAfterReturn = CountTokens(DisplayTokens) - totalReturned;

    if (totalAfterReturn <= 10 && totalReturned >= tokensToReturn) {
        document.getElementById("ReturnButton").disabled = false;
    } else {
        document.getElementById("ReturnButton").disabled = true;
    }
}





