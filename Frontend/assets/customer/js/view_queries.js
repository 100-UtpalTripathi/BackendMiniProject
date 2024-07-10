document.addEventListener('DOMContentLoaded', function () {
    const queriesContainer = document.getElementById('queriesContainer');
    const addQueryForm = document.getElementById('addQueryForm');

    const fetchQueries = () => {
        const token = localStorage.getItem("jwtToken");
        fetch('http://localhost:5071/api/Queries/get/all', {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
            },
        })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch queries.');
            }
            return response.json();
        })
        .then(queries => {
            queriesContainer.innerHTML = '';
            queries.forEach(query => {
                const card = document.createElement('div');
                card.classList.add('col');
                card.innerHTML = `
                    <div class="card">
                        <div class="card-body">
                            <h5 class="card-title">${query.subject}</h5>
                            <p class="card-text">Message: ${query.message}</p>
                            <p class="card-text">Status: ${query.status}</p>
                            <p class="card-text">Created Date: ${new Date(query.createdDate).toLocaleString()}</p>
                            ${query.updatedDate ? `<p class="card-text">Updated Date: ${new Date(query.updatedDate).toLocaleString()}</p>` : ''}
                            ${query.response ? `<p class="card-text">Response: ${query.response}</p>` : ''}
                        </div>
                    </div>
                `;
                queriesContainer.appendChild(card);
            });
        })
        .catch(error => {
            console.error('Error fetching queries:', error.message);
            alert('Error fetching queries. Please try again later.');
        });
    };

    addQueryForm.addEventListener('submit', function (event) {
        event.preventDefault();
        const formData = new FormData(addQueryForm);
        const queryData = {
            subject: formData.get('subject'),
            message: formData.get('message')
        };
        const token = localStorage.getItem("jwtToken");

        fetch('http://localhost:5071/api/Queries/add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
            body: JSON.stringify(queryData),
        })
        .then(response => {
            if (!response.ok) {
                throw response;
            }
            return response.json();
        })
        .then(() => {
            addQueryForm.reset();
            const addQueryModal = bootstrap.Modal.getInstance(document.getElementById('addQueryModal'));
            addQueryModal.hide();
            fetchQueries();
        })
        .catch(async error => {
            let errorMessage = 'Error adding query.';
            if (error.json) {
                const errorResponse = await error.json();
                errorMessage = errorResponse.message || errorMessage;
            }
            console.error('Error:', errorMessage);
            alert(errorMessage);
        });
    });

    fetchQueries();
});
