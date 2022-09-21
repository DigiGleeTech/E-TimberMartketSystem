using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Rocky.Models
{
    public class ShipOrder
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }

        [Display(Name = "PostCode")]
        [Required]
        public int PostCode { get; set; }

        [Required]
        [Display(Name = "House Number/Building")]
        public string HouseNoOrBuilding { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required]
        [Display(Name = "L.G.A")]
        public string Lga { get; set; }

        [Required]
        [Display(Name = "Shipping Price")]
        public decimal ShippingPrice { get; set; }

        public string TrackingId { get; set; }

        [Display(Name = "Order")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

    }
}
