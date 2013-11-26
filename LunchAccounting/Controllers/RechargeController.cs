using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LunchAccounting.Models;
using LunchAccounting.Helpers;

namespace LunchAccounting.Controllers
{    
    public class RechargeController : ControllerBase
    {

        //
        // GET: /Recharge/

        public ActionResult Index()
        {
            var foodcardrecharges = db.FoodCardRecharges
                .Include(f => f.OperatorInfo)
                .Include(f => f.RechargeFor)
                .Where(p => p.RechargeFor.UserName.Equals(HttpContext.User.Identity.Name, StringComparison.OrdinalIgnoreCase));
            return View(foodcardrecharges.ToList());
        }

        //
        // GET: /Recharge/Details/5

        public ActionResult Details(int id = 0)
        {
            FoodCardRecharge foodcardrecharge = db.FoodCardRecharges.Find(id);
            if (foodcardrecharge == null)
            {
                return HttpNotFound();
            }
            return View(foodcardrecharge);
        }

        //
        // GET: /Recharge/Create
        [RoleBasedAuthorize(Roles = "admin,cardowner")]
        public ActionResult Create()
        {
            ViewBag.OperateFor = new SelectList(db.Users, "User_id", "UserName");
            ViewBag.Operator = new SelectList(db.Users, "User_id", "UserName");
            ViewBag.FoodCard_id = new SelectList(db.FoodCards, "FoodCard_id", "FoodCardName");
            return View();
        }

        //
        // POST: /Recharge/Create
        [RoleBasedAuthorize(Roles = "admin,cardowner")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FoodCardRecharge foodcardrecharge)
        {
            if (ModelState.IsValid)
            {
                var foodCard = db.FoodCards.Find(foodcardrecharge.FoodCard_id);
                foodCard.CurrentBalance += foodcardrecharge.Amount;
                db.Entry(foodCard).State = EntityState.Modified;

                var rechargeFor = db.UserAccounts.Where(p => 
                    p.FoodCard_id == foodcardrecharge.FoodCard_id && 
                    p.User_id == foodcardrecharge.OperateFor);

                if (rechargeFor.Count() == 0)
                {
                    var useraccount = new UserAccount() {
                        User_id = foodcardrecharge.OperateFor,
                        FoodCard_id = foodcardrecharge.FoodCard_id,
                        Balance = foodcardrecharge.Amount
                    };

                    db.UserAccounts.Add(useraccount);
                }
                else if (rechargeFor.Count() == 1)
                {
                    var userAccount = rechargeFor.First();
                    userAccount.Balance += foodcardrecharge.Amount;
                    db.Entry(userAccount).State = EntityState.Modified;
                }
                else
                {
                    throw new Exception(string.Format("Invalid user account, user id: {0}, foodcard id: {1}.", 
                                                      foodcardrecharge.OperateFor, 
                                                      foodcardrecharge.FoodCard_id));
                }

                db.FoodCardRecharges.Add(foodcardrecharge);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.OperateFor = new SelectList(db.Users, "User_id", "UserName", foodcardrecharge.OperateFor);
            ViewBag.Operator = new SelectList(db.Users, "User_id", "UserName", foodcardrecharge.Operator);
            ViewBag.FoodCard_id = new SelectList(db.FoodCards, "FoodCard_id", "FoodCardName", foodcardrecharge.FoodCard_id);
            return View(foodcardrecharge);
        }

        //
        // GET: /Recharge/Edit/5
        [RoleBasedAuthorize(Roles = "admin,cardowner")]
        public ActionResult Edit(int id = 0)
        {
            FoodCardRecharge foodcardrecharge = db.FoodCardRecharges.Find(id);
            if (foodcardrecharge == null)
            {
                return HttpNotFound();
            }
            ViewBag.OperateFor = new SelectList(db.Users, "User_id", "UserName", foodcardrecharge.OperateFor);
            ViewBag.Operator = new SelectList(db.Users, "User_id", "UserName", foodcardrecharge.Operator);
            ViewBag.FoodCard_id = new SelectList(db.FoodCards, "FoodCard_id", "FoodCardName", foodcardrecharge.FoodCard_id);
            return View(foodcardrecharge);
        }

        //
        // POST: /Recharge/Edit/5
        [RoleBasedAuthorize(Roles = "admin,cardowner")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FoodCardRecharge foodcardrecharge)
        {
            if (ModelState.IsValid)
            {
                var originalRecharge = db.FoodCardRecharges.Find(foodcardrecharge.FoodCardRecharge_id);
                var originFoodcard = db.FoodCards.Find(originalRecharge.FoodCard_id);
                var rechargeFor = db.Users.Find(foodcardrecharge.OperateFor);

                var originUserAccount = db.UserAccounts.Where(p =>
                    p.FoodCard_id == originalRecharge.FoodCard_id &&
                    p.User_id == originalRecharge.OperateFor);

                var userAccount = db.UserAccounts.Where(p =>
                    p.FoodCard_id == foodcardrecharge.FoodCard_id &&
                    p.User_id == foodcardrecharge.OperateFor);

                if (originUserAccount.Count() == 0)
                {
                    var useraccount = new UserAccount()
                    {
                        User_id = originalRecharge.OperateFor,
                        FoodCard_id = originalRecharge.FoodCard_id,
                        Balance = -originalRecharge.Amount
                    };

                    db.UserAccounts.Add(useraccount);
                }

                if (userAccount.Count() == 0)
                {
                    var useraccount = new UserAccount()
                    {
                        User_id = foodcardrecharge.OperateFor,
                        FoodCard_id = foodcardrecharge.FoodCard_id,
                        Balance = -foodcardrecharge.Amount
                    };

                    db.UserAccounts.Add(useraccount);
                }

                var currentUserAccount = originUserAccount.First();
                var targetUserAccount = userAccount.First();

                // switch to another card
                if (originalRecharge.FoodCard_id != foodcardrecharge.FoodCard_id)
                {                    
                    originFoodcard.CurrentBalance -= originalRecharge.Amount;            

                    var newFoodcard = db.FoodCards.Find(foodcardrecharge.FoodCard_id);
                    newFoodcard.CurrentBalance += foodcardrecharge.Amount;
                    currentUserAccount.Balance -= originalRecharge.Amount;                    
                    targetUserAccount.Balance += foodcardrecharge.Amount;

                    db.Entry(currentUserAccount).State = EntityState.Modified;
                    db.Entry(targetUserAccount).State = EntityState.Modified;
                    db.Entry(originFoodcard).State = EntityState.Modified;
                    db.Entry(newFoodcard).State = EntityState.Modified;
                }
                else
                {
                    // same card, different amount
                    if (originalRecharge.Amount != foodcardrecharge.Amount)
                    { 
                        originFoodcard.CurrentBalance -= originalRecharge.Amount;
                        originFoodcard.CurrentBalance += foodcardrecharge.Amount;
                        db.Entry(originFoodcard).State = EntityState.Modified;

                        currentUserAccount.Balance -= originalRecharge.Amount;
                        currentUserAccount.Balance += foodcardrecharge.Amount;
                        db.Entry(rechargeFor).State = EntityState.Modified;
                    }
                }

                originalRecharge.Amount = foodcardrecharge.Amount;
                originalRecharge.OperateFor = foodcardrecharge.OperateFor;
                originalRecharge.Operator = foodcardrecharge.Operator;
                originalRecharge.FoodCard_id = foodcardrecharge.FoodCard_id;

                db.Entry(originalRecharge).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OperateFor = new SelectList(db.Users, "User_id", "UserName", foodcardrecharge.OperateFor);
            ViewBag.Operator = new SelectList(db.Users, "User_id", "UserName", foodcardrecharge.Operator);
            ViewBag.FoodCard_id = new SelectList(db.FoodCards, "FoodCard_id", "FoodCardName", foodcardrecharge.FoodCard_id);

            return View(foodcardrecharge);
        }

        //
        // GET: /Recharge/Delete/5
        [RoleBasedAuthorize(Roles = "admin,cardowner")]
        public ActionResult Delete(int id = 0)
        {
            FoodCardRecharge foodcardrecharge = db.FoodCardRecharges.Find(id);
            if (foodcardrecharge == null)
            {
                return HttpNotFound();
            }
            return View(foodcardrecharge);
        }

        //
        // POST: /Recharge/Delete/5
        [RoleBasedAuthorize(Roles = "admin,cardowner")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var foodcardrecharge = db.FoodCardRecharges.Find(id);
            var foodcard = db.FoodCards.Find(foodcardrecharge.FoodCard_id);
            foodcard.CurrentBalance -= foodcardrecharge.Amount;

            var rechargeFor = db.UserAccounts.Where(p =>
                    p.FoodCard_id == foodcardrecharge.FoodCard_id &&
                    p.User_id == foodcardrecharge.OperateFor);

            if (rechargeFor.Count() == 0)
            {
                var useraccount = new UserAccount()
                {
                    User_id = foodcardrecharge.OperateFor,
                    FoodCard_id = foodcardrecharge.FoodCard_id,
                    Balance = -foodcardrecharge.Amount
                };

                db.UserAccounts.Add(useraccount);
            }
            else if (rechargeFor.Count() == 1)
            {
                var userAccount = rechargeFor.First();
                userAccount.Balance -= foodcardrecharge.Amount;
                db.Entry(userAccount).State = EntityState.Modified;
            }
            else
            {
                throw new Exception(string.Format("Invalid user account, user id: {0}, foodcard id: {1}.",
                                                  foodcardrecharge.OperateFor,
                                                  foodcardrecharge.FoodCard_id));
            }
            
            db.Entry(foodcard).State = EntityState.Modified;
            db.FoodCardRecharges.Remove(foodcardrecharge);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}