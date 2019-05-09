﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AppASV
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "GenreRoute",
				url: "Genre/{action}/{name}",
				defaults: new { controller = "Genre", action = "Index", name = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "DoublePKroute",
				url: "{controller}/{action}/{id1}/{id2}",
				defaults: new { controller = "Home", action = "Index", id1 = UrlParameter.Optional, id2 = UrlParameter.Optional }
			);
		}
	}
}
