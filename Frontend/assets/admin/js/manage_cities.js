document.addEventListener('DOMContentLoaded', function () {
    const citiesContainer = document.getElementById('citiesContainer');
    const addCityForm = document.getElementById('addCityForm');
    const editCityForm = document.getElementById('editCityForm');

    const fetchCities = () => {
        const token = localStorage.getItem("jwtToken");
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
                    const token = localStorage.getItem("jwtToken");
                    fetch(`http://localhost:5071/api/Cities/delete/${cityId}`, {
                        method: 'DELETE',
                        headers: {
                            'Authorization': `Bearer ${token}`,
                        },
                    })
                    .then(response => {
                        if (!response.ok) {
                            throw response;
                        }
                        return response.json();
                    })
                    .then(() => fetchCities())
                    .catch(async error => {
                        let errorMessage = 'Error deleting city.';
                        if (error.json) {
                            const errorResponse = await error.json();
                            errorMessage = errorResponse.message || errorMessage;
                        }
                        console.error('Error:', errorMessage);
                        alert(errorMessage);
                    });
                });
            });
        })
        .catch(error => {
            console.error('Error fetching cities:', error.message);
            alert('Error fetching cities. Please try again later.');
        });
    };

    addCityForm.addEventListener('submit', function (event) {
        event.preventDefault();
        const formData = new FormData(addCityForm);
        const cityData = Object.fromEntries(formData.entries());
        const token = localStorage.getItem("jwtToken");

        fetch('http://localhost:5071/api/Cities/add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
            body: JSON.stringify(cityData),
        })
        .then(response => {
            if (!response.ok) {
                throw response;
            }
            return response.json();
        })
        .then(() => {
            addCityForm.reset();
            const addCityModal = bootstrap.Modal.getInstance(document.getElementById('addCityModal'));
            addCityModal.hide();
            fetchCities();
        })
        .catch(async error => {
            let errorMessage = 'Error adding city.';
            if (error.json) {
                const errorResponse = await error.json();
                errorMessage = errorResponse.message || errorMessage;
            }
            console.error('Error:', errorMessage);
            alert(errorMessage);
        });
    });

    editCityForm.addEventListener('submit', function (event) {
        event.preventDefault();
        const cityId = document.getElementById('editCityId').value;
        const formData = new FormData(editCityForm);
        const cityData = Object.fromEntries(formData.entries());
        const token = localStorage.getItem("jwtToken");

        fetch(`http://localhost:5071/api/Cities/edit/${cityId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
            body: JSON.stringify(cityData),
        })
        .then(response => {
            if (!response.ok) {
                throw response;
            }
            return response.json();
        })
        .then(() => {
            const editCityModal = bootstrap.Modal.getInstance(document.getElementById('editCityModal'));
            editCityModal.hide();
            fetchCities();
        })
        .catch(async error => {
            let errorMessage = 'Error editing city.';
            if (error.json) {
                const errorResponse = await error.json();
                errorMessage = errorResponse.message || errorMessage;
            }
            console.error('Error:', errorMessage);
            alert(errorMessage);
        });
    });

    fetchCities(); // Fetch cities initially when the page loads
});
