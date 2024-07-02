document.addEventListener("DOMContentLoaded", function () {
    const profileForm = document.getElementById("profileForm");
    const editProfileForm = document.getElementById("editProfileForm");
  
    const nameInput = document.getElementById("name");
    const dobInput = document.getElementById("dob");
    const phoneInput = document.getElementById("phone");
    const emailInput = document.getElementById("email");
    const roleInput = document.getElementById("role");
    const imageInput = document.getElementById("image");
  
    const editNameInput = document.getElementById("editName");
    const editDobInput = document.getElementById("editDob");
    const editPhoneInput = document.getElementById("editPhone");
    const editEmailInput = document.getElementById("editEmail");
    const editRoleInput = document.getElementById("editRole");
    const editImageInput = document.getElementById("editImage");
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
        Authorization: `Bearer ${token}`
      }
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
        imageInput.value = data.image;
  
        editNameInput.value = data.name;
        editDobInput.value = dateOfBirth;
        editPhoneInput.value = data.phone;
        editEmailInput.value = data.email;
        editRoleInput.value = data.role;
        editImageInput.value = data.image;
      })
      .catch((error) => {
        console.error(error);
        alert("An error occurred while fetching the profile.");
      });
  
    saveChangesButton.addEventListener("click", function () {
      const updatedProfile = {
        name: editNameInput.value,
        dateOfBirth: editDobInput.value,
        phone: editPhoneInput.value,
        email: editEmailInput.value,
        role: editRoleInput.value,
        image: editImageInput.value,
        password: editPasswordInput.value
      };
  
      fetch("http://localhost:5071/api/Customers/profile/edit", {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify(updatedProfile)
      })
        .then((response) => {
          if (!response.ok) {
            throw new Error("Failed to update profile");
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
          imageInput.value = data.image;
  
          alert("Profile updated successfully");
          const editModal = bootstrap.Modal.getInstance(document.getElementById('editModal'));
          editModal.hide();
        })
        .catch((error) => {
          console.error(error);
          alert("An error occurred while updating the profile.");
        });
    });
  });
  