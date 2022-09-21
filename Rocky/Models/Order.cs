using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rocky.Models
{
    public class Order
    {
        public int Id { get; set; }
                
        public string Owner { get; set; }

        public decimal Amount { get; set; }

        public string TrackingId { get; set; }

        [Display(Name ="Order Date")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Delivery Date")]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }

        [Display(Name = "Order Status")]
        public bool OrderStatus { get; set; }    

        [Display(Name = "Product")]
        public string Product{ get; set; }

        public string User { get; set; }

        public virtual ICollection<ShipOrder> ShipOrders { get; set; }

        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public Order()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[20];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            var trackingNumber = finalString;

            TrackingId = trackingNumber;
        }    

    }


}
