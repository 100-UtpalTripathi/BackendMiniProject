// Validation functions for the registration form

// validation functions
var validateName = () => {
  const name = document.getElementById("name");
  const nameError = document.getElementById("nameError");
  const regex = /^[a-zA-Z\s]+$/;
  if (name.value.trim() === "") {
    name.classList.add("error");
    name.classList.remove("correct");
    nameError.textContent = "Name is required.";
  } else if (!regex.test(name.value)) {
    name.classList.add("error");
    name.classList.remove("correct");
    nameError.textContent = "Name must contain only letters and spaces.";
  } else {
    name.classList.remove("error");
    name.classList.add("correct");
    nameError.textContent = "";
  }
};

function validatePhone() {
  const phone = document.getElementById("phone");
  const phoneError = document.getElementById("phoneError");
  const regex = /^\d{10}$/;
  if (phone.value.trim() === "") {
    phone.classList.add("error");
    phone.classList.remove("correct");
    phoneError.textContent = "Phone number is required.";
  } else if (!regex.test(phone.value)) {
    phone.classList.add("error");
    phone.classList.remove("correct");
    phoneError.textContent = "Phone number must be 10 digits.";
  } else {
    phone.classList.remove("error");
    phone.classList.add("correct");
    phoneError.textContent = "";
  }
}

function validateEmail() {
  const email = document.getElementById("email");
  const emailError = document.getElementById("emailError");
  const regex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;
  if (email.value.trim() === "") {
    email.classList.add("error");
    email.classList.remove("correct");
    emailError.textContent = "Email is required.";
  } else if (!regex.test(email.value)) {
    email.classList.add("error");
    email.classList.remove("correct");
    emailError.textContent = "Email is not valid.";
  } else {
    email.classList.remove("error");
    email.classList.add("correct");
    emailError.textContent = "";
  }
}

function validatePassword() {
  const password = document.getElementById("password");
  const passwordError = document.getElementById("passwordError");
  const regex = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$/;
  if (password.value.trim() === "") {
    password.classList.add("error");
    password.classList.remove("correct");
    passwordError.textContent = "Password is required.";
  } else if (!regex.test(password.value)) {
    password.classList.add("error");
    password.classList.remove("correct");
    passwordError.textContent =
      "Password must be 6-20 characters with at least one digit, one lowercase letter, and one uppercase letter.";
  } else {
    password.classList.remove("error");
    password.classList.add("correct");
    passwordError.textContent = "";
  }
}

document.querySelector("#myFileInput").addEventListener("change", function () {
  const reader = new FileReader();
  reader.addEventListener("load", () => {
    localStorage.setItem("Image", reader.result);
  });
  reader.readAsDataURL(this.files[0]);
});

document
  .getElementById("registerForm")
  .addEventListener("submit", function (event) {
    event.preventDefault();

    const form = event.target;
    const formData = new FormData(form);
    const email = formData.get("email");

    const storedImage = localStorage.getItem("Image");
    if (storedImage) {
      formData.append("Image", storedImage);
    }

    sendRegistrationData(formData);
  });

async function sendRegistrationData(formData) {
  const formObject = {
    Name: formData.get("name"),
    DateOfBirth: formData.get("dateOfBirth"),
    Phone: formData.get("phone"),
    Image: formData.get("Image") || "", // Default to empty string if Image is not set
    Role: formData.get("role"),
    Email: formData.get("email"),
    Password: formData.get("password"),
  };

  try {
    const response = await fetch("http://localhost:5071/api/User/Register", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(formObject),
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

