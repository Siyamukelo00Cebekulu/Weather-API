async function getWeather() {
  const city = document.getElementById("cityInput").value;
  const resultDiv = document.getElementById("result");

  if (!city) {
    resultDiv.innerHTML = "Please enter a city.";
    return;
  }

  try {
    const response = await fetch(`http://localhost:5244/api/weather/${city}`);

    if (!response.ok) {
      throw new Error("Failed to fetch weather");
    }

    const data = await response.json();

    // If your API returns raw OpenWeather JSON:
    const temp = data.main.temp;
    const weather = data.weather[0].description;
    const humidity = data.main.humidity;

    resultDiv.innerHTML = `
            <h3>${city}</h3>
            <p>Temperature: ${temp}°C</p>
            <p>Condition: ${weather}</p>
            <p>Humidity: ${humidity}%</p>
        `;
  } catch (error) {
    resultDiv.innerHTML = "Error fetching weather data.";
    console.error(error);
  }
}
