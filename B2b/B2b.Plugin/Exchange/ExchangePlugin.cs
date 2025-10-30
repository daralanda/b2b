using B2b.Plugin.Dto;
using System.Xml.Serialization;

namespace B2b.Plugin.Exchange
{
    public static class ExchangePlugin
    {
        private const string DovizUrl = "https://www.tcmb.gov.tr/kurlar/today.xml";

        public static List<CurrencyExchange> GetExchanges()
        {
            var result = new List<CurrencyExchange>();
            try
            {
                string xmlContent;
                using (HttpClient client = new())
                {
                    xmlContent =  client.GetStringAsync(DovizUrl).Result;
                }
                XmlSerializer serializer = new(typeof(TarihDate));
                TarihDate dovizBulteni;
                using (StringReader reader = new StringReader(xmlContent))
                {
                    dovizBulteni = (TarihDate)serializer.Deserialize(reader);
                }
                result = dovizBulteni.CurrencyExchange.Where(x=>x.CurrencyCode=="USD" ||x.CurrencyCode== "EUR").ToList();

            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return result;
        }
    }
}
