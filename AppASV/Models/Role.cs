using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AppASV.Models
{
	public class Role
	{
		[Key][Column(Order=0)]
		public int SeriesId { get; set; }
		[Key][Column(Order=1)]
		public int ActorId { get; set; }
		public string CharacterName { get; set; }

		public Series Series { get; set; }
		public Actor Actor { get; set; }
	}
}