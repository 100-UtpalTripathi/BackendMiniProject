document.addEventListener('DOMContentLoaded', function () {
    const citiesContainer = document.getElementById('citiesContainer');
    const addCityForm = document.getElementById('addCityForm');
    const editCityForm = document.getElementById('editCityForm');
    const searchNameInput = document.getElementById('searchName');
    const searchStateInput = document.getElementById('searchState');
    const searchCountryInput = document.getElementById('searchCountry');

    const token = localStorage.getItem("jwtToken");

    // Fetch cities from the server
    const fetchCities = () => {
        fetch('http://localhost:5071/api/Cities/all', {
            headers: {
                'Authorization': `Bearer ${token}`,
            },
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch cities.');
            }
            return response.json();
        })
        .then(cities => {
            displayCities(cities);
        })
        .catch(error => {
            console.error('Error fetching cities:', error.message);
            alert('Error fetching cities. Please try again later.');
        });
    };

    // Display cities on the page
    const displayCities = (cities) => {
        citiesContainer.innerHTML = '';
        cities.forEach(city => {
            const card = document.createElement('div');
            card.classList.add('col-md-4');
            card.innerHTML = 
                `<div class="card">
                    <div class="card-body">
                        <h5 class="card-title">${city.name}</h5>
                        <p class="card-text">State: ${city.state}</p>
                        <p class="card-text">Country: ${city.country}</p>
                        <p class="card-text">Pincode: ${city.pincode}</p>
                        <button class="btn btn-edit" data-bs-toggle="modal" data-bs-target="#editCityModal" 
                            data-id="${city.id}"
                            data-name="${city.name}"
                            data-state="${city.state}"
                            data-country="${city.country}"
                            data-pincode="${city.pincode}">Edit</button>
                        <button class="btn btn-delete" data-id="${city.id}">Delete</button>
                    </div>
                </div>`;
            citiesContainer.appendChild(card);
        });

        // Attach event listeners for edit and delete buttons
        document.querySelectorAll('.btn-edit').forEach(button => {
            button.addEventListener('click', (event) => {
                const cityId = event.target.getAttribute('data-id');
                const name = event.target.getAttribute('data-name');
                const state = event.target.getAttribute('data-state');
                const country = event.target.getAttribute('data-country');
                const pincode = event.target.getAttribute('data-pincode');
                
                document.getElementById('editCityId').value = cityId;
                document.getElementById('editName').value = name;
                document.getElementById('editState').value = state;
                document.getElementById('editCountry').value = country;
                document.getElementById('editPincode').value = pincode;
            });
        });

        document.querySelectorAll('.btn-delete').forEach(button => {
            button.addEventListener('click', (event) => {
                const cityId = event.target.getAttribute('data-id');
                deleteCity(cityId);
            });
        });
    };

    // Add a new city
    addCityForm.addEventListener('submit', function (event) {
        event.preventDefault();
        const formData = new FormData(addCityForm);
        const data = Object.fromEntries(formData.entries());

        fetch('http://localhost:5071/api/Cities', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
            body: JSON.stringify(data)
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to add city.');
            }
            return response.json();
        })
        .then(() => {
            addCityForm.reset();
            fetchCities();
        })
        .catch(error => {
            console.error('Error adding city:', error.message);
            alert('Error adding city. Please try again later.');
        });
    });

    // Edit an existing city
    editCityForm.addEventListener('submit', function (event) {
        event.preventDefault();
        const formData = new FormData(editCityForm);
        const data = Object.fromEntries(formData.entries());
        const cityId = document.getElementById('editCityId').value;

        fetch(`http://localhost:5071/api/Cities/${cityId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
            body: JSON.stringify(data)
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to edit city.');
            }
            return response.json();
        })
        .then(() => {
            fetchCities();
        })
        .catch(error => {
            console.error('Error editing city:', error.message);
            alert('Error editing city. Please try again later.');
        });
    });

    // Delete a city
    const deleteCity = (cityId) => {
        fetch(`http://localhost:5071/api/Cities/${cityId}`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${token}`,
            }
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to delete city.');
            }
            fetchCities();
        })
        .catch(error => {
            console.error('Error deleting city:', error.message);
            alert('Error deleting city. Please try again later.');
        });
    };

    // Search functionality
    const searchCities = () => {
        const searchName = searchNameInput.value.toLowerCase();
        const searchState = searchStateInput.value.toLowerCase();
        const searchCountry = searchCountryInput.value.toLowerCase();

        fetch('http://localhost:5071/api/Cities/all', {
            headers: {
                'Authorization': `Bearer ${token}`,
            },
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch cities.');
            }
            return response.json();
        })
        .then(cities => {
            const filteredCities = cities.filter(city => {
                return (
                    city.name.toLowerCase().includes(searchName) &&
                    city.state.toLowerCase().includes(searchState) &&
                    city.country.toLowerCase().includes(searchCountry)
                );
            });
            displayCities(filteredCities);
        })
        .catch(error => {
            console.error('Error fetching cities:', error.message);
            alert('Error fetching cities. Please try again later.');
        });
    };

    // Attach event listeners for search inputs
    searchNameInput.addEventListener('input', searchCities);
    searchStateInput.addEventListener('input', searchCities);
    searchCountryInput.addEventListener('input', searchCities);

    // Initial fetch of cities
    fetchCities();
});


// function to validate the form
// Validation functions
function validateCityName() {
    const name = document.getElementById("name");
    const nameError = document.getElementById("nameError");
    const regex = /^.{1,100}$/;
    if (name.value.trim() === "") {
        name.classList.add("error");
        name.classList.remove("correct");
        nameError.textContent = "City name is required.";
    } else if (!regex.test(name.value)) {
        name.classList.add("error");
        name.classList.remove("correct");
        nameError.textContent = "City name can't be longer than 100 characters.";
    } else {
        name.classList.remove("error");
        name.classList.add("correct");
        nameError.textContent = "";
    }
}

function validateState() {
    const state = document.getElementById("state");
    const stateError = document.getElementById("stateError");
    const regex = /^.{1,100}$/;
    if (state.value.trim() === "") {
        state.classList.add("error");
        state.classList.remove("correct");
        stateError.textContent = "State is required.";
    } else if (!regex.test(state.value)) {
        state.classList.add("error");
        state.classList.remove("correct");
        stateError.textContent = "State can't be longer than 100 characters.";
    } else {
        state.classList.remove("error");
        state.classList.add("correct");
        stateError.textContent = "";
    }
}

function validateCountry() {
    const country = document.getElementById("country");
    const countryError = document.getElementById("countryError");
    const regex = /^.{1,100}$/;
    if (country.value.trim() === "") {
        country.classList.add("error");
        country.classList.remove("correct");
        countryError.textContent = "Country is required.";
    } else if (!regex.test(country.value)) {
        country.classList.add("error");
        country.classList.remove("correct");
        countryError.textContent = "Country can't be longer than 100 characters.";
    } else {
        country.classList.remove("error");
        country.classList.add("correct");
        countryError.textContent = "";
    }
}

function validatePincode() {
    const pincode = document.getElementById("pincode");
    const pincodeError = document.getElementById("pincodeError");
    const regex = /^[0-9]{1,6}$/;
    if (pincode.value.trim() === "") {
        pincode.classList.add("error");
        pincode.classList.remove("correct");
        pincodeError.textContent = "Pincode is required.";
    } else if (!regex.test(pincode.value)) {
        pincode.classList.add("error");
        pincode.classList.remove("correct");
        pincodeError.textContent = "Pincode can't be longer than 6 characters.";
    } else {
        pincode.classList.remove("error");
        pincode.classList.add("correct");
        pincodeError.textContent = "";
    }
}

function validateEditCityName() {
    const name = document.getElementById("editName");
    const nameError = document.getElementById("editNameError");
    const regex = /^.{1,100}$/;
    if (name.value.trim() === "") {
        name.classList.add("error");
        name.classList.remove("correct");
        nameError.textContent = "City name is required.";
    } else if (!regex.test(name.value)) {
        name.classList.add("error");
        name.classList.remove("correct");
        nameError.textContent = "City name can't be longer than 100 characters.";
    } else {
        name.classList.remove("error");
        name.classList.add("correct");
        nameError.textContent = "";
    }
}

function validateEditState() {
    const state = document.getElementById("editState");
    const stateError = document.getElementById("editStateError");
    const regex = /^.{1,100}$/;
    if (state.value.trim() === "") {
        state.classList.add("error");
        state.classList.remove("correct");
        stateError.textContent = "State is required.";
    } else if (!regex.test(state.value)) {
        state.classList.add("error");
        state.classList.remove("correct");
        stateError.textContent = "State can't be longer than 100 characters.";
    } else {
        state.classList.remove("error");
        state.classList.add("correct");
        stateError.textContent = "";
    }
}

function validateEditCountry() {
    const country = document.getElementById("editCountry");
    const countryError = document.getElementById("editCountryError");
    const regex = /^.{1,100}$/;
    if (country.value.trim() === "") {
        country.classList.add("error");
        country.classList.remove("correct");
        countryError.textContent = "Country is required.";
    } else if (!regex.test(country.value)) {
        country.classList.add("error");
        country.classList.remove("correct");
        countryError.textContent = "Country can't be longer than 100 characters.";
    } else {
        country.classList.remove("error");
        country.classList.add("correct");
        countryError.textContent = "";
    }
}

function validateEditPincode() {
    const pincode = document.getElementById("editPincode");
    const pincodeError = document.getElementById("editPincodeError");
    const regex = /^[0-9]{1,6}$/;
    if (pincode.value.trim() === "") {
        pincode.classList.add("error");
        pincode.classList.remove("correct");
        pincodeError.textContent = "Pincode is required.";
    } else if (!regex.test(pincode.value)) {
        pincode.classList.add("error");
        pincode.classList.remove("correct");
        pincodeError.textContent = "Pincode can't be longer than 6 characters.";
    } else {
        pincode.classList.remove("error");
        pincode.classList.add("correct");
        pincodeError.textContent = "";
    }
}
