document.addEventListener('DOMContentLoaded', () => {
    fetchBookings();
});

function fetchBookings() {
    fetch('http://localhost:5071/api/Bookings', {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        }
    })
    .then(response => response.json())
    .then(bookings => {
        const container = document.getElementById('booking-cards-container');
        container.innerHTML = '';

        bookings.forEach(booking => {
            const card = document.createElement('div');
            card.classList.add('card');
            card.innerHTML = `
                <p><strong>Booking ID:</strong> ${booking.id}</p>
                <p><strong>Customer ID:</strong> ${booking.customerId}</p>
                <p><strong>Car ID:</strong> ${booking.carId}</p>
                <p><strong>Booking Date:</strong> ${booking.bookingDate}</p>
                <p><strong>Start Date:</strong> ${booking.startDate}</p>
                <p><strong>End Date:</strong> ${booking.endDate}</p>
                <p><strong>Status:</strong> ${booking.status}</p>
                <button class="btn btn-danger" onclick="openCancelModal(${booking.id})">Cancel</button>
            `;
            container.appendChild(card);
        });
    })
    .catch(error => console.error('Error fetching bookings:', error));
}

function openCancelModal(bookingId) {
    document.getElementById('delete-modal').style.display = 'flex';
    document.getElementById('delete-modal').dataset.bookingId = bookingId;
}

function closeCancelModal() {
    document.getElementById('delete-modal').style.display = 'none';
    delete document.getElementById('delete-modal').dataset.bookingId;
}

function confirmCancel() {
    const bookingId = document.getElementById('delete-modal').dataset.bookingId;
    
    fetch(`http://localhost:5071/api/Bookings/${bookingId}/cancel`, {
        method: 'PUT',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        }
    })
    .then(response => {
        if (response.ok) {
            closeCancelModal();
            fetchBookings();
        } else {
            console.error('Error canceling booking:', response.statusText);
        }
    })
    .catch(error => console.error('Error canceling booking:', error));
}
