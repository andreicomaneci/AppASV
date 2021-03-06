﻿using AppASV.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppASV.Controllers
{
	public class UsersController : Controller
    {
		private ApplicationDbContext db = ApplicationDbContext.Create();

		[Authorize(Roles = "Administrator")]
		public ActionResult Index()
		{
			var users = from user in db.Users
						orderby user.UserName
						select user;

			ViewBag.UsersList = users;
			return View();
		}

		public ActionResult Show()
		{
			string currentUserId = User.Identity.GetUserId();
			var favouriteSeries = db.FavouriteSeries.Where(x => currentUserId.Equals(x.UserId)).ToList();
			List<Series> seriesList = new List<Series>();
			foreach (FavouriteSeries fs in favouriteSeries)
			{
				Series series = db.Series.Find(fs.SeriesId);
				seriesList.Add(series);
			}
			ViewBag.SeriesList = seriesList.OrderBy(x => x.Title).ToList();
			return View();
		}

		[Authorize(Roles = "Administrator")]
		public ActionResult Edit(string id)
		{
			ApplicationUser user = db.Users.Find(id);
			user.AllRoles = GetAllRoles();
			// ViewBag.userRoles = user.Roles.ToList().First().RoleId;
			var userRole = user.Roles.FirstOrDefault();
			ViewBag.userRole = userRole.RoleId;
			return View(user);
		}

		[NonAction]
		public IEnumerable<SelectListItem> GetAllRoles()
		{
			var selectList = new List<SelectListItem>();
			var roles = from role in db.Roles select role;
			foreach (var role in roles)
			{
				selectList.Add(new SelectListItem
				{
					Value = role.Id.ToString(),
					Text = role.Name.ToString()
				});
			}
			return selectList;
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut]
		public ActionResult Edit(string id, ApplicationUser newData)
		{

			ApplicationUser user = db.Users.Find(id);
			user.AllRoles = GetAllRoles();
			// ViewBag.userRoles = user.Roles.ToList().First().RoleId;
			var userRole = user.Roles.FirstOrDefault();
			ViewBag.userRole = userRole.RoleId;

			try
			{
				ApplicationDbContext context = new ApplicationDbContext();
				var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
				var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


				if (TryUpdateModel(user))
				{
					user.UserName = newData.UserName;
					user.Email = newData.Email;
					user.PhoneNumber = newData.PhoneNumber;

					var roles = from role in db.Roles select role;
					foreach (var role in roles)
					{
						UserManager.RemoveFromRole(id, role.Name);
					}

					var selectedRole = db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
					UserManager.AddToRole(id, selectedRole.Name);

					db.SaveChanges();
				}
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Response.Write(e.Message);
				return View(user);
			}

		}
	}
}