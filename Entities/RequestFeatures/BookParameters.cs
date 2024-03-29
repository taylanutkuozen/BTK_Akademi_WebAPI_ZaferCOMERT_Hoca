﻿namespace Entities.RequestFeatures
{
    public class BookParameters :RequestParameters
	{
        public uint MinPrice { get; set; }
        public uint MaxPrice { get; set; } = 350;
        public bool ValidPriceRange => MaxPrice > MinPrice;
        public string? SearchTerm { get; set; } /*string? boşta olabilir anlamına gelmektedir.*/
        public BookParameters()
        {
            OrderBy = "BookID";
        }
    }
}