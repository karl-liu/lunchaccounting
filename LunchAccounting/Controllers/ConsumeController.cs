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
    public class ConsumeController : ControllerBase
    {
        //
        // GET: /Consume/

        public ActionResult Index()
        {
            var userconsumes = db.UserConsumes
                .Include(u => u.ConsumerInfo)
                .Where(p => p.ConsumerInfo.UserName.Equals(HttpContext.User.Identity.Name, StringComparison.OrdinalIgnoreCase)); ;
            return View(userconsumes.ToList());
        }

        //
        // GET: /Consume/Details/5

        public ActionResult Details(int id = 0)
        {
            UserConsume userconsume = db.UserConsumes.Find(id);
            if (userconsume == null)
            {
                return HttpNotFound();
            }
            return View(userconsume);
        }

        //
        // GET: /Consume/Create

        public ActionResult Create()
        {
            var user = db.Users.Where(p => p.UserName == HttpContext.User.Identity.Name).FirstOrDefault();
            ViewBag.User_id = new SelectList(db.Users, "User_id", "UserName", user.User_id);
            ViewBag.FoodCard_id = new SelectList(db.FoodCards, "FoodCard_id", "FoodCardName");
            return View();
        }

        //
        // POST: /Consume/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserConsume userconsume)
        {
            var today = Utilities.ToDateWithTimeZone(DateTime.Now.ToUniversalTime(), Utilities.ChinaTimeZoneId);
            userconsume.ConsumeDate = today;

            var register = db.LunchRegisters
                .Include(p => p.User)
                .Include(p => p.UserConsumes)
                .Where(p => p.User_id == userconsume.User_id && p.LunchDate == today.Date); 

            if (register.Count() == 1)
            {
                userconsume.Register_id = register.First().LunchRegister_id;
                if (ModelState.IsValid)
                {
                    var foodCard = db.FoodCards.Find(userconsume.FoodCard_id);
                    foodCard.CurrentBalance -= userconsume.Amount;
                    db.Entry(foodCard).State = EntityState.Modified;
                    
                    // charge on user account
                    var consumeOn = db.UserAccounts.Where(p =>
                    p.FoodCard_id == userconsume.FoodCard_id &&
                    p.User_id == userconsume.User_id);

                    if (consumeOn.Count() == 0)
                    {
                        var useraccount = new UserAccount()
                        {
                            User_id = userconsume.User_id,
                            FoodCard_id = userconsume.FoodCard_id,
                            Balance = -userconsume.Amount
                        };

                        db.UserAccounts.Add(useraccount);
                    }
                    else if (consumeOn.Count() == 1)
                    {
                        var userAccount = consumeOn.First();
                        userAccount.Balance -= userconsume.Amount;
                        db.Entry(userAccount).State = EntityState.Modified;
                    }
                    else
                    {
                        throw new Exception(string.Format("Invalid user account, user id: {0}, foodcard id: {1}.",
                                                          userconsume.User_id,
                                                          userconsume.FoodCard_id));
                    }

                    db.UserConsumes.Add(userconsume);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("User_id", "Invalid Lunch registeration");
            }

            ViewBag.User_id = new SelectList(db.Users, "User_id", "UserName", userconsume.User_id);
            ViewBag.FoodCard_id = new SelectList(db.FoodCards, "FoodCard_id", "FoodCardName", userconsume.FoodCard_id);
            return View(userconsume);
        }

        //
        // GET: /Consume/Edit/5

        public ActionResult Edit(int id = 0)
        {
            UserConsume userconsume = db.UserConsumes.Find(id);
            if (userconsume == null)
            {
                return HttpNotFound();
            }
            ViewBag.User_id = new SelectList(db.Users, "User_id", "UserName", userconsume.User_id);
            ViewBag.FoodCard_id = new SelectList(db.FoodCards, "FoodCard_id", "FoodCardName", userconsume.FoodCard_id);
            return View(userconsume);
        }

        //
        // POST: /Consume/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserConsume userconsume)
        {
            if (ModelState.IsValid)
            {
                var originalConsume = db.UserConsumes.Find(userconsume.UserConsume_id);
                var originFoodcard = db.FoodCards.Find(originalConsume.FoodCard_id);

                var originUserAccount = db.UserAccounts.Where(p =>
                    p.FoodCard_id == originalConsume.FoodCard_id &&
                    p.User_id == originalConsume.User_id);

                var userAccount = db.UserAccounts.Where(p =>
                    p.FoodCard_id == userconsume.FoodCard_id &&
                    p.User_id == userconsume.User_id);

                if (originUserAccount.Count() == 0)
                {
                    var useraccount = new UserAccount()
                    {
                        User_id = originalConsume.User_id,
                        FoodCard_id = originalConsume.FoodCard_id,
                        Balance = -originalConsume.Amount
                    };

                    db.UserAccounts.Add(useraccount);
                }

                if (userAccount.Count() == 0)
                {
                    var useraccount = new UserAccount()
                    {
                        User_id = userconsume.User_id,
                        FoodCard_id = userconsume.FoodCard_id,
                        Balance = -userconsume.Amount
                    };

                    db.UserAccounts.Add(useraccount);
                }

                var currentUserAccount = originUserAccount.First();
                var targetUserAccount = userAccount.First();

                // switch to another card
                if (originalConsume.FoodCard_id != userconsume.FoodCard_id)
                {
                    originFoodcard.CurrentBalance += originalConsume.Amount;
                    db.Entry(originFoodcard).State = EntityState.Modified;

                    var newFoodcard = db.FoodCards.Find(userconsume.FoodCard_id);
                    newFoodcard.CurrentBalance -= userconsume.Amount;
                    db.Entry(newFoodcard).State = EntityState.Modified;

                    currentUserAccount.Balance += originalConsume.Amount;
                    targetUserAccount.Balance -= userconsume.Amount;
                    db.Entry(currentUserAccount).State = EntityState.Modified;
                    db.Entry(targetUserAccount).State = EntityState.Modified;
                }
                else
                {
                    // same card, different amount
                    if (originalConsume.Amount != userconsume.Amount)
                    {
                        originFoodcard.CurrentBalance += originalConsume.Amount;
                        originFoodcard.CurrentBalance -= userconsume.Amount;
                        db.Entry(originFoodcard).State = EntityState.Modified;

                        currentUserAccount.Balance += originalConsume.Amount;
                        currentUserAccount.Balance -= userconsume.Amount;
                        db.Entry(currentUserAccount).State = EntityState.Modified;
                    }
                }

                originalConsume.Amount = userconsume.Amount;
                originalConsume.FoodCard_id = userconsume.FoodCard_id;

                db.Entry(originalConsume).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.User_id = new SelectList(db.Users, "User_id", "UserName", userconsume.User_id);
            return View(userconsume);
        }

        //
        // GET: /Consume/Delete/5

        public ActionResult Delete(int id = 0)
        {
            UserConsume userconsume = db.UserConsumes.Find(id);
            if (userconsume == null)
            {
                return HttpNotFound();
            }
            return View(userconsume);
        }

        //
        // POST: /Consume/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {            
            var userconsume = db.UserConsumes.Find(id);
            var foodCard = db.FoodCards.Find(userconsume.FoodCard_id);

            // charge on user account
            var consumeOn = db.UserAccounts.Where(p =>
            p.FoodCard_id == userconsume.FoodCard_id &&
            p.User_id == userconsume.User_id);

            if (consumeOn.Count() == 0)
            {
                var useraccount = new UserAccount()
                {
                    User_id = userconsume.User_id,
                    FoodCard_id = userconsume.FoodCard_id,
                    Balance = userconsume.Amount
                };

                db.UserAccounts.Add(useraccount);
            }
            else if (consumeOn.Count() == 1)
            {
                var userAccount = consumeOn.First();
                userAccount.Balance += userconsume.Amount;
                db.Entry(userAccount).State = EntityState.Modified;
            }
            else
            {
                throw new Exception(string.Format("Invalid user account, user id: {0}, foodcard id: {1}.",
                                                  userconsume.User_id,
                                                  userconsume.FoodCard_id));
            }


            foodCard.CurrentBalance += userconsume.Amount;

            db.Entry(foodCard).State = EntityState.Modified;
            db.UserConsumes.Remove(userconsume);
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