using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace sqlCacheDependencyDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {
        string connectionString = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            System.Data.SqlClient.SqlDependency.Start(connectionString);
        }

        protected void Application_End()
        {
            System.Data.SqlClient.SqlDependency.Stop(connectionString);
        }
    }
}
