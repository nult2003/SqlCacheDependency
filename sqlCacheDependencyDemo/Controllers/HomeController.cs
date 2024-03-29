﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace sqlCacheDependencyDemo.Controllers
{
    public class HomeController : Controller
    {
        private string ConnectionString = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
        // GET: Home
        public ActionResult Index()
        {
            LoadMessages();
            return View();
        }

        public ActionResult Message()
        {
            var result = string.Empty;
            var sb = new StringBuilder();
            JavaScriptSerializer ser = new JavaScriptSerializer();
            var serializedObject = ser.Serialize(new { item = "minh nhat", message = "hello" });
            sb.AppendFormat("data: {0}\n\n", serializedObject);

            return Content(sb.ToString(), "text/event-stream");
        }

        public ActionResult CheckSqlCache()
        {
            DataTable dtMessages = (DataTable)HttpContext.Cache.Get("Messages");
            return RedirectToAction("Index");
            //return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        private DataTable LoadMessages()
        {

            DataTable dtMessages = new DataTable();

            using (SqlConnection connection = new SqlConnection(this.ConnectionString))
            {
                SqlCommand command = new SqlCommand("Select [Code],[Name] from dbo.DropDown", connection);

                //SqlCacheDependency dependency = new SqlCacheDependency(command);
                SqlDependency dependency = new SqlDependency(command);
                dependency.OnChange += this.dependency_OnChange;
                
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                dtMessages.Load(command.ExecuteReader(CommandBehavior.CloseConnection));
                HttpContext.Cache.Insert("Messages", dtMessages);
                //Cache.Insert("Messages", dtMessages, dependency);
            }

            return dtMessages;

        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDependency dependency = sender as SqlDependency;
            dependency.OnChange -= new OnChangeEventHandler(dependency_OnChange);
            
        }


    }
}