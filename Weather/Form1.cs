using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Weather
{
    public partial class Form1 : Form
    {
        private HttpClient _httpClient;

        public Form1()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        public string apiKey = "fb2b8cc341efaad51e55e92591ec0f59";
        public string[] Directions = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };

        public async Task<WeatherData> GetWeatherData<WeatherData>(string url_weather)
        {
            var result_weather = await _httpClient.GetAsync(url_weather);
            if (result_weather.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }
            result_weather.EnsureSuccessStatusCode();
            return await result_weather.Content.ReadFromJsonAsync<WeatherData>();
        }
        public async Task<ForecastData> GetForecastData<ForecastData>(string url_forecast)
        {
            var result_forecast = await _httpClient.GetAsync(url_forecast);
            if (result_forecast.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }
            result_forecast.EnsureSuccessStatusCode();
            return await result_forecast.Content.ReadFromJsonAsync<ForecastData>();
        }

        private void ClearTextBox(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        DateTime convertTime(long seconds)
        {
            DateTime day = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime();
            day = day.AddSeconds(seconds).ToLocalTime();
            return day;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "Nitra";
            button1_Click(sender, e);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string cityName = textBox1.Text;

            try
            {
                string url_weather = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}&units=metric&lang=en";
                string url_forecast = $"https://api.openweathermap.org/data/2.5/forecast/daily?q={cityName}&cnt=5&appid={apiKey}&units=metric&lang=en";

                WeatherData weather;
                ForecastData forecast;

                try
                {
                    weather = await GetWeatherData<WeatherData>(url_weather);
                    forecast = await GetForecastData<ForecastData>(url_forecast);

                    if (weather != null)
                    {
                        WeatherData weatherData = weather;
                        ForecastData forecastData = forecast;

                        double wind_deg0 = weatherData.Wind.Deg;
                        wind_deg0 = wind_deg0 % 360;
                        int index0 = (int)Math.Floor(wind_deg0 / 22.5);
                        string lmao0 = Directions[index0 % Directions.Length];

                        double wind_deg1 = forecastData.List[1].Deg;
                        wind_deg1 = wind_deg1 % 360;
                        int index1 = (int)Math.Floor(wind_deg1 / 22.5);
                        string lmao1 = Directions[index1 % Directions.Length];

                        double wind_deg2 = forecastData.List[2].Deg;
                        wind_deg2 = wind_deg2 % 360;
                        int index2 = (int)Math.Floor(wind_deg2 / 22.5);
                        string lmao2 = Directions[index2 % Directions.Length];

                        double wind_deg3 = forecastData.List[3].Deg;
                        wind_deg3 = wind_deg3 % 360;
                        int index3 = (int)Math.Floor(wind_deg3 / 22.5);
                        string lmao3 = Directions[index3 % Directions.Length];

                        double wind_deg4 = forecastData.List[4].Deg;
                        wind_deg4 = wind_deg4 % 360;
                        int index4 = (int)Math.Floor(wind_deg4 / 22.5);
                        string lmao4 = Directions[index4 % Directions.Length];

                        label8.Text = cityName[0] + cityName.Substring(1).ToLower();
                        label100.Text = DateTime.Now.ToShortDateString();
                        label9.Text = weatherData.Main.Temp.ToString() + "°C";
                        label10.Text = weatherData.Main.Humidity.ToString() + "%";
                        label11.Text = weatherData.Wind.Speed.ToString() + "m/s";
                        label12.Text = lmao0;
                        label13.Text = weatherData.Main.Pressure.ToString() + "hPa";
                        label14.Text = weatherData.Clouds.All.ToString() + "%";
                        label15.Text = weatherData.Weather[0].Description;
                        pictureBox1.ImageLocation = $"http://openweathermap.org/img/wn/{weatherData.Weather[0].Icon}@2x.png";
                        label18.Text = convertTime(weatherData.Sys.Sunrise).ToShortTimeString();
                        label19.Text = convertTime(weatherData.Sys.Sunset).ToShortTimeString();

                        groupBox1.Text = convertTime(forecastData.List[1].dt).ToShortDateString();
                        label30.Text = forecastData.List[1].temp.day.ToString() + "°C";
                        label31.Text = forecastData.List[1].temp.night.ToString() + "°C";
                        label32.Text = forecastData.List[1].humidity.ToString() + "%";
                        label33.Text = forecastData.List[1].Speed.ToString() + "m/s";
                        label34.Text = lmao1;
                        label35.Text = forecastData.List[1].pressure.ToString() + "hPa";
                        label39.Text = forecastData.List[1].Clouds.ToString() + "%";
                        label37.Text = forecastData.List[1].Weather[0].Description;
                        label38.Text = convertTime(forecastData.List[1].sunrise).ToShortTimeString();
                        label36.Text = convertTime(forecastData.List[1].sunset).ToShortTimeString();
                        pictureBox2.ImageLocation = $"http://openweathermap.org/img/wn/{forecastData.List[1].Weather[0].Icon}@2x.png";

                        groupBox2.Text = convertTime(forecastData.List[2].dt).ToShortDateString();
                        label49.Text = forecastData.List[2].temp.day.ToString() + "°C";
                        label48.Text = forecastData.List[2].temp.night.ToString() + "°C";
                        label47.Text = forecastData.List[2].humidity.ToString() + "%";
                        label46.Text = forecastData.List[2].Speed.ToString() + "m/s";
                        label45.Text = lmao2;
                        label44.Text = forecastData.List[2].pressure.ToString() + "hPa";
                        label40.Text = forecastData.List[2].Clouds.ToString() + "%";
                        label42.Text = forecastData.List[2].Weather[0].Description;
                        label41.Text = convertTime(forecastData.List[2].sunrise).ToShortTimeString();
                        label43.Text = convertTime(forecastData.List[2].sunset).ToShortTimeString();
                        pictureBox3.ImageLocation = $"http://openweathermap.org/img/wn/{forecastData.List[2].Weather[0].Icon}@2x.png";

                        groupBox3.Text = convertTime(forecastData.List[3].dt).ToShortDateString();
                        label69.Text = forecastData.List[3].temp.day.ToString() + "°C";
                        label68.Text = forecastData.List[3].temp.night.ToString() + "°C";
                        label67.Text = forecastData.List[3].humidity.ToString() + "%";
                        label66.Text = forecastData.List[3].Speed.ToString() + "m/s";
                        label65.Text = lmao3;
                        label64.Text = forecastData.List[3].pressure.ToString() + "hPa";
                        label60.Text = forecastData.List[3].Clouds.ToString() + "%";
                        label62.Text = forecastData.List[3].Weather[0].Description;
                        label61.Text = convertTime(forecastData.List[3].sunrise).ToShortTimeString();
                        label63.Text = convertTime(forecastData.List[3].sunset).ToShortTimeString();
                        pictureBox4.ImageLocation = $"http://openweathermap.org/img/wn/{forecastData.List[3].Weather[0].Icon}@2x.png";

                        groupBox4.Text = convertTime(forecastData.List[4].dt).ToShortDateString();
                        label89.Text = forecastData.List[4].temp.day.ToString() + "°C";
                        label88.Text = forecastData.List[4].temp.night.ToString() + "°C";
                        label87.Text = forecastData.List[4].humidity.ToString() + "%";
                        label86.Text = forecastData.List[4].Speed.ToString() + "m/s";
                        label85.Text = lmao4;
                        label84.Text = forecastData.List[4].pressure.ToString() + "hPa";
                        label80.Text = forecastData.List[4].Clouds.ToString() + "%";
                        label82.Text = forecastData.List[4].Weather[0].Description;
                        label81.Text = convertTime(forecastData.List[4].sunrise).ToShortTimeString();
                        label83.Text = convertTime(forecastData.List[4].sunset).ToShortTimeString();
                        pictureBox5.ImageLocation = $"http://openweathermap.org/img/wn/{forecastData.List[4].Weather[0].Icon}@2x.png";

                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }

    class ForecastData
    {
        public List[] List { get; set; }
    }

    class List
    {
        public long dt { get; set; }
        public long sunrise { get; set; }
        public long sunset { get; set; }
        public temp temp { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public Weather[] Weather { get; set; }
        public double Speed { get; set; }
        public int Deg { get; set; }
        public double Gust { get; set; }
        public int Clouds { get; set; }
    }

    class temp
    {
        public double day { get; set; }
        public double night { get; set; }
    }


    class WeatherData
    {
        public Weather[] Weather { get; set; }
        public Main Main { get; set; }
        public Wind Wind { get; set; }
        public Clouds Clouds { get; set; }
        public Sys Sys { get; set; }
    }

    class Weather
    {
        public string Description { get; set; }
        public string Icon { get; set; }
    }

    class Main
    {
        public double Temp { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
    }

    class Wind
    {
        public double Speed { get; set; }
        public int Deg { get; set; }
    }

    class Clouds
    {
        public int All { get; set; }
    }

    class Sys
    {
        public long Sunrise { get; set; }
        public long Sunset { get; set; }
    }
}
