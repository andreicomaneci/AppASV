using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AppASV.Models
{
	public class SeriesGenre
	{
		[Key][Column(Order=0)]
		public int SeriesId { get; set; }
		[Key][Column(Order=1)]
		public string GenreName { get; set; }

		[ForeignKey("GenreName")]
		public Genre Genre { get; set; }
		[ForeignKey("SeriesId")]
		public Series Series { get; set; }
	}
}