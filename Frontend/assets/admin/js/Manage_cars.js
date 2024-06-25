document.addEventListener('DOMContentLoaded', function () {
  const carCardsContainer = document.getElementById('carCardsContainer');
  const addCarForm = document.getElementById('addCarForm');
  const editCarForm = document.getElementById('editCarForm');

  const fetchCars = () => {
    const token = localStorage.getItem("jwtToken");
    fetch('http://localhost:5071/api/Cars/all', {
      headers: {
        'Authorization': `Bearer ${token}`,
      },
    })
    .then(response => {
      if (!response.ok) {
        throw new Error('Failed to fetch cars.');
      }
      return response.json();
    })
    .then(cars => {
      carCardsContainer.innerHTML = '';
      cars.forEach(car => {
        const card = document.createElement('div');
        card.classList.add('col-md-4');
        card.innerHTML = 
          `<div class="card">
            <div class="card-body">
              <h5 class="card-title">${car.make} ${car.model}</h5>
              <p class="card-text">ID: ${car.id}</p>
              <p class="card-text">Year: ${car.year}</p>
              <p class="card-text">City ID: ${car.cityId}</p>
              <p class="card-text">Status: ${car.status}</p>
              <p class="card-text">Transmission: ${car.transmission}</p>
              <p class="card-text">Seats: ${car.numberOfSeats}</p>
              <p class="card-text">Category: ${car.category}</p>
              <p class="card-text">Price Per Day: $${car.pricePerDay}</p>
              <button class="btn btn-edit" data-bs-toggle="modal" data-bs-target="#editCarModal" 
                data-id="${car.id}"
                data-make="${car.make}"
                data-model="${car.model}"
                data-year="${car.year}"
                data-city-id="${car.cityId}"
                data-status="${car.status}"
                data-transmission="${car.transmission}"
                data-seats="${car.numberOfSeats}"
                data-category="${car.category}"
                data-price-per-day="${car.pricePerDay}">Edit</button>
              <button class="btn btn-delete" data-id="${car.id}">Delete</button>
            </div>
          </div>`;
        carCardsContainer.appendChild(card);
      });

      document.querySelectorAll('.btn-edit').forEach(button => {
        button.addEventListener('click', (event) => {
          const carId = event.target.getAttribute('data-id');
          const make = event.target.getAttribute('data-make');
          const model = event.target.getAttribute('data-model');
          const year = event.target.getAttribute('data-year');
          const cityId = event.target.getAttribute('data-city-id');
          const status = event.target.getAttribute('data-status');
          const transmission = event.target.getAttribute('data-transmission');
          const seats = event.target.getAttribute('data-seats');
          const category = event.target.getAttribute('data-category');
          const pricePerDay = event.target.getAttribute('data-price-per-day');
          
          document.getElementById('carId').value = carId;
          document.getElementById('editMake').value = make;
          document.getElementById('editModel').value = model;
          document.getElementById('editYear').value = year;
          document.getElementById('editCityId').value = cityId;
          document.getElementById('editStatus').value = status;
          document.getElementById('editTransmission').value = transmission;
          document.getElementById('editNumberOfSeats').value = seats;
          document.getElementById('editCategory').value = category;
          document.getElementById('editPricePerDay').value = pricePerDay;
        });
      });

      document.querySelectorAll('.btn-delete').forEach(button => {
        button.addEventListener('click', (event) => {
          const carId = event.target.getAttribute('data-id');
          const token = localStorage.getItem("jwtToken");
          fetch(`http://localhost:5071/api/Cars/delete/${carId}`, {
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
          .then(() => fetchCars())
          .catch(async error => {
            let errorMessage = 'Error deleting car.';
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
      console.error('Error fetching cars:', error.message);
      alert('Error fetching cars. Please try again later.');
    });
  };

  addCarForm.addEventListener('submit', function (event) {
    event.preventDefault();
    const formData = new FormData(addCarForm);
    const carData = Object.fromEntries(formData.entries());
    const token = localStorage.getItem("jwtToken");

    fetch('http://localhost:5071/api/Cars/add', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
      body: JSON.stringify(carData),
    })
    .then(response => {
      if (!response.ok) {
        throw response;
      }
      return response.json();
    })
    .then(() => {
      addCarForm.reset();
      const addCarModal = bootstrap.Modal.getInstance(document.getElementById('addCarModal'));
      addCarModal.hide();
      fetchCars();
    })
    .catch(async error => {
      let errorMessage = 'Error adding car.';
      if (error.json) {
        const errorResponse = await error.json();
        errorMessage = errorResponse.message || errorMessage;
      }
      console.error('Error:', errorMessage);
      alert(errorMessage);
    });
  });

  editCarForm.addEventListener('submit', function (event) {
    event.preventDefault();
    const carId = document.getElementById('carId').value;
    const formData = new FormData(editCarForm);
    const carData = Object.fromEntries(formData.entries());
    const token = localStorage.getItem("jwtToken");

    //console.log("Token Passed:" + token);

    fetch(`http://localhost:5071/api/Cars/edit/${carId}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
      body: JSON.stringify(carData),
    })
    .then(response => {
      if (!response.ok) {
        throw response;
      }
      return response.json();
    })
    .then(() => {
      editCarForm.reset();
      const editCarModal = bootstrap.Modal.getInstance(document.getElementById('editCarModal'));
      editCarModal.hide();
      fetchCars();
    })
    .catch(async error => {
      let errorMessage = 'Error editing car.';
      if (error.json) {
        const errorResponse = await error.json();
        errorMessage = errorResponse.message || errorMessage;
      }
      console.error('Error:', errorMessage);
      alert(errorMessage);
    });
  });

  fetchCars();
});
