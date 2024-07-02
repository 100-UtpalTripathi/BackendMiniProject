document.addEventListener('DOMContentLoaded', function () {
    const bookingsContainer = document.getElementById('bookingsContainer');
    const extendBookingForm = document.getElementById('extendBookingForm');
    const rateCarForm = document.getElementById('rateCarForm');

    const fetchBookings = () => {
        const token = localStorage.getItem("jwtToken");
        fetch('http://localhost:5071/api/Customers/bookings', {
            headers: {
                'Authorization': `Bearer ${token}`,
            },
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch bookings.');
            }
            return response.json();
        })
        .then(bookings => {
            bookingsContainer.innerHTML = '';
            bookings.forEach(booking => {
                const car = booking.car;
                if (!car) {
                    console.error(`Booking with ID ${booking.id} has no associated car.`);
                    return; // Skip this booking
                }

                const card = document.createElement('div');
                card.classList.add('col-md-4');
                card.innerHTML = `
                    <div class="card">
                        <div class="card-body">
                            <h5 class="card-title">${car.make} ${car.model}</h5>
                            <p class="card-text">Year: ${car.year}</p>
                            
                            <p class="card-text">Booking Date: ${new Date(booking.bookingDate).toLocaleDateString()}</p>
                            <p class="card-text">Start Date: ${new Date(booking.startDate).toLocaleDateString()}</p>
                            <p class="card-text">End Date: ${new Date(booking.endDate).toLocaleDateString()}</p>
                            <p class="card-text">Total Amount: $${booking.totalAmount}</p>
                            <p class="card-text">Discount Amount: $${booking.discountAmount}</p>
                            <p class="card-text">Final Amount: $${booking.finalAmount}</p>
                            <p class="card-text">Status: ${booking.status}</p>
                            ${new Date(booking.startDate) > new Date() ? `
                                <button class="btn btn-cancel" data-id="${booking.id}">Cancel Booking</button>
                            ` : ''}
                            
                            ${new Date(booking.endDate) > new Date() ? `
                                <button class="btn btn-extend" data-id="${booking.id}" data-bs-toggle="modal" data-bs-target="#extendBookingModal">Extend Booking</button>
                            ` : ''}
                            <button class="btn btn-rate" data-id="${booking.id}" data-car-id="${car.id}" data-bs-toggle="modal" data-bs-target="#rateCarModal">Rate Car</button>
                        </div>
                    </div>
                `;
                bookingsContainer.appendChild(card);
            });

            document.querySelectorAll('.btn-cancel').forEach(button => {
                button.addEventListener('click', (event) => {
                    const bookingId = event.target.getAttribute('data-id');
                    cancelBooking(bookingId);
                });
            });

            document.querySelectorAll('.btn-extend').forEach(button => {
                button.addEventListener('click', (event) => {
                    const bookingId = event.target.getAttribute('data-id');
                    document.getElementById('extendBookingId').value = bookingId;
                });
            });

            document.querySelectorAll('.btn-rate').forEach(button => {
                button.addEventListener('click', (event) => {
                    const bookingId = event.target.getAttribute('data-id');
                    const carId = event.target.getAttribute('data-car-id');
                    document.getElementById('rateBookingId').value = bookingId;
                    document.getElementById('rateCarId').value = carId;
                });
            });
        })
        .catch(error => {
            console.error('Error fetching bookings:', error.message);
            alert('Error fetching bookings. Please try again later.');
        });
    };

    const cancelBooking = (bookingId) => {
        const token = localStorage.getItem("jwtToken");
        fetch(`http://localhost:5071/api/Bookings/${bookingId}/cancel`, {
            method: 'PUT',
            headers: {
                'Authorization': `Bearer ${token}`,
            },
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to cancel booking.');
            }
            return response.json();
        })
        .then(() => {
            fetchBookings();
        })
        .catch(error => {
            console.error('Error canceling booking:', error.message);
            alert('Error canceling booking. Please try again later.');
        });
    };

    extendBookingForm.addEventListener('submit', function (event) {
        event.preventDefault();
        const bookingId = document.getElementById('extendBookingId').value;
        const formData = new FormData(extendBookingForm);
        const newEndDate = formData.get('newEndDate');
        const token = localStorage.getItem("jwtToken");

        fetch(`http://localhost:5071/api/Bookings/${bookingId}/extend`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
            body: JSON.stringify({ newEndDate: newEndDate }),
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to extend booking.');
            }
            return response.json();
        })
        .then(() => {
            extendBookingForm.reset();
            const extendBookingModal = bootstrap.Modal.getInstance(document.getElementById('extendBookingModal'));
            extendBookingModal.hide();
            fetchBookings();
        })
        .catch(error => {
            console.error('Error extending booking:', error.message);
            alert('Error extending booking. Please try again later.');
        });
    });

    rateCarForm.addEventListener('submit', function (event) {
        event.preventDefault();
        const bookingId = document.getElementById('rateBookingId').value;
        const carId = document.getElementById('rateCarId').value;
        const formData = new FormData(rateCarForm);
        const rating = formData.get('rating');
        const review = formData.get('review');
        const token = localStorage.getItem("jwtToken");

        fetch('http://localhost:5071/api/Customers/rate-car', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
            body: JSON.stringify({ bookingId: bookingId, carId: carId, rating: rating, review: review }),
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to rate car.');
            }
            return response.json();
        })
        .then(() => {
            rateCarForm.reset();
            const rateCarModal = bootstrap.Modal.getInstance(document.getElementById('rateCarModal'));
            rateCarModal.hide();
            fetchBookings();
        })
        .catch(error => {
            console.error('Error rating car:', error.message);
            alert('Error rating car. Please try again later.');
        });
    });

    fetchBookings();
});
