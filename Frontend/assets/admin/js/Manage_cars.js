document.getElementById('showCarsLink').addEventListener('click', function (e) {
    e.preventDefault();
    const showCarsContent = document.getElementById('showCarsContent');
    showCarsContent.classList.toggle('d-none');
    
    if (!showCarsContent.classList.contains('d-none')) {
      // Populate the car data here
      const carTableBody = document.getElementById('carTableBody');
      carTableBody.innerHTML = ''; // Clear previous data
  
      // Example car data
      const cars = [
        { id: 1, make: 'Toyota', model: 'Camry', year: 2021, cityId: 101, status: 'Available', transmission: 'Automatic', seats: 5, category: 'Medium', pricePerDay: 50 },
        // Add more car data here
      ];
  
      cars.forEach(car => {
        const row = document.createElement('tr');
        row.innerHTML = `
          <th scope="row">${car.id}</th>
          <td>${car.make}</td>
          <td>${car.model}</td>
          <td>${car.year}</td>
          <td>${car.cityId}</td>
          <td>${car.status}</td>
          <td>${car.transmission}</td>
          <td>${car.seats}</td>
          <td>${car.category}</td>
          <td>${car.pricePerDay}</td>
        `;
        carTableBody.appendChild(row);
      });
    }
  });
  