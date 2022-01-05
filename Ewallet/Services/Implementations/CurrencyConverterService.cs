using Ewallet.Models;
using Ewallet.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Ewallet.Services.Implementations
{
    public class CurrencyConverterService : ICurrencyConverterService
    {
        private readonly IConfiguration _configuration;
        private string _freeBaseURL;
        private string _premiumBaseURL;
        private string _apiKey;
        private string _apiVersion;

        private string _baseURL;
        private string _accessKey;

        public CurrencyConverterService(IConfiguration configuration)
        {
            _configuration = configuration;
            _freeBaseURL = _configuration.GetSection("CurrencyConverterSettings:FreeBaseURL").Value;
            _premiumBaseURL = _configuration.GetSection("CurrencyConverterSettings:PremiumBaseURL").Value;
            _apiKey = _configuration.GetSection("CurrencyConverterSettings:ApiKey").Value;
            _apiVersion = _configuration.GetSection("CurrencyConverterSettings:ApiVersion").Value;

            // Fixer.io
            _baseURL = _configuration.GetSection("FixerSettings:BaseURL").Value;
            _accessKey = _configuration.GetSection("FixerSettings:ApiKey").Value;
        }
        private decimal FetchSerializedData(string code)
        {
            string url;

            if (string.IsNullOrEmpty(_apiKey))
                url = $"{_freeBaseURL}/api/{_apiVersion}/convert?q={code}&compact=ultra&apiKey={_apiKey}";
            else
                url = $"{_freeBaseURL}/api/{_apiVersion}/convert?q={code}&compact=ultra&apiKey={_apiKey}";

            WebClient webClient = new WebClient();
            string jsonData = String.Empty;

            decimal conversionRate = 1.0m;

            try
            {
                jsonData = webClient.DownloadString(url);
                //var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, decimal>>>(jsonData);
                Dictionary<string, decimal> jsonObject = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(jsonData);
                decimal result = jsonObject[code];
                conversionRate = result;

            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }

            return conversionRate;
        }

        public decimal GetCurrencyExchange(string fromCurrency, string toCurrency)
        {
            string code = $"{fromCurrency}_{toCurrency}";
            decimal newRate = FetchSerializedData(code);
            return newRate;
        }

        
        public decimal FixerAPIConversion(string fromCurrency, string toCurrency, decimal amount)
        {
            string url = $"{_baseURL}latest?access_key={_accessKey}";
            WebClient webClient = new WebClient();
            string jsonData = String.Empty;

            decimal conversion = 1.0m;
            try
            {
                jsonData = webClient.DownloadString(url);
                //var jsonObject = JsonConvert.DeserializeObject<List<Conversion>>(jsonData);
                Dictionary<string, object> jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

                /*
                var fromResult = jsonObject.Where(x => x.Key == "rates").ToList();
                foreach (var rate in fromResult)
                {
                    if(rate[1] == fromCurrency)
                }
                var toResult = jsonObject.Where(x => x.Key == "rates").Select(y => y.Key == toCurrency);
                conversion = amount/fromResult * toResult["val"];

                */

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return conversion;
        }
    }
}
