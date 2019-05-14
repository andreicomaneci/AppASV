using AppASV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppASV.Controllers
{
    public class ActorController : Controller
    {
		private ApplicationDbContext db = ApplicationDbContext.Create();
		// GET: Actor
		public ActionResult Index()
        {
			var actors = db.Actors;
			if (TempData.ContainsKey("message"))
			{
				ViewBag.message = TempData["message"].ToString();
			}
			ViewBag.Actors = actors;
			return View();
        }

		public ActionResult Show(int id)
		{
			Actor actor = db.Actors.Find(id);
			ViewBag.Actor = actor;
			var rolesPlayed = db.Characters.Where(x => x.ActorId == id).ToList();
			ViewBag.Series = new List<Tuple<Series, string>>();
			foreach (var x in rolesPlayed)
			{
				Series series = db.Series.Find(x.SeriesId);
				ViewBag.Series.Add(new Tuple<Series, string>(series, x.CharacterName));
			}
			ViewBag.afisareButoane = false;
			if (User.IsInRole("Editor") || User.IsInRole("Administrator"))
			{
				ViewBag.afisareButoane = true;
			}
			ViewBag.esteAdmin = User.IsInRole("Administrator");
			return View(actor);
		}


		public ActionResult New()
		{
			Actor actor = new Actor();
			return View(actor);
		}

		[HttpPost]
		public ActionResult New(Actor actor)
		{
			try
			{
				if (ModelState.IsValid)
				{
					db.Actors.Add(actor);
					db.SaveChanges();
					return RedirectToAction("Index", "Actor");
				}
				else
				{
					return View(actor);
				}
			}
			catch (Exception e)
			{
				return RedirectToAction("Index", "Actor");
			}
		}

		public ActionResult Edit(int id)
		{
			Actor actor = db.Actors.Find(id);
			ViewBag.Actor = actor;

			return View(actor);
		}

		[HttpPut]
		public ActionResult Edit(Actor requestActor)
		{
			try
			{
				if (ModelState.IsValid)
				{
					Actor actor = db.Actors.Find(requestActor.ActorId);

					if (TryUpdateModel(actor))
					{
						actor.FirstName = requestActor.FirstName;
						actor.LastName = requestActor.LastName;
						db.SaveChanges();
						TempData["message"] = "The actor has been updated!";
					}
					return RedirectToAction("Index");

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
		}

		[HttpDelete]
		public ActionResult Delete(int id)
		{
			Actor actor = db.Actors.Find(id);

			db.Actors.Remove(actor);
			db.SaveChanges();
			TempData["message"] = "The actor has been removed!";
			return RedirectToAction("Index");
		}
	}
}