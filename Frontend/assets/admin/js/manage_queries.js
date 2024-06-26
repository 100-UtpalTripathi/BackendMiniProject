document.addEventListener('DOMContentLoaded', function () {
    const queriesContainer = document.getElementById('queriesContainer');
    const respondQueryForm = document.getElementById('respondQueryForm');

    const fetchQueries = () => {
        const token = localStorage.getItem("jwtToken");
        fetch('http://localhost:5071/api/Queries/get/all', {
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
                card.classList.add('col-md-4');
                card.innerHTML = 
                    `<div class="card">
                        <div class="card-body">
                            <h5 class="card-title">${query.subject}</h5>
                            <p class="card-text">Message: ${query.message}</p>
                            <button class="btn btn-respond" data-bs-toggle="modal" data-bs-target="#respondQueryModal" 
                                data-id="${query.id}">Respond</button>
                            <button class="btn btn-close" data-id="${query.id}"></button>
                        </div>
                    </div>`;
                queriesContainer.appendChild(card);
            });

            document.querySelectorAll('.btn-respond').forEach(button => {
                button.addEventListener('click', (event) => {
                    const queryId = event.target.getAttribute('data-id');
                    document.getElementById('respondQueryId').value = queryId;
                });
            });

            document.querySelectorAll('.btn-close').forEach(button => {
                button.addEventListener('click', (event) => {
                    const queryId = event.target.getAttribute('data-id');
                    const token = localStorage.getItem("jwtToken");
                    fetch(`http://localhost:5071/api/Queries/${queryId}/close`, {
                        method: 'PUT',
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
                    .then(() => fetchQueries())
                    .catch(async error => {
                        let errorMessage = 'Error closing query.';
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
            console.error('Error fetching queries:', error.message);
            alert('Error fetching queries. Please try again later.');
        });
    };

    respondQueryForm.addEventListener('submit', function (event) {
        event.preventDefault();
        const queryId = document.getElementById('respondQueryId').value;
        const formData = new FormData(respondQueryForm);
        const responseMessage = formData.get('response');
        const token = localStorage.getItem("jwtToken");

        fetch(`http://localhost:5071/api/Queries/${queryId}/respond`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`,
            },
            body: JSON.stringify(responseMessage),
        })
        .then(response => {
            if (!response.ok) {
                throw response;
            }
            return response.json();
        })
        .then(() => {
            respondQueryForm.reset();
            const respondQueryModal = bootstrap.Modal.getInstance(document.getElementById('respondQueryModal'));
            respondQueryModal.hide();
            fetchQueries();
        })
        .catch(async error => {
            let errorMessage = 'Error responding to query.';
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
