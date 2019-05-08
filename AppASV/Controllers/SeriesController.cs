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
			ViewBag.Reviews = db.Reviews.Where(x => x.SeriesId == id);
			var genres = db.SeriesGenres.Where(x => x.SeriesId == id);
			ViewBag.Genres = "";
			foreach (var genre in genres)
			{
				if (ViewBag.Genres == "")
					ViewBag.Genres = genre.GenreName;
				else
					ViewBag.Genres += ", " + genre.GenreName;
			}
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

		[HttpGet]
		public ActionResult Edit(int id)
		{
			var series = db.Series.Find(id);
			var model = new SeriesViewModel()
			{
				Series = series
			};
			var seriesGenres = db.SeriesGenres.Where(x => x.SeriesId == id).ToList();
			var genres = new List<Genre>();
			foreach (var seriesGenre in seriesGenres)
			{
				var genre = db.Genres.Find(seriesGenre.GenreName);
				genres.Add(genre);
			}
			var allGenres = GetAllGenres();
			var checkBoxListItems = new List<CheckBoxListItem>();
			foreach (var genre in allGenres)
			{
				checkBoxListItems.Add(new CheckBoxListItem()
				{
					ID = 0,
					Display = genre.Name,
					//We should have already-selected genres be checked
					IsChecked = genres.Where(x => x.Name == genre.Name).Any()
				});
			}
			model.Genres = checkBoxListItems;
			return View(model);
		}

		[HttpPost]
		public ActionResult Edit(SeriesViewModel model)
		{
			var selectedGenres = model.Genres.Where(x => x.IsChecked).Select(x => x.Display).ToList();
			try
			{
				Series series = db.Series.Find(model.Series.SeriesId);
				if (TryUpdateModel(series))
				{
					var toDelete = db.SeriesGenres.Where(x => selectedGenres.Contains(x.GenreName) == false
							&& x.SeriesId == series.SeriesId).ToList();
					foreach (var item in toDelete)
					{
						db.SeriesGenres.Remove(item);
					}
					var alreadyIn = db.SeriesGenres.Where(x => selectedGenres.Contains(x.GenreName) == true
							&& x.SeriesId == series.SeriesId).ToList();
					var genresAlreadyIn = alreadyIn.Select(x => x.GenreName).ToList();
					foreach (var genre in selectedGenres)
					{
						if (genresAlreadyIn.Contains(genre))
							continue;
						SeriesGenre seriesGenre = new SeriesGenre()
						{
							SeriesId = model.Series.SeriesId,
							GenreName = genre
						};
						db.SeriesGenres.Add(seriesGenre);
					}
					series.Title = model.Series.Title;
					series.NumberOfEpisodes = model.Series.NumberOfEpisodes;
					series.NumberOfSeasons = model.Series.NumberOfSeasons;
					db.SaveChanges();
				}
				else
				{
					return RedirectToAction("Index");
				}
			}
			catch (Exception e)
			{
				return RedirectToAction("Index");
			}
			return RedirectToAction("Index");
		}

		//[HttpPut]
		//public ActionResult Edit(int id, SeriesViewModel requestSeries)
		//{
		//	try
		//	{
		//		if (ModelState.IsValid)
		//		{
		//			Series series = db.Series.Find(id);
		//			var seriesGenres = db.SeriesGenres.Where(x => x.SeriesId == id).ToList();
		//			var genres = new List<Genre>();
		//			foreach (var seriesGenre in seriesGenres)
		//			{
		//				var genre = db.Genres.Find(seriesGenre.GenreName);
		//				genres.Add(genre);
		//			}
		//			List<CheckBoxListItem> genresCheckBox = new List<CheckBoxListItem>();
		//			var allGenres = GetAllGenres();
		//			foreach (var genre in allGenres)
		//			{
		//				if (genres.Contains(genre))
		//				{
		//					CheckBoxListItem checkBox = new CheckBoxListItem()
		//					{
		//						ID = 0,
		//						Display = genre.Name,
		//						IsChecked = true
		//					};
		//					genresCheckBox.Add(checkBox);
		//				}
		//				else
		//				{
		//					CheckBoxListItem checkBox = new CheckBoxListItem()
		//					{
		//						ID = 0,
		//						Display = genre.Name,
		//						IsChecked = false
		//					};
		//					genresCheckBox.Add(checkBox);
		//				}
		//			}
		//			SeriesViewModel model = new SeriesViewModel()
		//			{
		//				Series = series,
		//				Genres = genresCheckBox
		//			};

		//			if (TryUpdateModel(series))
		//			{
		//				series.Title = requestSeries.Series.Title;
		//				series.NumberOfEpisodes = requestSeries.Series.NumberOfEpisodes;
		//				series.NumberOfSeasons = requestSeries.Series.NumberOfSeasons;
		//				db.SaveChanges();
		//				TempData["message"] = "The series has been edited!";
		//			}
		//			return RedirectToAction("Index");

		//		}
		//		else
		//		{
		//			return View();
		//		}

		//	}
		//	catch (Exception e)
		//	{
		//		return View();
		//	}
		//}

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