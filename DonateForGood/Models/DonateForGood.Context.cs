﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DonateForGoodEntities : DbContext
    {
        public DonateForGoodEntities()
            : base("name=DonateForGoodEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<CommentsOnRecentActivity> CommentsOnRecentActivities { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<ItemCategory> ItemCategories { get; set; }
        public virtual DbSet<ItemCollectPreference> ItemCollectPreferences { get; set; }
        public virtual DbSet<ItemPost> ItemPosts { get; set; }
        public virtual DbSet<ItemStatu> ItemStatus { get; set; }
        public virtual DbSet<Kudo> Kudos { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<NGODetail> NGODetails { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
    }
}
