using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppASV.Models
{
	public class CrewMember : Person
	{
		[Key]
		public int CrewMemberId { get; set; }
		public Dictionary<Series, String> Filmography { get; set; }
	}
}