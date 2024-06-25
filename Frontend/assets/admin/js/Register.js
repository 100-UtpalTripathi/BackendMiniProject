const CLIENT_ID = '685240828887-090u7eolgde4gmbbp9mdshg4rnv4memq.apps.googleusercontent.com';
const API_KEY = 'AIzaSyD2p0a0Lu2TBS8hTdJoC6x2KuQXUYA6Jyg';
const DISCOVERY_DOCS = ["https://www.googleapis.com/discovery/v1/apis/drive/v3/rest"];
const SCOPES = 'https://www.googleapis.com/auth/drive.file';

function handleClientLoad() {
    gapi.load('client:auth2', initClient);
}

function initClient() {
    gapi.client.init({
        apiKey: API_KEY,
        clientId: CLIENT_ID,
        discoveryDocs: DISCOVERY_DOCS,
        scope: SCOPES
    }).then(() => {
        const authInstance = gapi.auth2.getAuthInstance();
        document.getElementById('registerForm').addEventListener('submit', function (event) {
            event.preventDefault();
            authInstance.signIn().then(() => handleFormSubmit(event));
        });
    }).catch(error => {
        console.error("Error initializing Google API client:", error);
    });
}

function handleFormSubmit(event) {
    const form = event.target;
    const formData = new FormData(form);
    const file = formData.get('image');

    if (file && file.size > 0) {
        uploadFileToDrive(file).then(fileData => {
            getShareableLink(fileData.id).then(linkData => {
                formData.append('imageLink', linkData.webViewLink);
                sendRegistrationData(formData);
            });
        }).catch(error => {
            console.error("Error uploading file:", error);
            alert("File upload failed. Please try again.");
        });
    } else {
        sendRegistrationData(formData);
    }
}

function uploadFileToDrive(file) {
    const metadata = {
        name: file.name,
        mimeType: file.type
    };

    const accessToken = gapi.auth.getToken().access_token;
    const form = new FormData();
    form.append('metadata', new Blob([JSON.stringify(metadata)], { type: 'application/json' }));
    form.append('file', file);

    return fetch('https://www.googleapis.com/upload/drive/v3/files?uploadType=multipart', {
        method: 'POST',
        headers: new Headers({ 'Authorization': 'Bearer ' + accessToken }),
        body: form
    }).then(response => response.json());
}

function getShareableLink(fileId) {
    const accessToken = gapi.auth.getToken().access_token;
    return fetch(`https://www.googleapis.com/drive/v3/files/${fileId}/permissions`, {
        method: 'POST',
        headers: new Headers({
            'Authorization': 'Bearer ' + accessToken,
            'Content-Type': 'application/json'
        }),
        body: JSON.stringify({
            role: 'reader',
            type: 'anyone'
        })
    }).then(() => {
        return fetch(`https://www.googleapis.com/drive/v3/files/${fileId}?fields=webViewLink`, {
            headers: new Headers({ 'Authorization': 'Bearer ' + accessToken })
        }).then(response => response.json());
    });
}

async function sendRegistrationData(formData) {
    const formObject = {};
    formData.forEach((value, key) => {
        formObject[key] = value;
    });

    try {
        const response = await fetch("http://localhost:5071/api/User/Register", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(formObject)
        });

        if (response.ok) {
            const result = await response.json();
            alert("Registration successful!");
            window.location.href = "login.html";
        } else {
            const error = await response.json();
            alert("Registration failed: " + error.message);
        }
    } catch (error) {
        console.error("Error:", error);
        alert("An error occurred. Please try again later.");
    }
}

handleClientLoad();
