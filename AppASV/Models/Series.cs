using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppASV.Models
{
	public class Series
	{
		[Key]
		public int SeriesId { get; set; }
		[Required]
		public string Title { get; set; }
		public int NumberOfSeasons { get; set; }
		public int NumberOfEpisodes { get; set; }
		public double Rating { get; set; }

		public virtual ICollection<Episode> Episodes { get; set; }
		public virtual ICollection<Role> Cast { get; set; }
		public virtual ICollection<SeriesGenre> SeriesGenres { get; set; }
		public virtual ICollection<Review> Reviews { get; set; }
	}
}