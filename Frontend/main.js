window.addEventListener("load", main);
function main() {
    document.getElementById("get").onclick = handleGet;
    document.getElementById("post").onclick = handlePost;

    document.getElementById("scannerFile").addEventListener("change", function(event) {
        const file = event.target.files[0];

        if(!file) return;

        const reader = new FileReader();
        reader.onload = function(e) {
            const img = document.getElementById("scannerPreview");
            img.src = e.target.result;
            img.style.display = "block";
            img.style.width = "300px";
        }
        reader.readAsDataURL(file);
    });
}

var container;
var forecastTemplate;
var infoElem;

function handleGet() {
    const promise = fetch("https://stable-well-moccasin.ngrok-free.app/helloworld", {
        headers: {
            "ngrok-skip-browser-warning": "69420"
        }
    });
    promise.then((response) => {
        if (!response.ok) {
            throw new Error(`HTTP error: ${response.status}`);
        }
        return response.text();
    }).then(function (text) {
        handleReceive(text);
    }).catch((error) => {
        alert(error);
    });
}
async function handlePost() {
    var title = document.getElementById("messageTitle").value;
    var content = document.getElementById("messageBody").value;

    var file = document.getElementById("scannerFile").files[0];

    if(!file) return;

    var formData = new FormData();
    formData.append("image", file);

    console.log(file.name, formData);
    const response = await fetch("https://stable-well-moccasin.ngrok-free.app/helloworld", {
        method: "POST",
        headers: {
            "ngrok-skip-browser-warning": "69420"
        },
        body: formData
    });


    await console.log(response);
    const result = await response.json();

    document.getElementById("scanResult").innerText = result.message;

    await console.log(result);

    // fetch("https://stable-well-moccasin.ngrok-free.app/helloworld", {
    //     method: "POST",
    //     headers: {
    //         "Content-Type": "application/json",
    //         "ngrok-skip-browser-warning": "69420"
    //     },
    //     body: JSON.stringify({
    //         "title":title,
    //         "body":content
    //     })
    // }).then((response) => console.log(response));
}

function handleReceive(data) {
    infoElem.innerText = data;
    // console.log(JSON.stringify(data));
    // container.innerHTML = "";

    // for(let i = 0; i < data.length; i++){
    //     var card = forecastTemplate.content.cloneNode(true);
    //     card.querySelector(".date").innerText = data[i].date;
    //     card.querySelector(".temperatureC").innerText = data[i].temperatureC;
    //     card.querySelector(".temperatureF").innerText = data[i].temperatureF;
    //     card.querySelector(".summary").innerText = data[i].summary;
    //     container.appendChild(card);
    // }
}