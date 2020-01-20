using SampleProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace SampleProject.Controllers
{
    public class ChartController : Controller
    {
        // GET: Chart
        [Authorize(Roles = "1")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "1")]
        public ActionResult ShowChart(DateTime selecteddate)
        {
            string DateString = selecteddate.ToString("yyyy-MM-dd");


            var Ls = new List<VideoChart>();
            string maincon = ConfigurationManager.ConnectionStrings["FilesEntities"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(maincon);
            SqlCommand chart = new SqlCommand("select * from videoview where ViewDate= '" + DateString + " ' ", sqlconn);
            sqlconn.Open();

            using (SqlDataReader Reader = chart.ExecuteReader())
            {
                while (Reader.Read())
                {
                   
                    var test = new VideoChart();
                    test.VideoName = Reader["VideoName"].ToString();
                    test.NoOfView = int.Parse(Reader["NoOfView"].ToString());
                    test.ViewDate = (Reader["ViewDate"].ToString());
                    Ls.Add(test);
                }
            }

            return Json(Ls);


        }
        public ActionResult ShowChart1(DateTime selecteddate)
        {
            string DateString = selecteddate.ToString("yyyy-MM-dd");


            var Ls = new List<VideoChart>();
            string maincon = ConfigurationManager.ConnectionStrings["FilesEntities"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(maincon);
            SqlCommand chart = new SqlCommand("select * from videoview where ViewDate= '" + DateString + " ' ", sqlconn);
            sqlconn.Open();

            using (SqlDataReader Reader = chart.ExecuteReader())
            {
                while (Reader.Read())
                {

                    var test = new VideoChart();
                    test.VideoName = Reader["VideoName"].ToString();
                    test.NoOfView = int.Parse(Reader["NoOfView"].ToString());
                    test.ViewDate = (Reader["ViewDate"].ToString());
                    test.downloadcount = int.Parse(Reader["downloadcount"].ToString());
                    Ls.Add(test);
                }
            }

            return Json(Ls);


        }


    }
}