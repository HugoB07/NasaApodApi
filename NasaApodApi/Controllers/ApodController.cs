using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using NasaApodApi.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NasaApodApi.Controllers
{
    [ApiController]
    [Route("apod")]
    public class ApodController : ControllerBase
    {
        private readonly IApodService _apodService;

        public ApodController(IApodService apodService)
        {
            _apodService = apodService;
        }

        [HttpGet("today")]
        public async Task<IActionResult> GetToday()
        {
            var apod = await _apodService.GetApodDataAsync();
            if (apod == null)
            {
                return NotFound("No APOD data found for the specified date.");
            }
            return Ok(apod);
        }

        [HttpGet("by_date")]
        public async Task<IActionResult> GetByDate([FromQuery] string date = null)
        {
            DateTime? parsedDate = null;
            if(!string.IsNullOrEmpty(date))
            {
                if(DateTime.TryParse(date, out var tempDate))
                {
                    parsedDate = tempDate;
                }
                else
                {
                    return BadRequest("Date format is not valid, Use YYYY-MM-DD");
                }
            }

            var apod = await _apodService.GetApodDataAsync(parsedDate);
            if(apod == null)
            {
                return NotFound("No APOD data found for the specified date.");
            }
            return Ok(apod);
        }

        [HttpGet("by_range_date")]
        public async Task<IActionResult> GetByRangeDate([FromQuery] string startDate, [FromQuery] string endDate)
        {
            DateTime startParsedDate = DateTime.Now;
            DateTime endParsedDate = DateTime.Now;

            if (!string.IsNullOrEmpty(startDate))
            {
                if (DateTime.TryParse(startDate, out var tempDate))
                {
                    startParsedDate = tempDate;
                }
                else
                {
                    return BadRequest("Date format is not valid, Use YYYY-MM-DD");
                }
            }

            if (!string.IsNullOrEmpty(endDate))
            {
                if (DateTime.TryParse(endDate, out var tempDate))
                {
                    endParsedDate = tempDate;
                }
                else
                {
                    return BadRequest("Date format is not valid, Use YYYY-MM-DD");
                }
            }

            var apods = await _apodService.GetApodDataByDateRangeAsync(startParsedDate, endParsedDate);
            if (apods == null)
            {
                return NotFound("No APODs data found for the specified dates.");
            }
            return Ok(apods);
        }
    }
}
