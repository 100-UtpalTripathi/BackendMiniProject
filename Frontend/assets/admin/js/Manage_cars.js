document.addEventListener("DOMContentLoaded", function () {
  const carCardsContainer = document.getElementById("carCardsContainer");
  const addCarForm = document.getElementById("addCarForm");
  const editCarForm = document.getElementById("editCarForm");
  const searchInput = document.getElementById("searchInput");
  const categoryFilter = document.getElementById("categoryFilter");
  const yearFilter = document.getElementById("yearFilter");
  const seatsFilter = document.getElementById("seatsFilter");
  const priceFilter = document.getElementById("priceFilter");
  const resetButton = document.getElementById("resetButton");
  const minRatingFilter = document.getElementById("minRatingFilter");
  const maxRatingFilter = document.getElementById("maxRatingFilter");
  let allCars = [];

  const fetchCars = () => {
    const token = localStorage.getItem("jwtToken");
    fetch("http://localhost:5071/api/Cars/all", {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error("Failed to fetch cars.");
        }
        return response.json();
      })
      .then((cars) => {
        allCars = cars;
        displayCars(cars);
      })
      .catch((error) => {
        console.error("Error fetching cars:", error.message);
        alert("Error fetching cars. Please try again later.");
      });
  };

  const displayCars = (cars) => {
    carCardsContainer.innerHTML = "";
    if (cars.length === 0) {
      carCardsContainer.innerHTML = "<p>No cars found</p>";
    }
    cars.forEach((car) => {
      const card = document.createElement("div");
      card.classList.add("col-md-4");
      card.innerHTML = `<div class="card">
                  <div class="card-body">
                      <h5 class="card-title">${car.make} ${car.model}</h5>
                      <p class="card-text">Year: ${car.year}</p>
                      <p class="card-text">City ID: ${car.cityId}</p>
                      <p class="card-text">Status: ${car.status}</p>
                      <p class="card-text">Transmission: ${car.transmission}</p>
                      <p class="card-text">Seats: ${car.numberOfSeats}</p>
                      <p class="card-text">Category: ${car.category}</p>
                      <p class="card-text">Rating: ${car.averageRating}</p>
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

    document.querySelectorAll(".btn-edit").forEach((button) => {
      button.addEventListener("click", (event) => {
        const carId = event.target.getAttribute("data-id");
        const make = event.target.getAttribute("data-make");
        const model = event.target.getAttribute("data-model");
        const year = event.target.getAttribute("data-year");
        const cityId = event.target.getAttribute("data-city-id");
        const status = event.target.getAttribute("data-status");
        const transmission = event.target.getAttribute("data-transmission");
        const seats = event.target.getAttribute("data-seats");
        const category = event.target.getAttribute("data-category");
        const pricePerDay = event.target.getAttribute("data-price-per-day");

        document.getElementById("carId").value = carId;
        document.getElementById("editMake").value = make;
        document.getElementById("editModel").value = model;
        document.getElementById("editYear").value = year;
        document.getElementById("editCityId").value = cityId;
        document.getElementById("editStatus").value = status;
        document.getElementById("editTransmission").value = transmission;
        document.getElementById("editNumberOfSeats").value = seats;
        document.getElementById("editCategory").value = category;
        document.getElementById("editPricePerDay").value = pricePerDay;
      });
    });

    document.querySelectorAll(".btn-delete").forEach((button) => {
      button.addEventListener("click", (event) => {
        const carId = event.target.getAttribute("data-id");
        const token = localStorage.getItem("jwtToken");
        fetch(`http://localhost:5071/api/Cars/delete/${carId}`, {
          method: "DELETE",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        })
          .then((response) => {
            if (!response.ok) {
              throw response;
            }
            return response.json();
          })
          .then(() => fetchCars())
          .catch(async (error) => {
            let errorMessage = "Error deleting car.";
            if (error.json) {
              const errorResponse = await error.json();
              errorMessage = errorResponse.message || errorMessage;
            }
            console.error("Error:", errorMessage);
            alert(errorMessage);
          });
      });
    });
  };

  const applyFilters = () => {
    const query = searchInput.value.trim().toLowerCase();
    const category = categoryFilter.value;
    const year = yearFilter.value;
    const seats = seatsFilter.value;
    const price = priceFilter.value;
    const minRating = minRatingFilter.value;
    const maxRating = maxRatingFilter.value;

    const filteredCars = allCars.filter((car) => {
      const makeMatch = car.make.toLowerCase().includes(query);
      const modelMatch = car.model.toLowerCase().includes(query);
      const categoryMatch = !category || car.category === category;
      const yearMatch = !year || car.year === parseInt(year);
      const seatsMatch = !seats || car.numberOfSeats === parseInt(seats);
      const priceMatch = !price || car.pricePerDay <= parseFloat(price);
      const minRatingMatch =
        !minRating || car.averageRating >= parseFloat(minRating);
      const maxRatingMatch =
        !maxRating || car.averageRating <= parseFloat(maxRating);

      return (
        (makeMatch || modelMatch) &&
        categoryMatch &&
        yearMatch &&
        seatsMatch &&
        priceMatch &&
        minRatingMatch &&
        maxRatingMatch
      );
    });

    displayCars(filteredCars);
  };

  searchInput.addEventListener("input", applyFilters);
  categoryFilter.addEventListener("change", applyFilters);
  yearFilter.addEventListener("input", applyFilters);
  seatsFilter.addEventListener("input", applyFilters);
  priceFilter.addEventListener("input", applyFilters);
  minRatingFilter.addEventListener("input", applyFilters);
  maxRatingFilter.addEventListener("input", applyFilters);

  resetButton.addEventListener("click", function () {
    searchInput.value = "";
    categoryFilter.value = "";
    yearFilter.value = "";
    seatsFilter.value = "";
    priceFilter.value = "";
    displayCars(allCars);
  });

  addCarForm.addEventListener("submit", function (event) {
    event.preventDefault();
    const formData = new FormData(addCarForm);
    const carData = Object.fromEntries(formData.entries());
    const token = localStorage.getItem("jwtToken");

    fetch("http://localhost:5071/api/Cars/add", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(carData),
    })
      .then((response) => {
        if (!response.ok) {
          throw response;
        }
        return response.json();
      })
      .then(() => {
        addCarForm.reset();
        const addCarModal = bootstrap.Modal.getInstance(
          document.getElementById("addCarModal")
        );
        addCarModal.hide();
        fetchCars();
      })
      .catch(async (error) => {
        let errorMessage = "Error adding car.";
        if (error.json) {
          const errorResponse = await error.json();
          errorMessage = errorResponse.message || errorMessage;
        }
        console.error("Error:", errorMessage);
        alert(errorMessage);
      });
  });

  editCarForm.addEventListener("submit", function (event) {
    event.preventDefault();
    const carId = document.getElementById("carId").value;
    const formData = new FormData(editCarForm);
    const carData = Object.fromEntries(formData.entries());
    const token = localStorage.getItem("jwtToken");

    fetch(`http://localhost:5071/api/Cars/edit/${carId}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`,
      },
      body: JSON.stringify(carData),
    })
      .then((response) => {
        if (!response.ok) {
          throw response;
        }
        return response.json();
      })
      .then(() => {
        editCarForm.reset();
        const editCarModal = bootstrap.Modal.getInstance(
          document.getElementById("editCarModal")
        );
        editCarModal.hide();
        fetchCars();
      })
      .catch(async (error) => {
        let errorMessage = "Error editing car.";
        if (error.json) {
          const errorResponse = await error.json();
          errorMessage = errorResponse.message || errorMessage;
        }
        console.error("Error:", errorMessage);
        alert(errorMessage);
      });
  });

  fetchCars();
});

// Validation functions
function validateMake() {
  const make = document.getElementById("make");
  const makeError = document.getElementById("makeError");
  const regex = /^.{1,20}$/;
  if (make.value.trim() === "") {
      make.classList.add("error");
      make.classList.remove("correct");
      makeError.textContent = "Make is required.";
  } else if (!regex.test(make.value)) {
      make.classList.add("error");
      make.classList.remove("correct");
      makeError.textContent = "Make can't be longer than 20 characters.";
  } else {
      make.classList.remove("error");
      make.classList.add("correct");
      makeError.textContent = "";
  }
}

function validateModel() {
  const model = document.getElementById("model");
  const modelError = document.getElementById("modelError");
  const regex = /^.{1,20}$/;
  if (model.value.trim() === "") {
      model.classList.add("error");
      model.classList.remove("correct");
      modelError.textContent = "Model is required.";
  } else if (!regex.test(model.value)) {
      model.classList.add("error");
      model.classList.remove("correct");
      modelError.textContent = "Model can't be longer than 20 characters.";
  } else {
      model.classList.remove("error");
      model.classList.add("correct");
      modelError.textContent = "";
  }
}

function validateYear() {
  const year = document.getElementById("year");
  const yearError = document.getElementById("yearError");
  const regex = /^(1886|188[7-9]|18[9-9][0-9]|19[0-9]{2}|200[0-9]|201[0-9]|202[0-9]|203[0-9]|204[0-9]|205[0-9]|20[6-9][0-9]|9999)$/;
  if (year.value.trim() === "") {
      year.classList.add("error");
      year.classList.remove("correct");
      yearError.textContent = "Year is required.";
  } else if (!regex.test(year.value)) {
      year.classList.add("error");
      year.classList.remove("correct");
      yearError.textContent = "Year must be a valid year.";
  } 
  else if(parseInt(year.value) > new Date().getFullYear()){
    year.classList.add("error");
    year.classList.remove("correct");
    yearError.textContent = "Year must be a valid year.";
  }
  else {
      year.classList.remove("error");
      year.classList.add("correct");
      yearError.textContent = "";
  }
}

function validateCityId() {
  const cityId = document.getElementById("cityId");
  const cityIdError = document.getElementById("cityIdError");
  if (cityId.value.trim() === "") {
      cityId.classList.add("error");
      cityId.classList.remove("correct");
      cityIdError.textContent = "City ID is required.";
  } else {
      cityId.classList.remove("error");
      cityId.classList.add("correct");
      cityIdError.textContent = "";
  }
}



function validateNumberOfSeats() {
  const numberOfSeats = document.getElementById("numberOfSeats");
  const numberOfSeatsError = document.getElementById("numberOfSeatsError");
  const regex = /^[1-9]|1[0-9]|20$/;
  if (numberOfSeats.value.trim() === "") {
      numberOfSeats.classList.add("error");
      numberOfSeats.classList.remove("correct");
      numberOfSeatsError.textContent = "Number of seats is required.";
  } else if (!regex.test(numberOfSeats.value)) {
      numberOfSeats.classList.add("error");
      numberOfSeats.classList.remove("correct");
      numberOfSeatsError.textContent = "Number of seats must be between 1 and 20.";
  } else {
      numberOfSeats.classList.remove("error");
      numberOfSeats.classList.add("correct");
      numberOfSeatsError.textContent = "";
  }
}

function validateEditMake(){
  const make = document.getElementById("editMake");
  const makeError = document.getElementById("editMakeError");
  const regex = /^.{1,20}$/;
  if (make.value.trim() === "") {
      make.classList.add("error");
      make.classList.remove("correct");
      makeError.textContent = "Make is required.";
  } else if (!regex.test(make.value)) {
      make.classList.add("error");
      make.classList.remove("correct");
      makeError.textContent = "Make can't be longer than 20 characters.";
  } else {
      make.classList.remove("error");
      make.classList.add("correct");
      makeError.textContent = "";
  }
}

function validateEditModel(){
  const model = document.getElementById("editModel");
  const modelError = document.getElementById("editModelError");
  const regex = /^.{1,20}$/;
  if (model.value.trim() === "") {
      model.classList.add("error");
      model.classList.remove("correct");
      modelError.textContent = "Model is required.";
  } else if (!regex.test(model.value)) {
      model.classList.add("error");
      model.classList.remove("correct");
      modelError.textContent = "Model can't be longer than 20 characters.";
  } else {
      model.classList.remove("error");
      model.classList.add("correct");
      modelError.textContent = "";
  }
}

function validateEditYear(){
  const year = document.getElementById("editYear");
  const yearError = document.getElementById("editYearError");
  const regex = /^(1886|188[7-9]|18[9-9][0-9]|19[0-9]{2}|200[0-9]|201[0-9]|202[0-9]|203[0-9]|204[0-9]|205[0-9]|20[6-9][0-9]|9999)$/;
  if (year.value.trim() === "") {
      year.classList.add("error");
      year.classList.remove("correct");
      yearError.textContent = "Year is required.";
  } else if (!regex.test(year.value)) {
      year.classList.add("error");
      year.classList.remove("correct");
      yearError.textContent = "Year must be a valid year.";
  } else if(parseInt(year.value) > new Date().getFullYear()){
      year.classList.add("error");
      year.classList.remove("correct");
      yearError.textContent = "Year must be a valid year.";
  }
  else {
      year.classList.remove("error");
      year.classList.add("correct");
      yearError.textContent = "";
  }
}

function validateEditCityId(){
  const cityId = document.getElementById("editCityId");
  const cityIdError = document.getElementById("editCityIdError");
  if (cityId.value.trim() === "") {
      cityId.classList.add("error");
      cityId.classList.remove("correct");
      cityIdError.textContent = "City ID is required.";
  } else {
      cityId.classList.remove("error");
      cityId.classList.add("correct");
      cityIdError.textContent = "";
  }
}

function validateEditNumberOfSeats(){
  const numberOfSeats = document.getElementById("editNumberOfSeats");
  const numberOfSeatsError = document.getElementById("editNumberOfSeatsError");
  const regex = /^[1-9]|1[0-9]|20$/;
  if (numberOfSeats.value.trim() === "") {
      numberOfSeats.classList.add("error");
      numberOfSeats.classList.remove("correct");
      numberOfSeatsError.textContent = "Number of seats is required.";
  } else if (!regex.test(numberOfSeats.value)) {
      numberOfSeats.classList.add("error");
      numberOfSeats.classList.remove("correct");
      numberOfSeatsError.textContent = "Number of seats must be between 1 and 20.";
  } else {
      numberOfSeats.classList.remove("error");
      numberOfSeats.classList.add("correct");
      numberOfSeatsError.textContent = "";
  }
}

