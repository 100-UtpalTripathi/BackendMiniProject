document.querySelector('#myFileInput').addEventListener('change', function() {
    const reader = new FileReader();
    reader.addEventListener('load', () => {
        localStorage.setItem('Image', reader.result);
    });
    reader.readAsDataURL(this.files[0]);
});

document.getElementById('registerForm').addEventListener('submit', function(event) {
    event.preventDefault();
    
    const form = event.target;
    const formData = new FormData(form);
    const email = formData.get('email');
    
    const storedImage = localStorage.getItem('Image');
    if (storedImage) {
        formData.append('Image', storedImage);
    }

    sendRegistrationData(formData);
});

async function sendRegistrationData(formData) {
    const formObject = {
        Name: formData.get('name'),
        DateOfBirth: formData.get('dateOfBirth'),
        Phone: formData.get('phone'),
        Image: formData.get('Image') || '', // Default to empty string if Image is not set
        Role: formData.get('role'),
        Email: formData.get('email'),
        Password: formData.get('password')
    };

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