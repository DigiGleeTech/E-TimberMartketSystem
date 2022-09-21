using System;
using System.Collections.Generic;

namespace Rocky.Models.ViewModels
{
    public class OrderShippingDetailsVM
    {
        public OrderShippingDetailsVM()
        {
            OrderList = new List<Order>();
            ShippingDetailsList = new List<ShippingDetails>();

        }
        
        public Order Orders { get; set; }
        public ShippingDetails ShippingDetail { get; set; }
        public IEnumerable<Order> OrderList { get; set; }
        public IEnumerable<ShippingDetails> ShippingDetailsList { get; set; }

        
    }
}
