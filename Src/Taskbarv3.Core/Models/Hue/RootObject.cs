using System.Collections.Generic;

namespace Taskbarv3.Core.Models.Hue
{
    public class RootObject
    {
        public List<RootObject> ListRoot { get; set; }
        public Error Error { get; set; }
        public State State { get; set; }
        public Swupdate Swupdate { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Modelid { get; set; }
        public string Manufacturername { get; set; }
        public string Uniqueid { get; set; }
        public string Swversion { get; set; }
        public string Swconfigid { get; set; }
        public string Productid { get; set; }
    }
}
