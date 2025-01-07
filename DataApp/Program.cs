//Import essential libraries
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PersonDataApp
{
    // HttpClient is a class for sending HTTP request and receiving responses
    private static readonly HttpClient client = new HttpClient();

    // Defining variables with API url where from data is requested
    private const string apiUrlWeather = "http://api.openweathermap.org/data/2.5/weather";
    private const string apiUrlPerson = "https://randomuser.me/api/";

    // Define API-key to get access to data in OpenWeatherMap
    private const string apiKeyWeather = "xxxxxxxxxxxxxxxxxxxx"; 
    
    // Variable to contain city for weather, value is set in GetPersonDataAsync() method.
    private static string cityFromPerson = "";

    // Turku city coordinates
    private const double turkuLongitude = 22.264824;
    private const double turkuLatitude = 60.454510;

    // Persons location city coordinates
    private static double locLongitude;
    private static double locLatitude;

    // Method to use to separate text blocks
    public static void WriteDashes()
    {
        Console.WriteLine("-------------------");
    }

    // Method to fetch random persons data from online database
    public static async Task GetPersonDataAsync()
    {
        try
        {
            // GET request to RandomUser.me API for random users data
            var responseP = await client.GetStringAsync(apiUrlPerson);

            // Deserialize the JSON response and set into a dynamic variable 
            dynamic personData = JsonConvert.DeserializeObject(responseP);

            // Extracting data of a random person
            string firstName = personData.results[0].name.first;
            string lastName = personData.results[0].name.last;
            string country = personData.results[0].location.country;
            locLatitude = personData.results[0].location.coordinates.latitude;
            locLongitude = personData.results[0].location.coordinates.longitude;
            cityFromPerson = personData.results[0].location.city;

            // Display the random person data in console
            Console.WriteLine($"Random user information:");
            Console.WriteLine($"");
            Console.WriteLine($"Name: {firstName} {lastName}");
            Console.WriteLine($"Country: {country}");
            Console.WriteLine($"City: {cityFromPerson}");
            Console.WriteLine($"");

        }
        catch (Exception ex)
        {
            // In case of error in fetching person data write a message.
            Console.WriteLine("Error in fetching person data: " + ex.Message);
        }
    }
    // Method to fetch weather data according to city defined in GetPersonDataAsync()-method.
    public static async Task GetWeatherDataAsync()
    {
        try
        {
             // Construction of Weather API url
            string weatherUrl = $"{apiUrlWeather}?q={cityFromPerson}&appid={apiKeyWeather}&units=metric";

            // GET request to OpenWeatherMap for weather data
            var responseW = await client.GetStringAsync(weatherUrl);

            // Deserialize the JSON response and set into a dynamic variable 
            dynamic weatherData = JsonConvert.DeserializeObject(responseW);

            // Setting weather data to variables
            string cityW = cityFromPerson; //cityFromPerson value is given in GetPersonDataAsync()-method
            string report = weatherData.weather[0].description;
            double temperature = weatherData.main.temp;
            double windSpeed = weatherData.wind.speed;

            // Display the weather data in console 
            Console.WriteLine($"Weather in {cityW}:");
            Console.WriteLine($"");
            Console.WriteLine($"Report: {report}");
            Console.WriteLine($"Temperature: {temperature} °C");
            Console.WriteLine($"Wind Speed: {windSpeed} m/s");
            Console.WriteLine($"");

        }
        catch (Exception ex)
        {
            // In case of error in fetching weather data write a message
            Console.WriteLine("Error in fetching weather data: " + ex.Message);
        }
    }
   

    // Method to convert degrees to radians for the calculations
    public static double DegreesToRadians(double degrees)
    {
        double radians = (Math.PI / 180) * degrees;
        return (radians);
    }

    // Method to calculate distance between Turku and location city of person
    public static void CalculateDistance()
    {
        Console.WriteLine($"Distance from {cityFromPerson} to Turku:");
        Console.WriteLine("");

        // Average earth radius in km
        int earthRadius = 6371;

        // Degree to radian conversions
        double turkuLatToRad = DegreesToRadians(turkuLatitude);
        double turkuLonToRad = DegreesToRadians(turkuLongitude);
        double locLatToRad = DegreesToRadians(locLatitude);
        double locLonToRad = DegreesToRadians(locLongitude);

        // Latitude and longitude differences
        double diffLat = locLatToRad - turkuLatToRad;
        double diffLon = locLonToRad - turkuLonToRad;

        // Using Haversine formula to calculate distance between two points in coordinate system
        // The roundness of the earth is taken account
        double a = Math.Sin(diffLat / 2) * Math.Sin(diffLat / 2) +
                   Math.Cos(turkuLatToRad) * Math.Cos(locLatToRad) *
                   Math.Sin(diffLon / 2) * Math.Sin(diffLon / 2);
        double b = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        // Rounds the distance to nearest integer
        double distance = Math.Round(earthRadius * b);

        Console.WriteLine($"{distance} kilometers");
        Console.WriteLine("");

    }


    // Main method to execute the data fetching methods
    public static async Task Main(string[] args)
    {
        // Display startup message in console
        Console.WriteLine("Start data fetching:");
        WriteDashes();

        // Call previously defined methods for fetching person and weather data
        await GetPersonDataAsync();
        WriteDashes();
        await GetWeatherDataAsync();
        WriteDashes();

        // Call method to calculate distance
        CalculateDistance();
        WriteDashes();

        // Display ending message
        Console.WriteLine("Press a button to exit the application");
        Console.ReadKey();

    }
}
