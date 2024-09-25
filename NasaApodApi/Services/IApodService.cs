using NasaApodApi.Model;

namespace NasaApodApi.Services
{
    public interface IApodService
    {
        /// <summary>
        /// Retrieves the Astronomy Picture of the Day (APOD) data for a specified date.
        /// If a date is provided, it first checks the database for existing data.
        /// If data for the specified date is not found, it retrieves data from NASA's API for that date.
        /// If no date is provided, it defaults to retrieving the current day's APOD data.
        /// The retrieved data is stored in the database for future access.
        /// </summary>
        /// <param name="date">An optional DateTime parameter to specify the date for which APOD data is requested.</param>
        /// <returns>A Task that represents the asynchronous operation, containing the APOD data for the specified date.</returns>
        Task<Apod> GetApodDataAsync(DateTime? date = null);

        /// <summary>
        /// Retrieves a list of Astronomy Picture of the Day (APOD) entries within a specified date range.
        /// The method first queries the database for APOD entries where the date falls between the provided start and end dates.
        /// If no entries are found in the database, it requests data from NASA's APOD API for each date in the range,
        /// storing the newly fetched entries in the database for future access.
        /// The start date must be less than or equal to the end date; otherwise, an exception is thrown.
        /// </summary>
        /// <param name="startDate">The start date of the range for which APOD data is requested.</param>
        /// <param name="endDate">The end date of the range for which APOD data is requested.</param>
        /// <returns>A Task that represents the asynchronous operation, containing a list of APOD entries within the specified date range.</returns>
        Task<List<Apod>> GetApodDataByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
