using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DonateForGood.Models
{
    public class KudosList
    {

        public int KudosId { get; set; }
        public string Comment { get; set; }
        public byte[] Photo { get; set; }
        public int NGOUserId { get; set; }
        public string FirstName { get; set; }
    }
}