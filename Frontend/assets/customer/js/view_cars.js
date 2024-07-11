document.addEventListener('DOMContentLoaded', function () {
    const carsContainer = document.getElementById('carsContainer');
    const bookCarForm = document.getElementById('bookCarForm');
    const searchInput = document.getElementById('searchInput');
    const categoryFilter = document.getElementById('categoryFilter');
    const yearFilter = document.getElementById('yearFilter');
    const seatsFilter = document.getElementById('seatsFilter');
    const priceFilter = document.getElementById('priceFilter');
    const minRatingFilter = document.getElementById('minRatingFilter');
    const maxRatingFilter = document.getElementById('maxRatingFilter');
    const resetButton = document.getElementById('resetButton');

    let carsData = []; // To store original data fetched

    // Function to fetch cars data
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
            carsData = cars; // Store original data
            renderCars(cars); // Initial rendering of cars
        })
        .catch(error => {
            console.error('Error fetching cars:', error.message);
            alert('Error fetching cars. Please try again later.');
        });
    };

    // Function to render cars based on filters
    const renderCars = (cars) => {
        carsContainer.innerHTML = '';
        cars.forEach(car => {
            // Apply filters here
            if (
                (searchInput.value.trim() === '' || 
                 car.make.toLowerCase().includes(searchInput.value.toLowerCase()) || 
                 car.model.toLowerCase().includes(searchInput.value.toLowerCase())) &&
                (categoryFilter.value === '' || car.category === categoryFilter.value) &&
                (yearFilter.value === '' || car.year == yearFilter.value) &&
                (seatsFilter.value === '' || car.numberOfSeats >= seatsFilter.value) &&
                (priceFilter.value === '' || car.pricePerDay <= priceFilter.value) &&
                (minRatingFilter.value === '' || (car.averageRating && car.averageRating >= minRatingFilter.value)) &&
                (maxRatingFilter.value === '' || (car.averageRating && car.averageRating <= maxRatingFilter.value))
            ) {
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
            }
        });

        // If no cars match filters
        if (carsContainer.children.length === 0) {
            carsContainer.innerHTML = '<p>No cars found.</p>';
        }

        // Attach event listeners to book buttons
        document.querySelectorAll('.btn-book').forEach(button => {
            button.addEventListener('click', (event) => {
                const carId = event.target.getAttribute('data-id');
                document.getElementById('bookCarId').value = carId;
            });
        });
    };

    // Event listeners for filters
    searchInput.addEventListener('input', () => {
        renderCars(carsData);
    });

    categoryFilter.addEventListener('change', () => {
        renderCars(carsData);
    });

    yearFilter.addEventListener('input', () => {
        renderCars(carsData);
    });

    seatsFilter.addEventListener('input', () => {
        renderCars(carsData);
    });

    priceFilter.addEventListener('input', () => {
        renderCars(carsData);
    });

    minRatingFilter.addEventListener('input', () => {
        renderCars(carsData);
    });

    maxRatingFilter.addEventListener('input', () => {
        renderCars(carsData);
    });

    resetButton.addEventListener('click', () => {
        searchInput.value = '';
        categoryFilter.value = '';
        yearFilter.value = '';
        seatsFilter.value = '';
        priceFilter.value = '';
        minRatingFilter.value = '';
        maxRatingFilter.value = '';
        renderCars(carsData);
    });

    // Function to handle form submission for booking
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
            fetchCars(); // Refresh car list after booking
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

    // Initial fetch and render
    fetchCars();
});
