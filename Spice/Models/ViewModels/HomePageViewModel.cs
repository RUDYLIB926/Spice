using System.Collections.Generic;

namespace Spice.Models.ViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<MenuItem> MenuItems { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Coupon> Coupons { get; set; }
    }
}
