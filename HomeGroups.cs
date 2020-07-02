using System.Xml.Serialization;

namespace ZaupHomeCommand
{
    public class HomeGroup
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public byte Wait { get; set; }
    }
}
