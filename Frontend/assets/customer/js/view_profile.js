document.addEventListener("DOMContentLoaded", function () {
  const profileForm = document.getElementById("profileForm");
  const editProfileForm = document.getElementById("editProfileForm");

  const nameInput = document.getElementById("name");
  const dobInput = document.getElementById("dob");
  const phoneInput = document.getElementById("phone");
  const emailInput = document.getElementById("email");
  const roleInput = document.getElementById("role");
  const imageInput = document.getElementById("image");
  const profileImage = document.getElementById("profileImage");

  const editNameInput = document.getElementById("editName");
  const editDobInput = document.getElementById("editDob");
  const editPhoneInput = document.getElementById("editPhone");
  const editEmailInput = document.getElementById("editEmail");
  const editRoleInput = document.getElementById("editRole");
  //const editImageInput = document.getElementById("editImage");
  const editPasswordInput = document.getElementById("editPassword");

  const saveChangesButton = document.getElementById("saveChanges");

  const token = localStorage.getItem("jwtToken");

  if (!token) {
    alert("User is not authenticated");
    return;
  }

  fetch("http://localhost:5071/api/Customers/profile/view", {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error("Failed to fetch profile");
      }
      return response.json();
    })
    .then((data) => {
      const dateOfBirth = data.dateOfBirth.split("T")[0]; // Extract only the date part

      nameInput.value = data.name;
      dobInput.value = dateOfBirth;
      phoneInput.value = data.phone;
      emailInput.value = data.email;
      roleInput.value = data.role;

      // Access the stored image from localStorage
      const storedImage = localStorage.getItem("Image");
      if (storedImage) {
        profileImage.src = storedImage;
      } else {
        profileImage.src = data.image;
      }

      editNameInput.value = data.name;
      editDobInput.value = dateOfBirth;
      editPhoneInput.value = data.phone;
      editEmailInput.value = data.email;
      editRoleInput.value = data.role;
      editImageInput.value = "";
    })
    .catch((error) => {
      console.error(error);
      alert("An error occurred while fetching the profile.");
    });

  editImageInput.addEventListener("change", function () {
    const reader = new FileReader();
    reader.addEventListener("load", () => {
      localStorage.setItem('Image', reader.result);
    });
    reader.readAsDataURL(this.files[0]);
  });

  saveChangesButton.addEventListener("click", function () {
    const storedImage = localStorage.getItem("Image");
    const updatedProfile = {
      name: editNameInput.value,
      dateOfBirth: editDobInput.value,
      phone: editPhoneInput.value,
      email: editEmailInput.value,
      role: editRoleInput.value,
      image: storedImage || "", // Get image from localStorage if available
      password: editPasswordInput.value,
    };

    updateProfile(updatedProfile);
  });

  async function updateProfile(updatedProfile) {
    const formData = new FormData();

    // Append all form fields to FormData object
    for (const key in updatedProfile) {
      formData.append(key, updatedProfile[key]);
    }

    try {
      const response = await fetch("http://localhost:5071/api/Customers/profile/edit", {
        method: "PUT",
        headers: {
          Authorization: `Bearer ${token}`,
        },
        body: formData,
      });

      if (!response.ok) {
        throw new Error("Failed to update profile");
      }

      const data = await response.json();
      const dateOfBirth = data.dateOfBirth.split("T")[0]; // Extract only the date part

      nameInput.value = data.name;
      dobInput.value = dateOfBirth;
      phoneInput.value = data.phone;
      emailInput.value = data.email;
      roleInput.value = data.role;

      // Access the stored image from localStorage
      const storedImage = localStorage.getItem("Image");
      if (storedImage) {
        profileImage.src = storedImage;
      } else {
        profileImage.src = data.image;
      }

      alert("Profile updated successfully");
      const editModal = new bootstrap.Modal(document.getElementById("editModal"));
      editModal.hide();
    } catch (error) {
      console.error(error);
      alert("An error occurred while updating the profile.");
    }
  }
});



// validation functions
var validateName = () => {
  const name = document.getElementById("editName");
  const nameError = document.getElementById("editNameError");
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
  const phone = document.getElementById("editPhone");
  const phoneError = document.getElementById("editPhoneError");
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
  const email = document.getElementById("editEmail");
  const emailError = document.getElementById("editEmailError");
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
  const password = document.getElementById("editPassword");
  const passwordError = document.getElementById("editPasswordError");
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
