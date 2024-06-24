var jwtDecode = function (jwt) {
  function b64DecodeUnicode(str) {
    return decodeURIComponent(
      atob(str).replace(/(.)/g, function (m, p) {
        var code = p.charCodeAt(0).toString(16).toUpperCase();
        if (code.length < 2) {
          code = "0" + code;
        }
        return "%" + code;
      })
    );
  }

  // Decode base64url encoded string
  function decode(str) {
    var output = str.replace(/-/g, "+").replace(/_/g, "/");
    switch (output.length % 4) {
      case 0:
        break;
      case 2:
        output += "==";
        break;
      case 3:
        output += "=";
        break;
      default:
        throw "Illegal base64url string!";
    }

    try {
      return b64DecodeUnicode(output);
    } catch (err) {
      return atob(output);
    }
  }

  var jwtArray = jwt.split(".");

  return {
    header: JSON.parse(decode(jwtArray[0])),
    payload: JSON.parse(decode(jwtArray[1])),
    signature: jwtArray[2], // The signature is not decoded
  };
};

// Function to check if JWT token is expired
var checkJwtExpiration = function (jwt) {
  var decodedJwt = jwtDecode(jwt);

  var currentTime = Math.floor(Date.now() / 1000); // Current time in seconds
  var expirationTime = decodedJwt.payload.exp; // Expiration time in seconds

  if (expirationTime < currentTime) {
    window.location.href = "/login"; // Redirect to login page if expired
  } else {
    console.log("JWT is still valid");
  }
};

// Login form submission
document
  .getElementById("loginForm")
  .addEventListener("submit", async function (event) {
    event.preventDefault();

    const form = event.target;
    const formData = new FormData(form);

    const loginData = {
      userid: formData.get("userid"),
      password: formData.get("password"),
    };

    try {
      const response = await fetch("http://localhost:5071/api/User/Login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(loginData),
      });

      if (response.ok) {
        const result = await response.json();
        const token = result.token;

        // Store the JWT token in local storage
        localStorage.setItem("jwtToken", token);

        // Decode the token to get user information
        const decodedToken = jwtDecode(token);
        const userRole = decodedToken.payload.role;

        alert(userRole + " logged in successfully");

        // Check token expiration
        checkJwtExpiration(token);

        // Redirect based on role
        if (userRole === "Admin") {
          window.location.href = "./views/admin/dashboard.html";
        } else {
          window.location.href = "./views/customer/dashboard.html";
        }
      } else {
        const error = await response.json();
        alert("Login failed: " + error.message);
      }
    } catch (error) {
      console.error("Error:", error);
      alert("An error occurred. Please try again later.");
    }
  });
