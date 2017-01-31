using Blog.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact([Bind(Include = "Id,Name,Email,Message,Phone,MessageSent")] Contact contact)
        {
            {
                contact.MessageSent = DateTime.Now;

                var svc = new EmailService();
                var msg = new IdentityMessage();
                msg.Destination = contact.Email;
                msg.Subject = "New Message from Blog Website";
                msg.Body = "Message: " + contact.Message + "<br><br>From: " + contact.Name + "<br><br>E-mail: " + contact.Email + "<br><br>Phone Number: " + contact.Phone;
                await svc.SendAsync(msg);

                ViewBag.Message = "Thank you for your message!";

                return View(contact);
            }
        }
    }
}