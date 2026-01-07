using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FxRateTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CimbController : ControllerBase
    {
        private readonly ILogger<CimbController> _logger;

        public CimbController(ILogger<CimbController> logger)
        {
            _logger = logger;
        }

        [HttpGet("sgd/to/myr")]
        public async Task<double> SgdToMyr()
        {
            double result = 0;

            string url = "https://www.cimbclicks.com.sg/sgd-to-myr-business";

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/141.0.0.0 Safari/537.36");
            string html = await httpClient.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            string ns = "";
            var match = Regex.Match(html, @"""(ns_.+?_)""");

            if (match.Success)
            {
                ns = match.Groups[1].Value;
            }

            var id = ns + "rateList";
            var inputNode = doc.DocumentNode.SelectSingleNode($"//input[@id='{id}']");

            if (inputNode != null)
            {
                string value = inputNode.GetAttributeValue("value", "");
                value = value.Substring(1, value.Length - 2);
                result = Convert.ToDouble(value);
                Console.WriteLine(result);

                //6556266381
                var bot = new TelegramBotClient("8434144452:AAERt3iv3-xGjhKG8qYp7i8SYmOkxo5nzY4");
                var me = await bot.GetMe();
                Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.Username}.");
                await bot.SendMessage("6556266381", "Current CIMB rate of SGD to MYR is " + result + ".");
            }
            else
            {
                Console.WriteLine("Input element not found.");
            }

            return result;
        }
    }
}