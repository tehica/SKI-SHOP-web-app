using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Specifications
{
    public class ProductSpecParams
    {
        // max 50 products per page
        private const int MaxPageSize = 50;
        public int PageIndex { get; set; } = 1; // 1 by default

        private int _pageSize = 6;

        // get => method returns page size (_pageSize)
        // set => set page size what is equal value whatever is coming into this property
        //        and if this value is greater than Max page size then we are going to return to MaxPageSize
        //        or if its not, just return the value
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public int? BrandId { get; set; }
        public int? TypeId { get; set; }

        public string Sort { get; set; }

        // for search functionality
        private string _search;
        public string Search 
        {
            get => _search;
            set => _search = value.ToLower();
        }

    }
}
