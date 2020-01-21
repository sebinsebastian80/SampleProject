using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SampleProject.Models;



namespace SampleProject.Controllers
{
  

    public class AccountController : Controller
    {

        public RoleBaseAccessibilityEntities databaseManager = new RoleBaseAccessibilityEntities();

        public AccountController()
        {
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                // Verification.    
                if (this.Request.IsAuthenticated)
                {
                    // Info.    
                    return this.RedirectToLocal(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info    
                Console.Write(ex);
            }
            // Info.    
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            string Decrypt(string clearText)
            {
                string EncryptionKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }

            try
            {
     
                // Verification.    
                if (ModelState.IsValid)
                {
                    // Initialization.    
                    var loginInfo = this.databaseManager.LoginByUsernamePassword(model.Username, Decrypt(model.Password).Trim()).ToList();
                    // Verification.    
                    if (loginInfo != null && loginInfo.Count() > 0)
                    {
                        // Initialization.    
                        var logindetails = loginInfo.First();
                        // Login In.    
                        this.SignInUser(logindetails.username, logindetails.role_id, false);
                        // setting.    
                        this.Session["role_id"] = logindetails.role_id;
                        // Info.    

                        return this.RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        // Setting.    
                        ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Info    
                Console.Write(ex);
            }
            // If we got this far, something failed, redisplay form    
            return this.View(model);
        }

        private object CreateSalt()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {
                if (Session.SessionID != null)
                {
                    // Setting.    
                    var ctx = Request.GetOwinContext();
                    var authenticationManager = ctx.Authentication;
                    // Sign Out.    
                    authenticationManager.SignOut();
                    Session.Abandon();
                }
                else
                {
                    RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                // Info    
                //throw ex;
            }
            // Info.    
            return this.RedirectToAction("Login", "Account");
        }

        private void SignInUser(string username, int role_id, bool isPersistent)
        {
            // Initialization.    
            var claims = new List<Claim>();
            try
            {
                // Setting    
                claims.Add(new Claim(ClaimTypes.Name, username));
                claims.Add(new Claim(ClaimTypes.Role, role_id.ToString()));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign In.    
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                // Verification.    
                if (Url.IsLocalUrl(returnUrl))
                {
                    // Info.    
                    return this.Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
            // Info.    
            return this.RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]


        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel newreg)
        {

            //password hashing
            string Encrypt(string clearText)
            {
                string EncryptionKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }



            

            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new RoleBaseAccessibilityEntities())
                    {
                        Login relog = new Login();
                        relog.username = newreg.UserName;
                        relog.password = Encrypt((newreg.Password).Trim());
                        relog.email = newreg.Email;
                        relog.role_id = 2;

                        db.Logins.Add(relog);
                        db.SaveChanges();

                        //smtp mail
                        //Configuring webMail class to send emails  
                        //gmail smtp server  
                        WebMail.SmtpServer = "smtp.gmail.com";
                        //gmail port to send emails  
                        WebMail.SmtpPort = 587;
                        WebMail.SmtpUseDefaultCredentials = true;
                        //sending emails with secure protocol  
                        WebMail.EnableSsl = true;
                        //EmailId used to send emails from application  
                        WebMail.UserName = "Spectrumprojectmail@gmail.com";
                        WebMail.Password = "sebin@1234";

                        //Sender email address.  
                        WebMail.From = newreg.Email;

                        //Send email  
                        WebMail.Send(to: newreg.Email, subject: "MyTube Registration Sucessfull", body: "Your MyTube Account is sucessfully registered.welcome to the world of unlimited free videos ", isBodyHtml: true);
                        ViewBag.Status = "Email Sent Successfully.";


                        return RedirectToAction("Login", "Account");
                    }
                }
                return this.View();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}