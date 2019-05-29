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
		public string Title { get; set; }
		public int Length { get; set; }
		public DateTime AirDate { get; set; }
		[Required]
		public int SeriesId { get; set; }
		public int SeasonNumber { get; set; }
		public int EpisodeNumber { get; set; }

		public Series series { get; set; }

	}
}