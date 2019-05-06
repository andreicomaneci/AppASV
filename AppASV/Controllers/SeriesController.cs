using AppASV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppASV.Controllers
{
    public class SeriesController : Controller
    {
		private ApplicationDbContext db = ApplicationDbContext.Create();
		// GET: Series
		public ActionResult Index()
        {
			var series = db.Series;
			if (TempData.ContainsKey("message"))
			{
				ViewBag.message = TempData["message"].ToString();
			}
			ViewBag.Series = series;
			return View();
        }

		public ActionResult Show(int id)
		{
			Series series = db.Series.Find(id);
			ViewBag.Series = series;
			ViewBag.afisareButoane = false;
			if (User.IsInRole("Editor") || User.IsInRole("Administrator"))
			{
				ViewBag.afisareButoane = true;
			}
			ViewBag.esteAdmin = User.IsInRole("Administrator");
			return View(series);
		}

		public ActionResult New()
		{
			Series series = new Series();
			return View(series);
		}

		[HttpPost]
		public ActionResult New(Series series)
		{
			try
			{
				if (ModelState.IsValid)
				{
					db.Series.Add(series);
					db.SaveChanges();
					TempData["message"] = "The series has been added!";
					return RedirectToAction("Index");
				}
				else
				{
					return View(series);
				}
			}
			catch (Exception e)
			{
				return View(series);
			}
		}

		public ActionResult Edit(int id)
		{

			Series series = db.Series.Find(id);
			ViewBag.Series = series;

			return View(series);
		}

		[HttpPut]
		public ActionResult Edit(int id, Series requestSeries)
		{
			try
			{
				if (ModelState.IsValid)
				{
					Series series = db.Series.Find(id);

					if (TryUpdateModel(series))
					{
						series.Title = requestSeries.Title;
						series.NumberOfEpisodes = requestSeries.NumberOfEpisodes;
						series.NumberOfSeasons = requestSeries.NumberOfSeasons;
						db.SaveChanges();
						TempData["message"] = "The series has been edited!";
					}
					return RedirectToAction("Index");

				}
				else
				{
					return View();
				}

			}
			catch (Exception e)
			{
				return View();
			}
		}

		[HttpDelete]
		public ActionResult Delete(int id)
		{
			Series series = db.Series.Find(id);

			db.Series.Remove(series);
			db.SaveChanges();
			TempData["message"] = "The series has been deleted!";
			return RedirectToAction("Index");
		}
	}
}