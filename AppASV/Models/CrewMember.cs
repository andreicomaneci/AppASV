using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppASV.Models
{
	public class CrewMember : Person
	{
		public Dictionary<Series, String> Filmography { get; set; }
	}
}