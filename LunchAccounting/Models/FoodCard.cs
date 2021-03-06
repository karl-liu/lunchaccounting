//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LunchAccounting.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FoodCard
    {
        public FoodCard()
        {
            this.FoodCardRecharges = new HashSet<FoodCardRecharge>();
            this.UserAccounts = new HashSet<UserAccount>();
            this.UserConsumes = new HashSet<UserConsume>();
        }
    
        public int FoodCard_id { get; set; }
        public string FoodCardName { get; set; }
        public int Owner_id { get; set; }
        public decimal CurrentBalance { get; set; }
        public System.DateTime CreateDate { get; set; }
    
        public virtual User OwnerInfo { get; set; }
        public virtual ICollection<FoodCardRecharge> FoodCardRecharges { get; set; }
        public virtual ICollection<UserAccount> UserAccounts { get; set; }
        public virtual ICollection<UserConsume> UserConsumes { get; set; }
    }
}
