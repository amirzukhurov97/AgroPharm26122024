document.getElementById('editProductForm').addEventListener('submit', function (event) {
    event.preventDefault(); // Останавливаем стандартную отправку формы

    const form = event.target;
    const formData = new FormData(form);
    const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
    const controllerName = window.location.pathname.split('/')[1];
    const indexUrl = `/${controllerName}/Index`; // Формируем путь

    fetch(form.action, {
        method: 'POST',
        headers: {
            'RequestVerificationToken': csrfToken
        },
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                Swal.fire({
                    title: 'Успешно!',
                    text: data.message,
                    icon: 'success'
                }).then(() => {
                    window.location.href = indexUrl
                });
            } else {
                Swal.fire({
                    title: 'Ошибка!',
                    text: data.message,
                    icon: 'error'
                });
            }
        })
        .catch(error => {
            Swal.fire({
                title: 'Ошибка!',
                text: `Произошла ошибка: ${error.message}`,
                icon: 'error'
            });
        });
});