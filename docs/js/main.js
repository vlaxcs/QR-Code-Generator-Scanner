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
    if (fileUploader){
        fileUploader.addEventListener("change", function(event) {
            handleGet();
            innerMessage = document.getElementsByClassName("message")[0];
            innerMessage.style.color = "white";
            innerMessage.innerHTML = "N/A";
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

    const scannerSend = document.getElementsByClassName("scanButton")[0];
    if (scannerSend){
        scannerSend.addEventListener("click", handlePost);
    }

    const generatorSend = document.getElementsByClassName("encodeButton")[0];
    if (generatorSend){
        generatorSend.addEventListener("click", handleGetGenerator);
        generatorSend.addEventListener("click", handlePostGenerator);
    }
}

var container;
var forecastTemplate;
var infoElem;

function handleGet() {
    const promise = fetch("https://stable-well-moccasin.ngrok-free.app/scanner", {
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

function handleGetGenerator() {
    const promise = fetch("https://stable-well-moccasin.ngrok-free.app/generator", {
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
    var file = document.getElementById("scannerFile").files[0];
    if(!file) return;

    var formData = new FormData();
    formData.append("image", file);

    console.log(file.name, formData);
    const response = await fetch("https://stable-well-moccasin.ngrok-free.app/scanner", {
        method: "POST",
        headers: {
            "ngrok-skip-browser-warning": "69420"
        },
        body: formData
    });

    console.log(response);
    const result = await response.json();

    innerMessage = document.getElementsByClassName("message")[0];
    if (result.message[0] == "-" && result.message[1] == "1") {
        innerMessage.style.color = "red";
        innerMessage.innerHTML = "An erorr has occured";
    } else {
        const innerVersion = document.getElementsByClassName("version")[0];
        innerVersion.innerHTML = result.version;
        const innerECC = document.getElementsByClassName("ecc-level")[0];
        innerECC.innerHTML = result.eccLevel;
        innerMessage.style.color = "lightgreen";
        innerMessage.innerHTML = result.message;
    }
}

function handleReceive(data) {
    uploaded = document.getElementsByClassName("uploaded")[0];
    uploaded.innerHTML = data;
}

async function handlePostGenerator(){

    userEccLevel = document.getElementById("errorCorrectionLevel").value;
    userVersion = document.getElementById("version").value;
    userMessage = document.getElementById("message").value;
    if (!userEccLevel || !userVersion || !userMessage){
        alert("Complete all fields!")
    }

    var aux;
    switch (userEccLevel){
        case "L": aux = 0; break;
        case "M": aux = 1; break;
        case "Q": aux = 2; break;
        case "M": aux = 3; break;
    }

    const requestData = { 
        message: userMessage,
        minVersion: parseInt(userVersion),
        eccLevel: aux,
    };

    console.log(JSON.stringify(requestData));

    const response = await fetch("https://stable-well-moccasin.ngrok-free.app/generator", {
        method: "POST",
        headers: {
            "ngrok-skip-browser-warning": "69420",
            "Content-Type": "application/json"
        },
        body: JSON.stringify(requestData)
    });

    const blob = await response.blob();
    const imageUrl = URL.createObjectURL(blob);
    const qrCode = document.getElementById("generated");
    qrCode.src = imageUrl;

    const version = document.getElementsByClassName("version")[0];
    version.innerHTML = response.headers.get("version");

    const eccLevel = document.getElementsByClassName("ecc-level")[0];
    eccLevel.innerHTML = response.headers.get("eccLevel") + " (" + userEccLevel + ")";
}