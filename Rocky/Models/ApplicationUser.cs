using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Tracking Number")]
        public string TrackingId { get; set; }
        [Display(Name = "Profile Picture")]
        public byte[] ProfilePicture { get; set; }

        [Display(Name = "Order")]
        public int? OrderId { get; set; }
        public virtual Order Order { get; set; }

        [Display(Name = "Shipping Details")]
        public int? ShipOrderId { get; set; }
        public virtual ShipOrder ShipOrder { get; set; }
    }
}
