//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DonateForGood.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Location
    {
        public Location()
        {
            this.ItemPosts = new HashSet<ItemPost>();
            this.NGODetails = new HashSet<NGODetail>();
        }
    
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int CityId { get; set; }
    
        public virtual City City { get; set; }
        public virtual ICollection<ItemPost> ItemPosts { get; set; }
        public virtual ICollection<NGODetail> NGODetails { get; set; }
    }
}
