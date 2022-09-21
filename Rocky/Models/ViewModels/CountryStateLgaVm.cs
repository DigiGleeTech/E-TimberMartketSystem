using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using static Rocky.Models.Location;

namespace Rocky.Models.ViewModels
{
    public class CountryStateLgaVm
    {
        public Lga Lga { get; set; }
        public State state { get; set; }
        public Country Country { get; set; }
        public IEnumerable<SelectListItem> LgaSelectList { get; set; }
        public IEnumerable<SelectListItem> StateSelectList { get; set; }
        public IEnumerable<SelectListItem> CountrySelectList { get; set; }

    }
}
