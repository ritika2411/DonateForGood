using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DonateForGood.Models
{
    public class NGORegistration 
    {

        public int UserId { get; set; }
        public string Description { get; set; }
        public string WebSiteURL { get; set; }
        public string ContactNumber1 { get; set; }
        public string ContactNumber2 { get; set; }
        public byte[] Photo { get; set; }
        public string ContactPersonName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string CityName { get; set; }

        public string CountryName { get; set; }

        public string StateName { get; set; }

        public string LocationName { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<int> StateId { get; set; }
        public Nullable<int> CountryId { get; set; }
        public string Zipcode { get; set; }
        public string Landmark { get; set; }

        public int UserTypeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Nullable<int> SecurityQuestionID { get; set; }
        public string SecurityAnswer { get; set; }

    }
}