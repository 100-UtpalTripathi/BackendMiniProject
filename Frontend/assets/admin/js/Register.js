document.getElementById("registerForm").addEventListener("submit", async function (event) {
    event.preventDefault();

    const form = event.target;
    const formData = new FormData(form);

    // Convert form data to JSON object
    const formObject = {};
    formData.forEach((value, key) => {
      if (key === "image") {
        formObject[key] = value.name;
      } else {
        formObject[key] = value;
      }
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
  });