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
    
    public partial class Review
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string ReviewTitle { get; set; }
        public string ReviewComment_ { get; set; }
        public Nullable<int> Rating_ { get; set; }
        public string UserName { get; set; }
    
        public virtual User User { get; set; }
    }
}
