using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using DesafioGeoFusion.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.SqlServerCe;
using System.Data;

namespace DesafioGeoFusion.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public bool SaveEmail(string email)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection cn = new SqlConnection(connectionString);
            //SqlConnection cn = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=C:\Users\Michel\documents\visual studio 2010\Projects\DesafioGeoFusion\DesafioGeoFusion\App_Data\Database.mdf;Integrated Security=True;User Instance=True");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;
            
            try
            {
                cn.Open();
                cmd.CommandText = "insert into Contacts (email) values ('" + email + "')";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public bool SaveSurvey(string email, string question1, string question2, string question3)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection cn = new SqlConnection(connectionString);
            //SqlConnection cn = new SqlConnection(@"Data Source=.\SQLEXPRESS;AttachDbFilename=C:\Users\Michel\documents\visual studio 2010\Projects\DesafioGeoFusion\DesafioGeoFusion\App_Data\Database.mdf;Integrated Security=True;User Instance=True");
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cn;

            try
            {
                cn.Open();
                cmd.CommandText = "update [Contacts] set Question1 = '" + question1 + "', Question2 = '"+ question2+"', Question3 = '"+question3+"' where Email = '"+email+"'";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return false;
            }

            return true;

        }
        [HttpPost]
        public ActionResult Index(EmailModels emailModels)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            
            try
            {
                SaveEmail(emailModels.Email);

                string url = Request.Url.ToString();
                url += "Home/EmailResponse";

                MailMessage mail = new MailMessage(){};
                mail.To.Add(emailModels.Email);
                mail.From = new MailAddress("Empresa@Teste");
                mail.Subject = "Thank you for subscribing to Lucky's newsletter!";
                mail.IsBodyHtml = true;
                mail.Body = "<html><body><form action='"+url+"' method='post' target='_blank'><label>How did you like the movie <strong>Turfnuts</strong>?</label><br /><input name='Email' type='hidden' value='"+emailModels.Email+"' /><label for='Question1'>What do you expect from our company?</label><br /><textarea cols='75' name='Question1' rows='5'></textarea><br /><br /><label for='Question2'>How much would you pay for our services?</label><br /><textarea cols='75' name='Question2' rows='5'></textarea><br /><br /><label for='Question3'>What do you really need?</label><br /><textarea cols='75' name='Question3' rows='5'></textarea><br /><br /><input type='submit' value='Submit your survey.' />&nbsp;</form></body></html>";
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                //587
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("mintchel@gmail.com", "Elsalvador");
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }
            catch(Exception ex){
                ViewBag.Title = "Erro.";
                ViewBag.Message = ex.Message;
            }
            ViewBag.Title = "Thank you!";
            ViewBag.Message = "We'll be contacting you soon! Please take a moment to answer our e-mail survey. ;)";
            return View("Subscribe");
        }

        [HttpPost]
        public ActionResult EmailResponse(FormCollection form)
        {
            string question1, question2, question3, email;

            question1 = Request.Form["Question1"];
            question2 = Request.Form["Question2"];
            question3 = Request.Form["Question3"];
            email = Request.Form["Email"];
            try
            {
                SaveSurvey(email, question1, question2, question3);
            }
            catch(Exception ex)
            {
                ViewBag.Title = "Erro.";
                ViewBag.Message = ex.Message; 
                return View("Subscribe");
            }
            ViewBag.Title = "Survey submitted!";
            ViewBag.Message = "Thanks for participating on our survey.";
            return View("Subscribe");
        }

    }
}
