using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM()
        {
            ProductList = new List<Product>();
            ShipOrder = new ShipOrder();
            ShippingList = new List<ShipOrder>();
        }

        public ApplicationUser ApplicationUser { get; set; }
        public Order Order { get; set; }
        public CreditCard CreditCard { get; set; }
        public ShipOrder ShipOrder { get; set; }
        public ShippingDetails ShippingDetails { get; set; }
        public IList<Product> ProductList { get; set; }
        public IList<ShipOrder> ShippingList { get; set; }

    }
}
