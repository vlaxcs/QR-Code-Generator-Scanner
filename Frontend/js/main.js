function updateVersion(version){
    const spanVersion = document.getElementById("versionValue");
    spanVersion.innerHTML = version.value;
}
function pancaker(field){
    var i = 20, cnt = 0.2;
    setInterval(() => {
        i += cnt;
        if (i > 50.0) {
            cnt = -0.7;
        }
        else if (i < 20){
            cnt = 0.2;
        }
        field.style.backgroundSize = i + "px";
    }, 25);

}
window.onload = async function(){
    // document.getElementById("get").onclick = handleGet;
    // document.getElementById("post").onclick = handlePost;
    const preview = document.getElementById("preview");
    const result = document.getElementById("generated");
    if (preview){
       pancaker(preview);
    }
    else if (result){
        pancaker(result);
    }
    const version = document.getElementById("version");
    if (version) {
        updateVersion(version);
        version.addEventListener("input", function(event) {
            updateVersion(version);
        });
    }

    const fileUploader = document.getElementById("scannerFile");
    fileUploader.addEventListener("change", function(event) {
        const file = event.target.files[0];
        if(!file) {
            preview.src = "none";
            preview.visibility = "hidden";
            return;
        } else {
            const reader = new FileReader();
            reader.onload = function(e) {
                console.log(e.target.result);
                preview.src = e.target.result;
            };
    
            reader.readAsDataURL(file);
        }
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