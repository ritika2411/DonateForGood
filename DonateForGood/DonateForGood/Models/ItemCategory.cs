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
    
    public partial class ItemCategory
    {
        public ItemCategory()
        {
            this.ItemPosts = new HashSet<ItemPost>();
        }
    
        public int ItemCategoryId { get; set; }
        public string ItemCategoryName { get; set; }
    
        public virtual ICollection<ItemPost> ItemPosts { get; set; }
    }
}
