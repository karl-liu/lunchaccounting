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
    
    public partial class FoodCardRecharge
    {
        public int FoodCardRecharge_id { get; set; }
        public int FoodCard_id { get; set; }
        public decimal Amount { get; set; }
        public int Operator { get; set; }
        public int OperateFor { get; set; }
    
        public virtual FoodCard FoodCardInfo { get; set; }
        public virtual User RechargeFor { get; set; }
        public virtual User OperatorInfo { get; set; }
    }
}
