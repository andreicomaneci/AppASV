using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppASV.Exceptions
{
	using System;

	public class ActorNotFoundException : Exception
	{
		public ActorNotFoundException()
		{
		}

		public ActorNotFoundException(string message)
			: base(message)
		{
		}

		public ActorNotFoundException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}