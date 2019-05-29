using AppASV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace AppASV.Controllers
{
    public class EpisodeController : Controller
    {
		private ApplicationDbContext db = ApplicationDbContext.Create();
		public ActionResult Index()
        {
			var episodes = db.Episodes.ToList();
			ViewBag.Episodes = episodes;
            return View();
        }

		public ActionResult Show(int id)
		{
			Episode episode = db.Episodes.Find(id);
			Series series = db.Series.Find(episode.SeriesId);
			
			ViewBag.Episode = episode;
			ViewBag.Series = series;
			ViewBag.afisareButoane = false;
			
			if (User.IsInRole("Editor") || User.IsInRole("Administrator"))
			{
				ViewBag.afisareButoane = true;
			}
			ViewBag.esteAdmin = User.IsInRole("Administrator");
			return View(episode);
		}

		[HttpGet]
		public ActionResult New()
		{
			Episode model = new Episode();
			ViewBag.SeriesList = GetAllSeries();
			return View(model);
		}

		[HttpPost]
		public ActionResult New(Episode model)
		{
			try
			{
				ViewBag.SeriesList = GetAllSeries();
				if (ModelState.IsValid)
				{
					db.Episodes.Add(model);
					
					db.SaveChanges();
					TempData["message"] = "The episode has been added!";
					return RedirectToAction("Show", "Series", new { @id = model.SeriesId });
				}
				else
				{
					return View(model);
				}
			}
			catch (Exception e)
			{
				return View(model);
			}
		}

		[HttpGet]
		public ActionResult Edit(int id)
		{
			Episode episode = db.Episodes.Find(id);
			Series series = db.Series.Find(episode.SeriesId);
			ViewBag.Series = series;
			ViewBag.SeriesList = GetAllSeries();
			return View(episode);
		}

		[HttpPost]
		public ActionResult Edit(Episode model)
		{
			try
			{
				ViewBag.SeriesList = GetAllSeries();
				Episode episode = db.Episodes.Find(model.EpisodeId);
				if (TryUpdateModel(episode))
				{
					episode.Title = model.Title;
					episode.Length = model.Length;
					episode.AirDate = model.AirDate;
					episode.EpisodeNumber = model.EpisodeNumber;
					episode.SeasonNumber = model.SeasonNumber;
					db.SaveChanges();
				}
				else
				{
					return RedirectToAction("Show", "Series", new { @id = model.SeriesId });
				}
			}
			catch (Exception e)
			{
				return RedirectToAction("Show", "Series", new { @id = model.SeriesId });
			}
			return RedirectToAction("Show", "Series", new { @id = model.SeriesId });
		}

		[HttpDelete]
		public ActionResult Delete(int id)
		{
			Episode episode = db.Episodes.Find(id);

			db.Episodes.Remove(episode);
			db.SaveChanges();
			TempData["message"] = "The episode has been deleted!";
			return RedirectToAction("Show", "Series", new { @id = episode.SeriesId });
		}


		[NonAction]
		public IEnumerable<SelectListItem> GetAllSeries()
		{
			var selectList = new List<SelectListItem>();
			
			var allSeries = from series in db.Series select series;
			
			foreach (var series in allSeries)
			{
				selectList.Add(new SelectListItem
				{
					Value = series.SeriesId.ToString(),
					Text = series.Title
				});
			}
			
			return selectList;
		}
	}
}