document.addEventListener("DOMContentLoaded", function () {
	const priceTJSInput = document.getElementById("priceTJS");
	const priceUSDInput = document.getElementById("priceUSD");
	const quantity = document.getElementById("quantity");
	const totalTJSInput = document.getElementById("sumTJS");
	const totalUSDInput = document.getElementById("sumUSD");

	// Установите курс валюты (пример: 1 USD = 11 TJS)
	const exchangeRate = 11;

	function calculateSum() {
		// Получить введенные значения
		const priceTJS = parseFloat(priceTJSInput.value) || 0;
		const priceUSD = parseFloat(priceUSDInput.value) || 0;
		const BuyQuantity = parseFloat(quantity.value) || 0;

		// Рассчитать суммы
		const totalTJS = priceTJS * BuyQuantity;
		const totalUSD = priceUSD * BuyQuantity;

		// Отобразить результаты
		totalTJSInput.value = totalTJS.toFixed(2);
		totalUSDInput.value = totalUSD.toFixed(2);
	}
	function tjstousd() {
		const priceTJS = parseFloat(priceTJSInput.value);

		const totalUSD = priceTJS / exchangeRate;

		priceUSDInput.value = totalUSD.toFixed(2);
	}
	function usdtotjs() {
		const priceUSD = parseFloat(priceUSDInput.value);

		const totalTJS = priceUSD * exchangeRate;

		priceTJSInput.value = totalTJS.toFixed(2);
	}

	// Добавить обработчики событий
	priceTJSInput.addEventListener("input", tjstousd);
	priceUSDInput.addEventListener("input", usdtotjs);
	priceTJSInput.addEventListener("input", calculateSum);
	priceUSDInput.addEventListener("input", calculateSum);
	quantity.addEventListener("input", calculateSum);

});