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
			var model = new SeriesViewModel();
			var series = db.Series;
			if (TempData.ContainsKey("message"))
			{
				ViewBag.message = TempData["message"].ToString();
			}
			ViewBag.Series = series;
			return View(model);
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

		[HttpGet]
		public ActionResult New()
		{
			SeriesViewModel model = new SeriesViewModel();
			var allGenres = GetAllGenres(); //returns List<Genre>
			var checkBoxListItems = new List<CheckBoxListItem>();
			foreach (var genre in allGenres)
			{
				checkBoxListItems.Add(new CheckBoxListItem()
				{
					ID = 0,
					Display = genre.Name,
					IsChecked = false //On the add view, no genres are selected by default
				});
			}
			model.Genres = checkBoxListItems;
			return View(model);
		}

		//[HttpPost]
		//public ActionResult New(Series series)
		//{
		//	try
		//	{
		//		if (ModelState.IsValid)
		//		{
		//			db.Series.Add(series);
		//			db.SaveChanges();
		//			TempData["message"] = "The series has been added!";
		//			return RedirectToAction("Index");
		//		}
		//		else
		//		{
		//			return View(series);
		//		}
		//	}
		//	catch (Exception e)
		//	{
		//		return View(series);
		//	}
		//}

		[HttpPost]
		public ActionResult New(SeriesViewModel model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					db.Series.Add(model.Series);
					var selectedGenres = model.Genres.Where(x => x.IsChecked).Select(x => x.Display).ToList();
					foreach (var genreName in selectedGenres)
					{
						SeriesGenre seriesGenre = new SeriesGenre
						{
							SeriesId = model.Series.SeriesId,
							GenreName = genreName
						};
						//seriesGenre.Genre = genre;
						//seriesGenre.Series = model.Series;
						db.SeriesGenres.Add(seriesGenre);
					}
					db.SaveChanges();
					TempData["message"] = "The series has been added!";
					return RedirectToAction("Index");
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

		[NonAction]
		public List<Genre> GetAllGenres()
		{
			var selectList = new List<Genre>();

			var genres = from gen in db.Genres select gen;

			foreach (var genre in genres)
			{
				selectList.Add(genre);
			}

			return selectList;
		}
	}
}