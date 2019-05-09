using AppASV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppASV.Controllers
{
    public class GenreController : Controller
    {
		private ApplicationDbContext db = ApplicationDbContext.Create();
		// GET: Genre
		public ActionResult Index()
        {
			var genres = db.Genres;
			if (TempData.ContainsKey("message"))
			{
				ViewBag.message = TempData["message"].ToString();
			}
			ViewBag.Genres = genres;
			return View();
        }

		public ActionResult Show(string name)
		{
			Genre genre = db.Genres.Find(name);
			ViewBag.Genre = genre;
			var seriesWithGenre = db.SeriesGenres.Where(x => x.GenreName == name).ToList();
			ViewBag.Series = new List<Series>();
			foreach (var x in seriesWithGenre)
			{
				Series series = db.Series.Find(x.SeriesId);
				ViewBag.Series.Add(series);
			}
			ViewBag.afisareButoane = false;
			if (User.IsInRole("Editor") || User.IsInRole("Administrator"))
			{
				ViewBag.afisareButoane = true;
			}
			ViewBag.esteAdmin = User.IsInRole("Administrator");
			return View(genre);
		}

		public ActionResult New()
		{
			Genre genre = new Genre();
			return View(genre);
		}

		[HttpPost]
		public ActionResult New(Genre genre)
		{
			try
			{
				if (ModelState.IsValid)
				{
					db.Genres.Add(genre);
					db.SaveChanges();
					TempData["message"] = "The series has been added!";
					return RedirectToAction("Index");
				}
				else
				{
					return View(genre);
				}
			}
			catch (Exception e)
			{
				return View(genre);
			}
		}

		public ActionResult Edit(string name)
		{
			Genre genre = db.Genres.Find(name);
			ViewBag.Genre = genre;

			return View(genre);
		}

		[HttpPut]
		public ActionResult Edit(string name, Genre requestGenre)
		{
			try
			{
				if (ModelState.IsValid)
				{
					Genre genre = db.Genres.Find(name);

					if (TryUpdateModel(genre))
					{
						genre.Name = requestGenre.Name;
						db.SaveChanges();
						TempData["message"] = "The genre has been edited!";
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
		public ActionResult Delete(string name)
		{
			Genre genre = db.Genres.Find(name);

			db.Genres.Remove(genre);
			db.SaveChanges();
			TempData["message"] = "The genre has been deleted!";
			return RedirectToAction("Index");
		}
	}
}