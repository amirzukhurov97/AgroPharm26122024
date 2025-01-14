document.addEventListener("DOMContentLoaded", function () {

    console.log("мы здесь");
    fetch(`/Home/Index`)
    .then(response => response.json())
    .then(data => {
        if (data.success) {            
            renderTable(data.data); // Отображаем данные
            console.log("DATA", data.data);
            Swal.fire({
                position: 'top-end',
                icon: 'success',
                title: 'Данные загружены успешно',
                showConfirmButton: false,
                timer: 1500
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

function renderTable(items) {
    const tableBody = document.querySelector('#example4 tbody');
    console.log("Целевой элемент:", tableBody);
    tableBody.innerHTML = ""; // Очистка текущей таблицы
    items.forEach(item => {
        const row = document.createElement('tr');
        row.className = 'odd gradeX';
        row.innerHTML = `
            <td class="text-center">${item.id}</td>
            <td class="text-center">${item.productName}</td>
            <td class="text-center">${item.obemProducts}</td>
        `;
        console.log("Добавляемая строка:", row.innerHTML);
        tableBody.appendChild(row);
    });
}
