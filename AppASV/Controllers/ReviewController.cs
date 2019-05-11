using AppASV.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppASV.Controllers
{
    public class ReviewController : Controller
    {
		private ApplicationDbContext db = ApplicationDbContext.Create();
		// GET: Review
		public ActionResult Index()
        {
            return View();
        }
		public ActionResult New(int id)
		{
			ViewBag.Message = id;
			return View();
		}

		[HttpPost]
		public ActionResult New(Review review)
		{
			review.UserId = User.Identity.GetUserId();
			try
			{
				if (ModelState.IsValid)
				{
					db.Reviews.Add(review);
					db.SaveChanges();
					return RedirectToAction("Index", "Series");
				}
				else
				{
					return View(review);
				}
			}
			catch (Exception e)
			{
				return RedirectToAction("Index", "Series");
			}
		}

		//public ActionResult Edit(int id)
		//{
		//	Review review = db.Reviews.SingleOrDefault(x => (x.UserId == User.Identity.GetUserId() && x.SeriesId == id));
		//	ViewBag.Review = review;

		//	return View();
		//}

		[HttpGet]
		public ActionResult Edit(int idSeries, string idUser)
		{
			Review review = db.Reviews.SingleOrDefault(x => (x.UserId == idUser && x.SeriesId == idSeries));
			ViewBag.Review = review;

			return View(review);
		}

		[HttpPost]
		public ActionResult Edit(Review requestReview)
		{
			try
			{
				if (ModelState.IsValid)
				{
					string userId = requestReview.UserId;
					int seriesId = requestReview.SeriesId;
					Review review = db.Reviews.SingleOrDefault(x => (x.UserId == requestReview.UserId
												&& x.SeriesId == requestReview.SeriesId));
					if (TryUpdateModel(review))
					{
						review.Stars = requestReview.Stars;
						review.Text = requestReview.Text;
						db.SaveChanges();
					}
					return RedirectToAction("Index", "Series");
				}
				else
				{
					return RedirectToAction("Index", "Series");
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Series");
			}
		}

		//public ActionResult Delete(int id)
		//{
		//	Review review = db.Reviews.SingleOrDefault(x => (x.UserId == User.Identity.GetUserId() && x.SeriesId == id));
		//	if (review != null)
		//	{
		//		db.Reviews.Remove(review);
		//		db.SaveChanges();
		//	}
		//	return RedirectToAction("Index", "Series");
		//}

		public ActionResult Delete(int idSeries, string idUser)
		{
			Review review = db.Reviews.SingleOrDefault(x => (x.UserId == idUser && x.SeriesId == idSeries));
			if (idUser == User.Identity.GetUserId() || User.IsInRole("Administrator") == true)
			{
				db.Reviews.Remove(review);
				db.SaveChanges();
			}
			return RedirectToAction("Index", "Series");
		}
	}
}