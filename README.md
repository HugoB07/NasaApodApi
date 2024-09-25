# NASA Astronomy Picture of the Day (APOD) API

This project is a .NET application that retrieves and stores data from NASA's Astronomy Picture of the Day (APOD) API. The application provides endpoints to access APOD data for specific dates or date ranges, with local storage in a MongoDB database to enhance performance and avoid duplicate requests.

## Table of Contents
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Setup](#setup)
  - [appsettings.json Configuration](#appsettingsjson-configuration)
- [API Endpoints](#api-endpoints)
  - [Get Today's APOD](#get-todays-apod)
  - [Get APOD by Date](#get-apod-by-date)
  - [Get APOD by Date Range](#get-apod-by-date-range)
- [Contributing](#contributing)
- [License](#license)

## Features
- Fetch APOD data for today, a specific date, or a range of dates.
- Store retrieved data locally in a MongoDB database.
- Avoid duplicate entries by checking the local database before making API requests.

## Technologies Used
- **C#**: The primary programming language for the application.
- **ASP.NET Core**: Framework for building the web API.
- **MongoDB**: NoSQL database for local storage.
- **HttpClient**: To make HTTP requests to NASA's APOD API.
- **Newtonsoft.Json**: For JSON serialization and deserialization.

## Setup

### appsettings.json Configuration

To configure the application, you will need to set up an `appsettings.json` file. Below is a sample configuration:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DbName": "nasa",
    "ApodCollectionName": "apod"
  },
  "NASA": {
    "ApiKey": "DEMO_KEY" // Replace with your actual API key from NASA
  }
}
```

1. **MongoDB Configuration**:
   - `ConnectionString`: Set the connection string for your MongoDB instance (default is `mongodb://localhost:27017`).
   - `DbName`: Specify the name of the database (e.g., `nasa`).
   - `ApodCollectionName`: Name of the collection to store APOD data (e.g., `apod`).

2. **NASA API Key**:
   - Replace `DEMO_KEY` with your actual NASA API key, which you can obtain from the [NASA API portal](https://api.nasa.gov).

## API Endpoints

### Get Today's APOD
- **Endpoint**: `GET /apod/today`
- **Description**: Retrieves the Astronomy Picture of the Day for today.
- **Response**: Returns the APOD data or a 404 error if not found.

### Get APOD by Date
- **Endpoint**: `GET /apod/by_date`
- **Query Parameter**: `date` (format: YYYY-MM-DD)
- **Description**: Retrieves the APOD data for a specified date.
- **Response**: Returns the APOD data or a 404 error if not found. Returns a 400 error if the date format is invalid.

### Get APOD by Date Range
- **Endpoint**: `GET /apod/by_range_date`
- **Query Parameters**: 
  - `startDate` (format: YYYY-MM-DD)
  - `endDate` (format: YYYY-MM-DD)
- **Description**: Retrieves APOD data for a specified range of dates.
- **Response**: Returns a list of APOD entries or a 404 error if not found. Returns a 400 error if the date formats are invalid.

## Contributing
Contributions are welcome! Please feel free to submit a pull request or open an issue if you find any bugs or have suggestions for improvements.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.