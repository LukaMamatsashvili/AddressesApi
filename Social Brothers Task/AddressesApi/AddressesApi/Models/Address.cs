using System;
using System.Collections.Generic;

namespace AddressesApi.Models
{
    public partial class Address
    {
        public long Id { get; set; }
        public string Street { get; set; } = null!;
        public string HouseNumber { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
    }
}
