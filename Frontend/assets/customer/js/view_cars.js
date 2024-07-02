document.addEventListener('DOMContentLoaded', function () {
    const carsContainer = document.getElementById('carsContainer');
    const bookCarForm = document.getElementById('bookCarForm');

    const fetchCars = () => {
        const token = localStorage.getItem("jwtToken");
        fetch('http://localhost:5071/api/Customers/view-cars', {
            method: 'GET',
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
            carsContainer.innerHTML = '';
            cars.forEach(car => {
                const card = document.createElement('div');
                card.classList.add('col');
                card.innerHTML = 
                    `<div class="card h-100">
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title">${car.make} ${car.model}</h5>
                            <p class="card-text">Year: ${car.year}</p>
                            <p class="card-text">Category: ${car.category}</p>
                            <p class="card-text">Transmission: ${car.transmission}</p>
                            <p class="card-text">Seats: ${car.numberOfSeats}</p>
                            <p class="card-text">Price per day: $${car.pricePerDay.toFixed(2)}</p>
                            <p class="card-text">Rating: ${car.averageRating ? car.averageRating.toFixed(1) : 'N/A'}</p>
                            <p class="card-text">Status: ${car.status}</p>
                            <button class="btn btn-primary mt-auto btn-book" data-bs-toggle="modal" data-bs-target="#bookCarModal" data-id="${car.id}">Book</button>
                        </div>
                    </div>`;
                carsContainer.appendChild(card);
            });

            document.querySelectorAll('.btn-book').forEach(button => {
                button.addEventListener('click', (event) => {
                    const carId = event.target.getAttribute('data-id');
                    document.getElementById('bookCarId').value = carId;
                });
            });
        })
        .catch(error => {
            console.error('Error fetching cars:', error.message);
            alert('Error fetching cars. Please try again later.');
        });
    };

    bookCarForm.addEventListener('submit', function (event) {
        event.preventDefault();
        const carId = document.getElementById('bookCarId').value;
        const formData = new FormData(bookCarForm);
        // Get today's date in YYYY-MM-DD format
        const today = new Date().toISOString().split('T')[0];

        const bookingDetails = {
            carId: carId,
            bookingDate: today,
            startDate: formData.get('startDate'),
            endDate: formData.get('endDate')
        };
        const token = localStorage.getItem("jwtToken");

        fetch('http://localhost:5071/api/Bookings/book', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
            body: JSON.stringify(bookingDetails),
        })
        .then(response => {
            if (!response.ok) {
                throw response;
            }
            return response.json();
        })
        .then(() => {
            bookCarForm.reset();
            const bookCarModal = bootstrap.Modal.getInstance(document.getElementById('bookCarModal'));
            bookCarModal.hide();
            fetchCars();
        })
        .catch(async error => {
            let errorMessage = 'Error booking car.';
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
