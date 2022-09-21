using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rocky.Models
{
    public class Location
    {
       
    }

    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Country")]
        public string CountryName { get; set; }
        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }
        public ICollection<State> States { get; set; }
    }

    public class State
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Sate")]
        public string StateName { get; set; }
        [Display(Name = "Country")]
        public int CountryId { get; set; }
        public Country Country { get; set; }
        public ICollection<Lga> Lgas { get; set; }

    }

    public class Lga
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "L.G.A")]
        public string LgaName { get; set; }

        [Display(Name = "State")]
        public int StateId { get; set; }
        public State State { get; set; }

    }
}
