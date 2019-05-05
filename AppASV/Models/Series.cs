using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppASV.Models
{
	public class Series
	{
		public int NumberOfSeasons { get; set; }
		public int NumberOfEpisodes { get; set; }
		public List<List<Episode>> Episodes { get; set; }
		public Dictionary<Actor, Character> Cast { get; set; }
		public List<String> Genres { get; set; }
		public double Rating { get; set; }
		public Dictionary<ApplicationUser, String> Reviews { get; set; }
	}
}