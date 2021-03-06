﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppASV.Models
{
	public class SeriesViewModel
	{
		public Series Series { get; set; }
		public List<CheckBoxListItem> Genres { get; set; }

		public string ActorList { get; set; }

		public bool IsFavourite { get; set; }
		public SeriesViewModel()
		{
			Genres = new List<CheckBoxListItem>();
			IsFavourite = false;
		}
	}
}