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
    alert();
    alert(ImageName);
    alert(purchaseable);
    return `
            <section id="CardScreen" class="OverlayScreen">
                    <div class="mx-auto purple" style="width: 150px;">
                        <button class="mx-auto btn btn-outline-dark btn-lg" style="width: 150px;" onclick="CardScreen()">Back</button>
                    </div>
            </section >
    `;
}