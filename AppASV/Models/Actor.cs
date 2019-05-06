using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AppASV.Models
{
	public class Actor
	{
		[Key]
		public int ActorId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public virtual ICollection<Role> Roles { get; set; }
	}
}