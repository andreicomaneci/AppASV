using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AppASV.Models
{
	public class Review
	{
		[Key][Column(Order=0)]
		public string UserId { get; set; }
		[Key][Column(Order=1)]
		public int SeriesId { get; set; }
		public string Text { get; set; }
		public int Stars { get; set; }

		public ApplicationUser User { get; set; }
		public Series Series { get; set; }

	}
}