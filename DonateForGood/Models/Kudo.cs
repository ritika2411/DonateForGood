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
    
    public partial class Kudo
    {
        public int KudosId { get; set; }
        public string Comment { get; set; }
        public byte[] Photo { get; set; }
        public int NGOUserId { get; set; }
        public Nullable<int> ItemPostId { get; set; }
    
        public virtual ItemPost ItemPost { get; set; }
        public virtual User User { get; set; }
    }
}
