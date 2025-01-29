document.getElementById('getReport').addEventListener('submit', function (event) {
    event.preventDefault(); // Останавливаем стандартную отправку формы

    const form = event.target;
    const formData = new FormData(form);
    const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]').value;

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
                    window.location.href = "Report"
                });
            } else if (data.warning) {
                Swal.fire({
                    title: 'Внимание!',
                    text: data.message,
                    icon: 'warning'
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