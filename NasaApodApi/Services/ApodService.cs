using MongoDB.Driver;
using NasaApodApi.Model;

namespace NasaApodApi.Services
{
    public class ApodService : IApodService
    {
        private readonly IMongoCollection<Apod> _apodCollection;
        private readonly HttpClient _httpClient;
        private readonly string _nasaApiKey;

        public ApodService(IMongoClient mongoClient, IConfiguration configuration)
        {
            _apodCollection = mongoClient.GetDatabase(configuration["MongoDb:DbName"]).GetCollection<Apod>(configuration["MongoDb:ApodCollectionName"]);
            _httpClient = new HttpClient();
            _nasaApiKey = configuration["NASA:ApiKey"];
        }

        public async Task<Apod> GetApodDataAsync(DateTime? date = null)
        {
            // If a date is provided, search the database for data for that date
            if (date.HasValue)
            {
                var existingData = await _apodCollection.Find(apod => apod.Date == date.Value.ToString("yyyy-MM-dd")).FirstOrDefaultAsync();
                if (existingData != null)
                {
                    return existingData; // Return existing data if found
                }

                // If no data is found for the specified date, call the API for that specific date
                var response = await _httpClient.GetStringAsync($"https://api.nasa.gov/planetary/apod?api_key={_nasaApiKey}&date={date.Value.ToString("yyyy-MM-dd")}");
                var apodData = Newtonsoft.Json.JsonConvert.DeserializeObject<Apod>(response);

                apodData.Copyright = apodData.Copyright?.Replace("\n", "").Trim();

                // Store the data in the database
                await _apodCollection.InsertOneAsync(apodData);

                return apodData; // Return the new APOD data for the specific date
            }

            // If no data is found for the specified date, or if no date is provided, retrieve data for the current day
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var todayData = await _apodCollection.Find(apod => apod.Date == today).FirstOrDefaultAsync();

            // If we do not have data for today, then call the API
            if (todayData == null)
            {
                // Retrieve data from NASA's API
                var response = await _httpClient.GetStringAsync($"https://api.nasa.gov/planetary/apod?api_key={_nasaApiKey}");
                var apodData = Newtonsoft.Json.JsonConvert.DeserializeObject<Apod>(response);

                apodData.Copyright = apodData.Copyright?.Replace("\n", "").Trim();

                // Store the data in the database
                await _apodCollection.InsertOneAsync(apodData);

                return apodData; // Return the new APOD data
            }

            // If today's data exists in the database, return that data
            return todayData;
        }

        public async Task<List<Apod>> GetApodDataByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Ensure the start date is less than or equal to the end date
            if(startDate > endDate)
            {
                throw new ArgumentException("Start date must be less than or equal to end date.");
            }

            // Convert dates to string format (YYYY-MM-DD) for querying
            var startDateString = startDate.ToString("yyyy-MM-dd");
            var endDateString = endDate.ToString("yyyy-MM-dd");

            // Query the database for APOD entries within the date range
            var apodList = await _apodCollection
                .Find(apod => apod.Date.CompareTo(startDateString) >= 0 && apod.Date.CompareTo(endDateString) <= 0)
                .ToListAsync();

            // Calculate the total number of days in the range
            var totalDays = (endDate - startDate).Days + 1; // Include both start and end date

            // If no data is found for the specified date range, request data from the NASA APOD API
            if (apodList.Count < totalDays)
            {
                var httpClient = new HttpClient();
                var apiKey = _nasaApiKey; // Assume you have an API key available
                var apiUrl = "https://api.nasa.gov/planetary/apod";

                // List to hold newly fetched APOD entries
                var newApodEntries = new List<Apod>();

                // Iterate through the date range and fetch data for each date
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var dateString = date.ToString("yyyy-MM-dd");

                    // Check if the data for this date already exists in the database
                    var existingData = await _apodCollection.Find(apod => apod.Date == dateString).FirstOrDefaultAsync();
                    if (existingData != null)
                    {
                        // If it exists, continue to the next date
                        newApodEntries.Add(existingData); // Optionally, you can still add it to the new list
                        continue;
                    }

                    var response = await httpClient.GetStringAsync($"{apiUrl}?api_key={apiKey}&date={dateString}");
                    var apodData = Newtonsoft.Json.JsonConvert.DeserializeObject<Apod>(response);

                    // Clean up the copyright field
                    apodData.Copyright = apodData.Copyright?.Replace("\n", "").Trim();

                    // Store the fetched data in the new list and the database
                    newApodEntries.Add(apodData);
                    await _apodCollection.InsertOneAsync(apodData);
                }

                // Return the newly fetched APOD entries
                return newApodEntries.OrderByDescending(apod => apod.Date).ToList();
            }

            return apodList.OrderByDescending(apod => apod.Date).ToList();
        }
    }
}
