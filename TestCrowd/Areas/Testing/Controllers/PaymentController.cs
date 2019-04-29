using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PayPal.Api;
using TestCrowd.Areas.Testing.Class;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Repository;

namespace TestCrowd.Areas.Testing.Controllers
{
    [Authorize(Roles = "company, tester")]
    public class PaymentController : Controller
    {
        public ActionResult Index()
        {
            if (User.IsInRole("tester"))
            {
                return RedirectToAction("Withdraw");
            }

            return RedirectToAction("Deposit");
        }

        [Authorize(Roles = "company")]
        public ActionResult Deposit()
        {
            ApplicationUserRepository<Company> userRepo = new ApplicationUserRepository<Company>();
            Company company = userRepo.GetByUserName(User.Identity.Name);
            ViewBag.Credits = company.Credits;
            return View("Deposit");
        }

        [Authorize(Roles = "tester")]
        public ActionResult Withdraw()
        {
            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            Tester tester = userRepo.GetByUserName(User.Identity.Name);
            ViewBag.Credits = tester.Credits;
            return View("Withdraw");
        }

        public ActionResult PaymentWithPaypal(string Cancel = null, int credits = 0) 
        {
            ApplicationUserRepository<Company> userRepo = new ApplicationUserRepository<Company>();
            Company company = userRepo.GetByUserName(User.Identity.Name);

            //getting the apiContext
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal
                //Payer Id will be returned when payment proceeds or click to pay
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist
                    //it is returned by the create function call of the payment class

                    // Creating a payment
                    // baseURL is the url on which paypal sendsback the data.
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority +
                                "/Testing/Payment/PaymentWithPaypal?";

                    //here we are generating guid for storing the paymentID received in session
                    //which will be used in the payment execution

                    var guid = Convert.ToString((new Random()).Next(100000));

                    //CreatePayment function gives us the payment approval url
                    //on which payer is redirected for paypal account payment

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, company, credits);

                    //get links returned from paypal in response to Create function call

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {

                    // This function exectues after receving all parameters for the payment

                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    credits = int.Parse(executedPayment.transactions[0].item_list.items[0].quantity);

                    //If executed payment failed then we will show payment failure message to user
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        TempData["error"] = "Deposit failed";
                        ViewBag.Credits = company.Credits;
                        return View("Deposit");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Deposit failed";
                ViewBag.Credits = company.Credits;
                return View("Deposit");
            }

            //on successful payment, show success page to user.
            company.Credits = company.Credits + credits;
            userRepo.Update(company);

            ViewBag.Credits = company.Credits;
            TempData["success"] = "Deposit succeed";
            return View("Deposit");
        }

        private Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl, Company company, int credits)
        {

            //create itemlist and add item objects to it
            var itemList = new ItemList() { items = new List<Item>() };

            //Adding Item Details like name, currency, price etc
            itemList.items.Add(new Item()
            {
                name = $"TestCrowd credits for account {company.UserName}",
                currency = "EUR",
                price = "0.011",
                quantity = credits.ToString(),
                sku = "sku"
            });

            var payer = new Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            // Adding Tax, shipping and Subtotal details
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = ((int)(0.01*credits)).ToString()
            };

            //Final amount with details
            var amount = new Amount()
            {
                currency = "EUR",
                total = ((int)(0.01 * credits)).ToString(), // Total must be equal to sum of tax, shipping and subtotal.
                details = details
            };

            var transactionList = new List<Transaction>();
            // Adding description about the transaction
            transactionList.Add(new Transaction()
            {
                description = "TestCrowd credits",
                invoice_number = $"INO-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}-{Guid.NewGuid()}", //Generate an Invoice No
                amount = amount,
                item_list = itemList
            });


            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);
        }

        public ActionResult PayoutWithPaypal(int credits = 0)
        {
            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            Tester tester = userRepo.GetByUserName(User.Identity.Name);

            if (credits < 100)
            {
                TempData["error"] = "Minimal amount of credits for withdraw is 100 (1 EUR)";
                ViewBag.Credits = tester.Credits;
                return View("Withdraw");
            }

            if (tester.Credits < credits)
            {
                TempData["error"] = "You don't have required amount of credits!";
                ViewBag.Credits = tester.Credits;
                return View("Withdraw");
            }

            //getting the apiContext
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                var payout = new Payout
                {
                    // #### sender_batch_header
                    // Describes how the payments defined in the `items` array are to be handled.
                    sender_batch_header = new PayoutSenderBatchHeader
                    {
                        sender_batch_id = "batch_" + Guid.NewGuid().ToString().Substring(0, 8),
                        email_subject = "You have a payment"
                    },
                    // #### items
                    // The `items` array contains the list of payout items to be included in this payout.
                    // If `syncMode` is set to `true` when calling `Payout.Create()`, then the `items` array must only
                    // contain **one** item.  If `syncMode` is set to `false` when calling `Payout.Create()`, then the `items`
                    // array can contain more than one item.
                    items = new List<PayoutItem>
                {
                    new PayoutItem
                    {
                        recipient_type = PayoutRecipientType.EMAIL,
                        amount = new Currency
                        {
                            value = ((int)(credits*0.01)).ToString(),
                            currency = "EUR"
                        },
                        receiver = tester.Email,
                        note = "Thank you.",
                        sender_item_id = "item_1"
                    }                   
                }
                };

                var creatPayout = payout.Create(apiContext, false);
            }
            catch (Exception ex)
            {
                TempData["error"] = "Withdraw failed";
                ViewBag.Credits = tester.Credits;
                return View("Withdraw");
            }

            //on successful payment, show success page to user.
            tester.Credits = tester.Credits - credits;
            userRepo.Update(tester);

            ViewBag.Credits = tester.Credits;
            TempData["success"] = "Withdraw request succeed. Please check your mailbox and follow instructions of PayPal company.";
            return View("Withdraw");
        }       
    }
}