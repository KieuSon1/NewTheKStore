using NewTheKStore.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewTheKStore.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                String userName = Session["username"].ToString();
                using (TheKStore dbModel = new TheKStore())
                {
                    account names = dbModel.accounts.FirstOrDefault(x => x.email == userName);
                    if (names.admin == true)
                    {
                        return View();
                    }
                }

            }

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }
       

        public ActionResult ProductInfo(int ? page)
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                String userName = Session["username"].ToString();
                using (TheKStore dbModel = new TheKStore())
                {
                    account names = dbModel.accounts.FirstOrDefault(x => x.email == userName);
                    if (names.admin == true)
                    {
                        using (TheKStore dbModel1 = new TheKStore())
                        {
                            List<product> product = dbModel1.products.ToList();
                            ViewBag.product = product;
                            ViewBag.page = page;
                            if (page == null)
                            {
                                ViewBag.page = 1;
                            }
                        }
                        return View();
                    }
                }
            }
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        public ActionResult EditProduct(int? id)
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                String userName = Session["username"].ToString();
                using (TheKStore dbModel = new TheKStore())
                {
                    account names = dbModel.accounts.FirstOrDefault(x => x.email == userName);
                    if (names.admin == true)
                    {
                        List<product> product = dbModel.products.ToList();
                        ViewBag.product = product;
                        if (id == null)
                        {
                            ViewBag.Title = "Thêm Sản Phẩm";
                            
                            ViewBag.index = product.Count;
                            return View();
                        }
                            ViewBag.Title = "Chỉnh sửa thông tin sản phẩm";



                        using (var db = new TheKStore())
                        {
                            var result = db.products.SingleOrDefault(b => b.productID == id);                            
                            if (result != null)
                            {
                                ViewBag.name = result.name;
                                ViewBag.image = result.imgscr;
                                ViewBag.subID = result.subID;
                                ViewBag.price = result.price;
                                ViewBag.processor = result.processor;
                                ViewBag.memory = result.memory;
                                ViewBag.storage = result.storage;
                                ViewBag.display = result.display;
                                ViewBag.categorize = result.categorize;                              

                                ViewBag.id = id;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return View();
            }
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        public ActionResult SaveProduct(int? id, string image, string name, string subID, string processor, int memory, string storage, string display, string categorize, string price)
        {
            decimal p = decimal.Parse(price, CultureInfo.InvariantCulture);


            if (!string.IsNullOrEmpty(Session["username"] as string))
            {

                String userName = Session["username"].ToString();
                using (TheKStore dbModel = new TheKStore())
                {
                    account names = dbModel.accounts.FirstOrDefault(x => x.email == userName);
                    if (names.admin == true)
                    {
                        if (id == null)
                            return RedirectToAction("ProductInfo", "Admin");

                        product result = dbModel.products.FirstOrDefault(b => b.productID == id);

                        if (result != null)
                        {
                            result.name = name;
                            result.subID = subID;
                            result.imgscr = image;
                            result.price = p;
                            result.processor = processor;
                            result.memory = memory;
                            result.storage = storage;
                            result.categorize = categorize;
                            dbModel.SaveChanges();
                        }
                        else
                        {
                            result = new product();
                            result.productID = id??0;
                            result.name = name;
                            result.subID = subID;
                            result.imgscr = image;
                            result.price = p;
                            result.processor = processor;
                            result.memory = memory;
                            result.storage = storage;
                            result.categorize = categorize;
                            dbModel.products.Add(result);
                            dbModel.SaveChanges();
                        }
                        return RedirectToAction("ProductInfo", "Admin");
                    }
                }

            }
            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        //Đây Là Form Hóa Đơn Bán
        public ActionResult Bill(bool complete = false, bool holding = false, bool priceT = false, bool priceG = false, bool pNew = false, bool pOld = false)
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                String userName = Session["username"].ToString();
                using (TheKStore dbModel = new TheKStore())
                {
                    account names = dbModel.accounts.FirstOrDefault(x => x.email == userName);
                    if (names.admin == true)
                    {
                        List<cart> cartList = dbModel.carts.ToList();
                        List<cartinfo> cartI = dbModel.cartinfoes.ToList();
                        ViewBag.pending = "";
                        if (complete == true)
                        {
                            cartList = dbModel.carts.Where(x => x.type == 1).ToList();
                            ViewBag.pending = "Pending: Finish";
                        }
                        if (holding == true)
                        {
                            cartList = dbModel.carts.Where(x => x.type == 0).ToList();
                            ViewBag.pending = "Pending: On delivery";
                        }

                        if (pNew == true)
                        {
                            cartList = dbModel.carts.OrderByDescending(x => x.orderdate).ToList();
                            ViewBag.pending = "Date Descending";
                        }
                        if (pOld == true)
                        {
                            cartList = dbModel.carts.OrderBy(x => x.orderdate).ToList();
                            ViewBag.pending = "Date Ascending";
                        }


                        if (priceT == true)
                        {
                            List<SortList> listSort = new List<SortList>();
                            for (int i = 0; i < cartList.Count; i++)
                            {
                                decimal total = 0;
                                List<cartinfo> cartF = cartI.Where(x => x.cartID == cartList[i].cartID).ToList();
                                foreach (cartinfo c in cartF)
                                {
                                    total += c.price * c.quantity ?? 1;
                                }
                                listSort.Add(new SortList(i, total));
                            }
                            listSort = listSort.OrderBy(x => x.total).ToList();
                            List<cart> newList = new List<cart>();
                            for (int i = 0; i < listSort.Count; i++)
                            {
                                cart c = cartList.FirstOrDefault(x => x.cartID == listSort[i].id);
                                newList.Add(c);
                            }
                            cartList = newList;

                        }

                        if (priceG == true)
                        {
                            List<SortList> listSort = new List<SortList>();
                            for (int i = 0; i < cartList.Count; i++)
                            {
                                decimal total = 0;
                                List<cartinfo> cartF = cartI.Where(x => x.cartID == cartList[i].cartID).ToList();
                                foreach (cartinfo c in cartF)
                                {
                                    total += c.price * c.quantity ?? 1;
                                }
                                listSort.Add(new SortList(i, total));
                            }
                            listSort = listSort.OrderByDescending(x => x.total).ToList();
                            List<cart> newList = new List<cart>();
                            for (int i = 0; i < listSort.Count; i++)
                            {
                                cart c = cartList.FirstOrDefault(x => x.cartID == listSort[i].id);
                                newList.Add(c);
                            }
                            cartList = newList;

                        }





                        ViewBag.carts = cartList;
                        ViewBag.cartsI = cartI;
                        return View();
                    }
                }

            }

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        //Đây Là Form Chi Tiết Của Hóa Đơn Bán Khi Click Vào Tình Trạng Của Hóa Đơn
        public ActionResult BillInfo(int? id)
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                String userName = Session["username"].ToString();
                using (TheKStore dbModel = new TheKStore())
                {
                    account names = dbModel.accounts.FirstOrDefault(x => x.email == userName);
                    if (names.admin == true)
                    {
                        if (id == null)
                        {
                            return RedirectToAction("Bill", "Admin");
                        }
                        List<cartinfo> cartI = dbModel.cartinfoes.Where(x => x.cartID == id).ToList();
                        List<product> pList = dbModel.products.ToList();

                        ViewBag.cartsI = cartI;
                        ViewBag.pList = pList;
                        return View();
                    }
                }

            }

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        public ActionResult ConfirmBill(int? id)
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                String userName = Session["username"].ToString();
                using (TheKStore dbModel = new TheKStore())
                {
                    account names = dbModel.accounts.FirstOrDefault(x => x.email == userName);
                    if (names.admin == true)
                    {
                        if (id == null)
                        {
                            return RedirectToAction("Bill", "Admin");
                        }
                        cart cartGet = dbModel.carts.FirstOrDefault(x => x.cartID == id);
                        cartGet.type = 1;
                        dbModel.SaveChanges();
                        return RedirectToAction("Bill", "Admin");
                    }
                }

            }

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        //Đây Là Form Danh Sách Tài Khoản (Đã Chỉnh Sửa Chỗ DropDown Không Còn Chia Làm 2 Loại Tài Khoản Nữa Mà Lọc Chung Với Phần Tìm Kiếm
        public ActionResult ListAccount(bool? admin, string name = "", string email = "", string location = "")
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                String userName = Session["username"].ToString();
                using (TheKStore dbModel = new TheKStore())
                {
                    account names = dbModel.accounts.FirstOrDefault(x => x.email == userName);
                    if (names.admin == true)
                    {
                        if (name != "")
                        {
                            List<account> account = dbModel.accounts.Where(a => (a.email.ToLower()).IndexOf(name) != -1).ToList();
                            ViewBag.account = account;
                        }
                        else if (email != "")
                        {
                            List<account> account = dbModel.accounts.Where(a => (a.email.ToLower()).IndexOf(email) != -1).ToList();
                            ViewBag.account = account;
                        }
                        else if (location != "")
                        {
                            List<account> account = dbModel.accounts.Where(a => (a.location.ToLower()).IndexOf(location) != -1).ToList();
                            ViewBag.account = account;
                        }
                        else if (admin == true)
                        {
                            List<account> account = dbModel.accounts.Where(a => a.admin == true).ToList();
                            ViewBag.account = account;
                        }
                        else if (admin == false)
                        {
                            List<account> account = dbModel.accounts.Where(a => a.admin != true).ToList();
                            ViewBag.account = account;
                        }
                        else if (admin == null)
                        {
                            List<account> account = dbModel.accounts.ToList();
                            ViewBag.account = account;
                        }
                        return View();
                    }
                }


            }
            return RedirectToAction("Index", "Home", new { area = "Admin" });

        }

        public ActionResult RemoveAccount(string acc = "")
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                String userName = Session["username"].ToString();
                using (TheKStore dbModel = new TheKStore())
                {
                    if (acc == "")
                        return RedirectToAction("ListAccount", "Admin");

                    account find = dbModel.accounts.FirstOrDefault(a => a.email == acc && a.admin != true);
                    dbModel.accounts.Remove(find);
                    dbModel.SaveChanges();

                    return RedirectToAction("ListAccount", "Admin");
                }

            }

            return RedirectToAction("Index", "Home", new { area = "Admin" });

        }

        public ActionResult RemoveProDuctinfo(int? productID)
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                String userName = Session["username"].ToString();
                using (TheKStore dbModel = new TheKStore())
                {
                    if (productID == null)
                        return RedirectToAction("ProductInfo", "Admin");

                    product idToRemove = dbModel.products.FirstOrDefault(a => a.productID == productID);
                    dbModel.products.Remove(idToRemove);
                    dbModel.SaveChanges();

                    return RedirectToAction("ProductInfo", "Admin");
                }

            }
            return RedirectToAction("Index", "Home", new { area = "Admin" });

        }

        public ActionResult UpdateInfo(int? id)
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
                        if (acc == null)
                        {
                            acc = dbModel.accounts.First();
                            ViewBag.acc = acc;
                            return View();
                        }
                        ViewBag.acc = acc;
                        return View();
                    }
                }
            }
            else
            {
                ViewBag.userName = "";
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        public ActionResult SaveAccount(int id, string pass, string email, string phone, string location, string admin)
        {
            if (!string.IsNullOrEmpty(Session["username"] as string))
            {
                string uName = Session["username"].ToString();

                ViewBag.userName = uName;
                ViewBag.admin = Session["admin"];
                using (TheKStore dbModel = new TheKStore())
                {
                    account acc = dbModel.accounts.FirstOrDefault(a => a.userID == id);                  
                    
                    acc.password = pass;
                    acc.email = email;
                    acc.phone = phone;
                    acc.location = location;
                    acc.admin = bool.Parse(admin);
                    dbModel.SaveChanges();
                }
            }
            else
            {
                ViewBag.userName = "";
                return RedirectToAction("Login", "Home");
            }
            return RedirectToAction("ListAccount"); ;
        }
    }
}