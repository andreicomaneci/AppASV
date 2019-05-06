using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppASV.Models
{
	public class Episode
	{
		[Key]
		public int EpisodeId { get; set; }
		[Required]
		public int Title { get; set; }
		public int Length { get; set; }
		public double Rating { get; set; }
		public DateTime AirDate { get; set; }
		[Required]
		public int SeriesId { get; set; }
		public int SeasonNumber { get; set; }
		public int EpisodeNumber { get; set; }

		//public virtual ICollection<Actor> Cast { get; set; }
		public virtual ICollection<Review> Reviews { get; set; }
	}
}