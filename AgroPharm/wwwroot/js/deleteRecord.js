function deleteItem(item) {
    const controllerName = window.location.pathname.split('/')[1]; // Получаем имя контроллера из URL
    const deleteUrl = `/${controllerName}/Delete?id=${item.id}`; // Формируем путь
    Swal.fire({
        title: 'Удалить?',
        text: `Этот запись будет удалён с базы данных!`,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        cancelButtonText: 'Отмена',
        confirmButtonText: 'Да, удалить!'
    }).then((result) => {
        if (result.isConfirmed) {
            // Отправляем запрос на удаление
            fetch(deleteUrl, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                }
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        Swal.fire({
                            title: 'Успешно!',
                            text: data.message,
                            icon: 'success'
                        }).then(() => {
                            window.location.reload(); // Перезагружаем страницу
                        });
                    } else if (data.warning) {
                        Swal.fire({
                            title: 'Внимание!',
                            text: data.message,
                            icon: 'warning'
                        });
                    }
                    else {
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
        }
    });
}
