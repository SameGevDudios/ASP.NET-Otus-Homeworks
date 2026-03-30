import { useEffect, useState } from 'react'
import {type WeatherForecast } from './types.ts'
import { Config } from './common/config/config.ts';
import './App.css'


function App() {
  const [forecasts, setForecasts] = useState<WeatherForecast[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const API_URL = Config.getBaseUrl();

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const response = await fetch(`${API_URL}/WeatherForecast`);
        
        if (!response.ok) {
          throw new Error(`Ошибка: ${response.status}`);
        }
        
        const data: WeatherForecast[] = await response.json();
        setForecasts(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Неизвестная ошибка');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) return <div className="container">Загрузка данных...</div>;
  if (error) return <div className="container error">Ошибка при загрузке: {error}</div>;

  return (
    <div className="container">
      <h1>Прогноз погоды</h1>
      <table className="weather-table">
        <thead>
          <tr>
            <th>Дата</th>
            <th>Temp. (C)</th>
            <th>Temp. (F)</th>
            <th>Описание</th>
          </tr>
        </thead>
        <tbody>
          {forecasts.map((item, index) => (
            <tr key={index}>
              <td>{new Date(item.date).toLocaleDateString()}</td>
              <td>{item.temperatureC}°C</td>
              <td>{item.temperatureF}°F</td>
              <td>{item.summary}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

export default App