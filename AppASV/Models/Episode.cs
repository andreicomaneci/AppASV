using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppASV.Models
{
	public class Episode
	{
		public Dictionary<Actor, String> Cast { get; set; }
		public Dictionary<CrewMember, String> Crew { get; set; }
		public int Length { get; set; }
		public double Rating { get; set; }
		public Dictionary<ApplicationUser, String> Reviews { get; set; }
		public DateTime AirDate { get; set; }
	}
}