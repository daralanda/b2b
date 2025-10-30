using System.Xml.Serialization;

namespace B2b.Plugin.Dto
{
    public class CurrencyExchange
    {

        [XmlElement(ElementName = "CurrencyName")]
        public string CurrencyName { get; set; }

        [XmlElement(ElementName = "ForexBuying")]
        public string ForexBuying { get; set; }

        [XmlElement(ElementName = "ForexSelling")]
        public string ForexSelling { get; set; }
        [XmlAttribute(AttributeName = "CurrencyCode")]
        public string CurrencyCode { get; set; }

    }

    [XmlRoot(ElementName = "Tarih_Date")]
    public class TarihDate
    {

        [XmlElement(ElementName = "Currency")]
        public List<CurrencyExchange> CurrencyExchange { get; set; }

        [XmlAttribute(AttributeName = "Tarih")]
        public string Tarih { get; set; }

    }

}