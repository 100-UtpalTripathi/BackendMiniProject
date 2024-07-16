document.addEventListener("DOMContentLoaded", function () {
  const bookingCardsContainer = document.getElementById(
    "bookingCardsContainer"
  );
  const statusFilter = document.getElementById("statusFilter");
  const startDateFilter = document.getElementById("startDateFilter");
  const endDateFilter = document.getElementById("endDateFilter");
  const resetFiltersButton = document.getElementById("resetFilters");
  let allBookings = [];

  const fetchBookings = () => {
    const token = localStorage.getItem("jwtToken");
    fetch("http://localhost:5071/api/Bookings", {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error("Failed to fetch bookings.");
        }
        return response.json();
      })
      .then((bookings) => {
        allBookings = bookings;
        displayBookings(bookings);
      })
      .catch((error) => {
        console.error("Error fetching bookings:", error.message);
        alert("Error fetching bookings. Please try again later.");
      });
  };

  const displayBookings = (bookings) => {
    bookingCardsContainer.innerHTML = "";
    if (bookings.length === 0) {
      bookingCardsContainer.innerHTML = "<p>No bookings found</p>";
    }
    bookings.forEach((booking) => {
      const card = document.createElement("div");
      card.classList.add("col-md-4", "mb-4");

      const currentDate = new Date();
      const startDate = new Date(booking.startDate);

      let cancelButton = "";
      if (currentDate < startDate && booking.status !== "Cancelled") {
        cancelButton = `<button class="btn btn-danger" onclick="openCancelModal(${booking.id})">Cancel</button>`;
      }

      card.innerHTML = `<div class="card">
                      <div class="card-body">
                          <p><strong>Booking ID:</strong> ${booking.id}</p>
                          <p><strong>Customer ID:</strong> ${
                            booking.customerId
                          }</p>
                          <p><strong>Car ID:</strong> ${booking.carId}</p>
                          <p><strong>Booking Date:</strong> ${new Date(
                            booking.bookingDate
                          ).toLocaleDateString()}</p>
                          <p><strong>Start Date:</strong> ${new Date(
                            booking.startDate
                          ).toLocaleDateString()}</p>
                          <p><strong>End Date:</strong> ${new Date(
                            booking.endDate
                          ).toLocaleDateString()}</p>
                          <p><strong>Status:</strong> ${booking.status}</p>
                          ${cancelButton}
                      </div>
                  </div>`;
      bookingCardsContainer.appendChild(card);
    });
  };

  const applyFilters = () => {
    const status = statusFilter.value;
    const startDate = startDateFilter.value;
    const endDate = endDateFilter.value;

    const filteredBookings = allBookings.filter((booking) => {
      const statusMatch = !status || booking.status === status;
      const startDateMatch =
        !startDate || new Date(booking.bookingDate) >= new Date(startDate);
      const endDateMatch =
        !endDate || new Date(booking.bookingDate) <= new Date(endDate);

      return statusMatch && startDateMatch && endDateMatch;
    });

    displayBookings(filteredBookings);
  };

  const resetFilters = () => {
    statusFilter.value = "";
    startDateFilter.value = "";
    endDateFilter.value = "";
    displayBookings(allBookings);
  };

  statusFilter.addEventListener("change", applyFilters);
  startDateFilter.addEventListener("change", applyFilters);
  endDateFilter.addEventListener("change", applyFilters);
  resetFiltersButton.addEventListener("click", resetFilters);

  const openCancelModal = (bookingId) => {
    document.getElementById("delete-modal").style.display = "flex";
    document.getElementById("delete-modal").dataset.bookingId = bookingId;
  };

  const closeDeleteModal = () => {
    document.getElementById("delete-modal").style.display = "none";
    delete document.getElementById("delete-modal").dataset.bookingId;
  };

  const confirmCancel = () => {
    const bookingId = document.getElementById("delete-modal").dataset.bookingId;
    const token = localStorage.getItem("jwtToken");

    fetch(`http://localhost:5071/api/Bookings/${bookingId}/cancel`, {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then((response) => {
        if (response.ok) {
          closeDeleteModal();
          fetchBookings();
        } else {
          console.error("Error canceling booking:", response.statusText);
        }
      })
      .catch((error) => console.error("Error canceling booking:", error));
  };

  fetchBookings();
});
