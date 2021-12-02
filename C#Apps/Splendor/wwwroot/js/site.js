// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


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
        <img class="mx-auto card" style="margin-top: 15rem;" src="Images/` + ImageName + `" width="50%"/>
        <div class="container">
            <div class="mx-auto col purple" style="width: 150px;">
                <button class="mx-auto btn btn-outline-dark btn-lg" style="width: 150px;" onclick='ToggleScreen("CardScreen")'>Back</button>
            </div>
            <div class="mx-auto col" style="width: 150px;">
                <button class="mx-auto btn btn-purple btn-lg" style="width: 150px;" onclick='ToggleScreen("CardScreen")'>Purchase</button>
            </div>
        </div>
    `;

    let CardScreen = document.getElementById("CardScreen")

    CardScreen.innerHTML = innerHtml;
    CardScreen.style.display = "block";
}