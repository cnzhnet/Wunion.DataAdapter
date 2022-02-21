using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Wunion.DataAdapter.CodeFirstDemo.Controllers
{
    /// <summary>
    /// 自动创建的 webapi 控制器.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 我肏，用于测试的 Get 请求 api.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// 这是一个在控制器中自定义的方法，传入什么就返回什么.
        /// </summary>
        /// <param name="code">传入代码.</param>
        /// <param name="message">代码所对应的消息.</param>
        /// <returns>返回什么值?</returns>
        [Route("/[controller]/CustomMethod"), HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public Dictionary<string, object> CustomMethod([FromForm] int code, [FromForm] string message)
        {
            return new Dictionary<string, object> {
                { "code", code }, { "message", message }
            };
        }

        /// <summary>
        /// 用于将指定的整数与 2 求模测试.
        /// </summary>
        /// <param name="value">测试值.</param>
        /// <returns></returns>
        /// <response code="200">value 为 2 的倍数.</response>
        /// <response code="510">value 不是 2 的倍数.</response>
        [Route("/[controller]/IsModTwo"), HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status510NotExtended)]
        public IActionResult IsModTwo([FromForm] int value)
        {
            if ((value % 2) == 0)
                return StatusCode(StatusCodes.Status200OK, value);
            return StatusCode(StatusCodes.Status510NotExtended, value);
        }
    }
}
