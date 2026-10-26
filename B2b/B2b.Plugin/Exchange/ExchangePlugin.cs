using B2b.Plugin.Dto;
using System.Xml.Serialization;

namespace B2b.Plugin.Exchange
{
    public static class ExchangePlugin
    {
        private const string DovizUrl = "https://www.tcmb.gov.tr/kurlar/today.xml";
        // TCMB yanıt vermezse sayfa beklemesin diye kısa zaman aşımı; tek örnek soket tükenmesini önler
        private static readonly HttpClient _client = new() { Timeout = TimeSpan.FromSeconds(10) };

        public static List<CurrencyExchange> GetExchanges()
        {
            return GetBulletin()?.CurrencyExchange ?? new List<CurrencyExchange>();
        }

        // Günlük TCMB bülteni (yalnızca USD ve EUR), bülten tarihiyle birlikte; hata olursa null
        public static TarihDate GetBulletin()
        {
            try
            {
                string xmlContent = _client.GetStringAsync(DovizUrl).Result;
                XmlSerializer serializer = new(typeof(TarihDate));
                TarihDate dovizBulteni;
                using (StringReader reader = new StringReader(xmlContent))
                {
                    dovizBulteni = (TarihDate)serializer.Deserialize(reader);
                }
                dovizBulteni.CurrencyExchange = dovizBulteni.CurrencyExchange.Where(x => x.CurrencyCode == "USD" || x.CurrencyCode == "EUR").ToList();
                return dovizBulteni;
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                return null;
            }
        }
    }
}
