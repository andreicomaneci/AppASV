﻿using AppASV.Exceptions;
using AppASV.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AppASV.Controllers
{
    public class SeriesController : Controller
    {
		private ApplicationDbContext db = ApplicationDbContext.Create();
		// GET: Series
		//public ActionResult Index()
		//      {
		//	var model = new SeriesViewModel();
		//	var series = db.Series;
		//	if (TempData.ContainsKey("message"))
		//	{
		//		ViewBag.message = TempData["message"].ToString();
		//	}
		//	ViewBag.Series = series;
		//	return View(model);
		//      }

		[Authorize(Roles = "User,Editor,Administrator")]
		public ActionResult Index(string searchString)
		{
			var model = new SeriesViewModel();
			var series = db.Series.OrderBy(x => x.Title).ToList();

			if (!String.IsNullOrEmpty(searchString))
			{
				series = db.Series.Where(s => s.Title.Contains(searchString)).ToList();
			}
			ViewBag.Series = series;
			ViewBag.afisareButoane = false;
			if (User.IsInRole("Editor") || User.IsInRole("Administrator"))
			{
				ViewBag.afisareButoane = true;
			}
			return View(model);
		}

		[Authorize(Roles = "User,Editor,Administrator")]
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
			ViewBag.Episodes = GetAllEpisodes(id);
			SeriesViewModel model = new SeriesViewModel
			{
				Series = series,
				Genres = new List<CheckBoxListItem>(),
				ActorList = "",
				IsFavourite = false
			};

			string currentUserId = User.Identity.GetUserId();
			if (db.FavouriteSeries.Any(u => u.SeriesId == series.SeriesId && currentUserId.Equals(u.UserId)))
			{
				model.IsFavourite = true;
			}
			else
			{
				model.IsFavourite = false;
			}

			List<Role> characters = db.Characters.Where(x => x.SeriesId == id).OrderBy(x => x.CharacterName).ToList();
			ViewBag.Cast = new List<Tuple<int, string, string>>();
			foreach (Role character in characters)
			{
				Actor actor = db.Actors.Find(character.ActorId);
				string actorName = actor.FirstName + " " + actor.LastName;
				ViewBag.Cast.Add(new Tuple<int, string, string>(character.ActorId, actorName, character.CharacterName));
			}

			return View(model);
		}

		[Authorize(Roles = "Editor,Administrator")]
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

		[Authorize(Roles = "Editor,Administrator")]
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
					List <Role> characters = GetCharacters(model.Series.SeriesId, model.ActorList);
					foreach (Role role in characters)
					{
						db.Characters.Add(role);
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

		[Authorize(Roles = "Editor,Administrator")]
		[HttpGet]
		public ActionResult Edit(int id)
		{
			var series = db.Series.Find(id);
			List<Role> characters = db.Characters.Where(x => x.SeriesId == id).ToList();
			string actorList = ToActorList(characters);
			var model = new SeriesViewModel()
			{
				Series = series,
				ActorList = actorList
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

		[Authorize(Roles = "User,Editor,Administrator")]
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

					foreach (var chr in db.Characters.Where(x => x.SeriesId == model.Series.SeriesId))
					{
						db.Characters.Remove(chr);
					}

					List<Role> characers = GetCharacters(model.Series.SeriesId, model.ActorList);
					foreach (Role role in characers)
					{
						db.Characters.Add(role);
					}

					series.Title = model.Series.Title;
					series.NumberOfEpisodes = model.Series.NumberOfEpisodes;
					series.NumberOfSeasons = model.Series.NumberOfSeasons;
					db.SaveChanges();
				}
				else
				{
					return RedirectToAction("Show", "Series", new { @id = model.Series.SeriesId});
				}
			}
			catch (Exception e)
			{
				return RedirectToAction("Show", "Series", new { @id = model.Series.SeriesId });
			}
			return RedirectToAction("Show", "Series", new { @id = model.Series.SeriesId });
		}

		[Authorize(Roles = "Editor,Administrator")]
		[HttpDelete]
		public ActionResult Delete(int id)
		{
			Series series = db.Series.Find(id);

			db.Series.Remove(series);
			db.SaveChanges();
			TempData["message"] = "The series has been deleted!";
			return RedirectToAction("Index");
		}

		[Authorize(Roles = "User,Editor,Administrator")]
		public ActionResult Unfavourite(int id)
		{
			Series series = db.Series.Find(id);
			try
			{
				string currentUserId = User.Identity.GetUserId();
				if (db.FavouriteSeries.Any(u => u.SeriesId == id && currentUserId.Equals(u.UserId)))
				{
					FavouriteSeries fs = db.FavouriteSeries.
						Where(u => u.SeriesId == id && currentUserId.Equals(u.UserId)).FirstOrDefault();
					db.FavouriteSeries.Remove(fs);
					db.SaveChanges();
				}
			}
			catch (Exception e)
			{
				return RedirectToAction("Show", new { id = id });
			}
			return RedirectToAction("Show", new { id = id });
		}

		[Authorize(Roles = "User,Editor,Administrator")]
		public ActionResult Favourite(int id)
		{
			Series series = db.Series.Find(id);
			try
			{
				string currentUserId = User.Identity.GetUserId();
				if (db.FavouriteSeries.Any(u => u.SeriesId == id && currentUserId.Equals(u.UserId)))
				{

				}
				else
				{
					FavouriteSeries fs = new FavouriteSeries
					{
						SeriesId = id,
						UserId = User.Identity.GetUserId()
					};

					db.FavouriteSeries.Add(fs);
					db.SaveChanges();
				}
			}
			catch (Exception e)
			{
				return RedirectToAction("Show", new { id = id });
			}
			return RedirectToAction("Show", new { id = id });
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

		[NonAction]
		public List<Role> GetCharacters(int seriesId, string charList)
		{
			List<Role> characters = new List<Role>();
			if (charList == null)
				return characters;
			string[] entries = charList.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < entries.Length; ++i)
			{
				//if (entries[i].Equals(" ")) continue;
				string[] data = entries[i].Split(new string[] { ", ", "; "}, StringSplitOptions.RemoveEmptyEntries);
				string firstName = data[1];
				string lastName = data[0];
				Actor actor = db.Actors.Where(x => firstName.Equals(x.FirstName) && lastName.Equals(x.LastName)).FirstOrDefault();
				if (actor == null)
					throw new ActorNotFoundException();
				Role role = new Role
				{
					ActorId = actor.ActorId,
					SeriesId = seriesId,
					CharacterName = data[2]
				};
				characters.Add(role);
			}
			return characters;
		}

		[NonAction]
		public string ToActorList(List<Role> characters)
		{
			string solution = "";
			foreach (Role character in characters)
			{
				Actor actor = db.Actors.Find(character.ActorId);
				solution += actor.LastName + ", " + actor.FirstName + "; " + character.CharacterName + "\n";
			}
			return solution;
		}

		[NonAction]
		public List<Episode> GetAllEpisodes(int id)
		{
			List<Episode> list = db.Episodes.Where(x => x.SeriesId == id).OrderBy(x => x.SeasonNumber).ThenBy(x => x.EpisodeNumber).ToList();
			return list;
		}
	}
}