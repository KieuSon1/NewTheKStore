using NewTheKStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewTheKStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(int ?id)
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                ViewBag.userName = Session["username"].ToString();
                ViewBag.admin = Session["admin"];
            }
            else
            {
                ViewBag.userName = "`";
            }

            using (TheKStore dbModel = new TheKStore())
            {
                List<product> p = new List<product>();               
                List<product> product = dbModel.products.ToList();
                if (id == null)
                {
                    id = 0;
                }
                else if (id == 1)
                {
                    product = dbModel.products.Where(x => x.categorize == "notebook").ToList();
                }
                else if (id == 2)
                {
                    product = dbModel.products.Where(x => x.categorize == "desktop").ToList();
                }
                else if (id == 3)
                {
                    product = dbModel.products.Where(x => x.categorize == "display").ToList();
                }
                else if (id == 4)
                {
                    product = dbModel.products.Where(x => x.categorize == "accessories").ToList();
                }


                foreach (product a in product)
                {
                    if (p.Exists(x => x.subID == a.subID))
                    {
                        continue;
                    }
                    p.Add(a);
                }
                ViewBag.product = p;
            }
            ViewBag.id = id;
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(string email, string password, string verifyPassword, string phone, string area = "")
        {
            ViewBag.Page = "SignUp";
            using (TheKStore dbModel = new TheKStore())
            {
                account acc = new account(password, email, phone);
                dbModel.accounts.Add(acc);
                try
                {
                    dbModel.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
            ModelState.Clear();


            Session["username"] = email;

            if (area == "ConfirmCart")
                return RedirectToAction("ComfirmPay");
            else if (area == "")
                return RedirectToAction("Index");
            else
                return RedirectToAction(area);
        }

        [HttpPost]
        public ActionResult Login(string email, string password, string area = "")
        {
            ViewBag.Page = "Login";
            using (TheKStore dbModel = new TheKStore())
            {
                account names = dbModel.accounts.FirstOrDefault(x => x.email == email);

                if (names == null)
                {
                    return View();
                }

                if (names.password != password)
                {
                    return View();

                }
                Session["username"] = email;
                Session["admin"] = names.admin;
                if (area == "ConfirmCart")
                    return RedirectToAction("ComfirmPay");
                else if (area == "")
                    return RedirectToAction("Index");
                else
                    return RedirectToAction(area);
            }
        }

        public ActionResult LogOut()
        {
            Session["username"] = null;
            Session["admin"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult Product(int? id)
        {
            ViewBag.Page = "Product";
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                ViewBag.userName = Session["username"].ToString();
                ViewBag.admin = Session["admin"];
            }
            else
            {
                ViewBag.userName = "";
            }

            if (id == null)
                id = 1;

            using (TheKStore dbModel = new TheKStore())
            {
                product product = dbModel.products.FirstOrDefault(x => x.productID == id);
                List<product> products = dbModel.products.Where(x => x.subID == product.subID).ToList();
                ViewBag.product = products;
                ViewBag.id = id;
            }
            return View();
        }



        public ActionResult AddCart(int count, int? id, decimal price, string name = "", string url = "")
        {
            List<Cart> cartlist = GetListCart();

            string array = "";
            if (cartlist.Exists(x => x.id == id))
            {
                Cart c = cartlist.Find(x => x.id == id);

                cartlist.Remove(c);
                c.count += count;
                cartlist.Add(c);
            }
            else
            {
                cartlist.Add(new Cart(id, name, url, price, count));
            }

            foreach (Cart l in cartlist)
            {
                array += l.ToString() + "\n";
            }
            Session["Cart"] = cartlist;
            return RedirectToAction("Cart");
        }

        private List<Cart> GetListCart()
        {
            if (Session["Cart"] is List<Cart>)
            {
                return (List<Cart>)Session["Cart"];
            }
            else
            {
                return new List<Cart>();
            }
        }

        public ActionResult Cart(int? id)
        {
            ViewBag.Page = "Cart";
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                ViewBag.userName = Session["username"].ToString();
                ViewBag.admin = Session["admin"];
            }
            else
            {
                ViewBag.userName = "";
            }
            using (TheKStore dbModel = new TheKStore())
            {
                List<product> product = dbModel.products.ToList();
                ViewBag.product = product;
            }
            if (id is null)
            {
                List<Cart> cart1 = GetListCart();
                if (Session["Cart"] is List<Cart>)
                {
                    ViewBag.Cart = (List<Cart>)Session["Cart"];
                }
                else
                {
                    ViewBag.Cart = new List<Cart>();
                }
                return View();
            }
            List<Cart> cart = GetListCart();
            cart.RemoveAt((int)id);
            Session["Cart"] = cart;
            ViewBag.Cart = (List<Cart>)Session["Cart"];
            return RedirectToAction("Cart");
        }

        public ActionResult ComfirmPay()
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                string uname = Session["username"].ToString();
                ViewBag.userName = uname;
                ViewBag.admin = Session["admin"];

                using (TheKStore dbModel = new TheKStore())
                {
                    account names = dbModel.accounts.FirstOrDefault(x => x.email == uname);

                    if (names == null)
                    {
                        return View();
                    }
                    ViewBag.accResult = names;

                  
                    List<product> product = dbModel.products.ToList();
                    ViewBag.product = product;
                    
                    List<Cart> cart = GetListCart();
                    if (cart.Count == 0)
                    {
                        return RedirectToAction("Index", "Home", new { area = "ConfirmCart" });
                    }
                    ViewBag.cart = cart;

                    DateTime today = DateTime.Today;

                    account acc = dbModel.accounts.FirstOrDefault(x => x.email == uname);
                    cart newCart = new cart();
                    newCart.cartID = dbModel.carts.ToList().Count;
                    newCart.userID = acc.userID;
                    newCart.orderdate = today;
                    newCart.deliverydate = today.AddDays(2);
                    newCart.location = acc.location;
                    newCart.type = 0;
                    dbModel.carts.Add(newCart);
                    dbModel.SaveChanges();


                    List<Cart> gh = (List<Cart>)Session["Cart"];

                    foreach (Cart item in gh)
                    {
                        cartinfo newcartinfo = new cartinfo();
                        newcartinfo.cartID = newCart.cartID;
                        newcartinfo.productID = item.id ?? 0;
                        newcartinfo.price = item.price;
                        newcartinfo.quantity = item.count;
                        dbModel.cartinfoes.Add(newcartinfo);
                        dbModel.SaveChanges();
                    }
                    
                    Session["Cart"] = new List<Cart>();
                }
            }
            else
            {
                ViewBag.userName = "";
                return RedirectToAction("Login", "Home", new { area = "ConfirmCart" });
            }

           

            return View();
        }

        public ActionResult SuccessPay()
        {
            string name = Session["username"].ToString();
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                ViewBag.userName = Session["username"].ToString();
                ViewBag.admin = Session["admin"];

                using (TheKStore dbModel = new TheKStore())
                {
                    DateTime today = DateTime.Today;

                    account acc = dbModel.accounts.FirstOrDefault(x => x.email == name);
                    cart newCart = new cart();
                    newCart.cartID = dbModel.carts.ToList().Count;
                    newCart.userID = acc.userID;
                    newCart.orderdate = today;
                    newCart.deliverydate = today.AddDays(2);
                    newCart.location = acc.location;
                    dbModel.carts.Add(newCart);
                    dbModel.SaveChanges();


                    List<Cart> gh = (List<Cart>)Session["Cart"];

                    foreach (Cart item in gh)
                    {
                        cartinfo newcartinfo = new cartinfo();
                        newcartinfo.cartID = newCart.cartID;
                        newcartinfo.productID = item.id ?? 0;
                        newcartinfo.price = item.price;
                        newcartinfo.quantity = item.count;
                        dbModel.cartinfoes.Add(newcartinfo);
                        dbModel.SaveChanges();
                    }
                }
                Session["Cart"] = new List<Cart>();
            }
            else
            {
                ViewBag.userName = "";
            }
            return View();
        }


        [HttpPost]
        public ActionResult UpdateCart(string id, string count)
        {
            if (Session["Cart"] is List<Cart>)
            {
                List<Cart> cart = (List<Cart>)Session["Cart"];
               
                Cart c = cart.FirstOrDefault(x => x.id == Int32.Parse(id));
                Cart e = c;
                c.count = Int32.Parse(count);
                cart.Remove(e);
                cart.Add(c);
                ViewBag.Cart = cart;
            }
            else
            {
                ViewBag.Cart = new List<Cart>();
            }
            return RedirectToAction("Cart");
        }



        public ActionResult ChangePassword()
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                string uName = Session["username"].ToString();

                ViewBag.userName = uName;
                ViewBag.admin = Session["admin"];
                using (TheKStore dbModel = new TheKStore())
                {
                    account acc = dbModel.accounts.FirstOrDefault(a => a.email == uName);
                    ViewBag.acc = acc;
                }
            }
            else
            {
                ViewBag.userName = "";
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult SavePassword(string id, string pass)
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                string uName = Session["username"].ToString();

                ViewBag.userName = uName;
                ViewBag.admin = Session["admin"];
                using (TheKStore dbModel = new TheKStore())
                {
                    account acc = dbModel.accounts.FirstOrDefault(a => a.email == uName);
                    acc.password = pass;
                    dbModel.SaveChanges();
                }
            }
            else
            {
                ViewBag.userName = "";
                return RedirectToAction("Login", "Home");
            }
            return RedirectToAction("UpdateInfo");
        }


        public ActionResult UpdateInfo()
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                string uName = Session["username"].ToString();

                ViewBag.userName = uName;
                ViewBag.admin = Session["admin"];
                using (TheKStore dbModel = new TheKStore())
                {
                    account acc = dbModel.accounts.FirstOrDefault(a => a.email == uName);
                    ViewBag.acc = acc;
                }
            }
            else
            {
                ViewBag.userName = "";
                return RedirectToAction("Login", "Home");
            }

            return View();
        }


        public ActionResult SaveAccountInfo(int id, string email, string phone, string location)        
        {
                if (!string.IsNullOrEmpty(Session["username"] as string))
                {  
                
                String userName = Session["username"].ToString();
                using (TheKStore dbModel = new TheKStore())
                    {
                    account names = dbModel.accounts.FirstOrDefault(x => x.email == userName);
                    if (names.admin == true)
                    {

                        account acc = dbModel.accounts.FirstOrDefault(a => a.userID == id);                        
                        acc.email = email;
                        acc.phone = phone;
                        acc.location = location;
                        dbModel.SaveChanges();
                    }
                }
                }
                else
                {
                    ViewBag.userName = "";
                    return RedirectToAction("Login", "Home");
                }
                return RedirectToAction("UpdateInfo"); 
        }

        
    }
}