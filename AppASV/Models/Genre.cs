using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppASV.Models
{
	public class Genre
	{
		[Key]
		public string Name { get; set; }

		public virtual ICollection<SeriesGenre> SeriesGenre { get; set; }
	}
}