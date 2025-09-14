using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2b.Dal.Entity
{
    public class Slider
    {
        [Key]
        public int SliderId { get; set; }
        public string SliderName { get; set; }
        public string SliderUrl { get; set; }
        public int Queno { get; set; }

    }
}
