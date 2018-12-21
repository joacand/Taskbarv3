using System.Collections.Generic;

namespace Taskbarv3.Core.Models.Hue
{
    public class State
    {
        public bool On { get; set; }
        public int Bri { get; set; }
        public int Hue { get; set; }
        public int Sat { get; set; }
        public string Effect { get; set; }
        public List<double> Xy { get; set; }
        public int Ct { get; set; }
        public string Alert { get; set; }
        public string Colormode { get; set; }
        public bool Reachable { get; set; }
    }
}
