﻿
namespace SampleProject.Controllers
{
    using SampleProject.Models;
    using System.Configuration;
    using System.Data.SqlClient;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Net.Http;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Web.UI.WebControls;
    using System.Data;

    [Authorize]
    public class HomeController : Controller
    {
      
        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }
       
        [HttpGet]
        public ActionResult Index(int? fileId)
        {
            List<UploadClass> videolist = new List<UploadClass>();
            string CS = ConfigurationManager.ConnectionStrings["FilesEntities"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("select Id,Name,Path from Files", con);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UploadClass video = new UploadClass();
                   video.Id =int.Parse(rdr["Id"].ToString());
                    video.Name = rdr["Name"].ToString();
                    video.Path = rdr["Path"].ToString();

                    videolist.Add(video);
                }
            }
            return View(videolist);
        }
       
        public ActionResult DownloadFile(int? id)
        {
            string maincon = ConfigurationManager.ConnectionStrings["FilesEntities"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(maincon);
            SqlCommand deletefile = new SqlCommand("select * from Files where Id=" + id, sqlconn);
            sqlconn.Open();
            using (SqlDataReader Reader = deletefile.ExecuteReader())
            {
                while (Reader.Read())
                {
                    var filename = Reader["Name"].ToString();
                    var fullpath = Reader["Path"].ToString();

                    string contentType = "VideoFiles/mp4";

                    return File(fullpath, contentType, filename);
                }
              
            }return RedirectToAction("Index");
          
            
        }
        public ActionResult VideoView(int VideoId,string VideoName)
        {
            try
            {
                int VId = VideoId;
                string Vname = VideoName;
                string maincon = ConfigurationManager.ConnectionStrings["FilesEntities"].ConnectionString;
                SqlConnection sqlconn = new SqlConnection(maincon);

                SqlCommand cmd = new SqlCommand("spVideoCount", sqlconn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("VideoName", Vname);
                cmd.Parameters.AddWithValue("videoId", VId);

                cmd.Parameters.AddWithValue("Viewdate", DateTime.Now.Date);
                sqlconn.Open();
                cmd.ExecuteNonQuery();
                sqlconn.Close();
                return Json("sucess");
                    }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }

        }

        public ActionResult DeleteFile(int deleteId)
        {
            string maincon = ConfigurationManager.ConnectionStrings["FilesEntities"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(maincon);
            SqlCommand deletefile = new SqlCommand("select * from Files where Id=" + deleteId + " ", sqlconn);
            sqlconn.Open();
            using (SqlDataReader oReader = deletefile.ExecuteReader())
            {
                while (oReader.Read())
                {
                    var test = oReader["Name"].ToString();
                    string path = Server.MapPath("~/VideoFiles/" + test);
                    FileInfo file = new FileInfo(path);
                    if (file.Exists)//check file exsit or not
                    {
                        file.Delete();
                    }
                }
                SqlCommand sqlcomm = new SqlCommand("delete from Files where Id = " + deleteId + " ", sqlconn);

                SqlCommand deletevideo = new SqlCommand("delete from videoview where VideoId = " + deleteId + " ", sqlconn);

                sqlcomm.ExecuteNonQuery();
                deletevideo.ExecuteNonQuery();
                sqlconn.Close();
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "1")]
        public ActionResult AdminOnlyLink()
        {
            return View();
        }
  
        }
}