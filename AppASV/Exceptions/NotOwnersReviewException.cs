using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppASV.Exceptions
{
	using System;

	public class NotOwnersReviewException : Exception
	{
		public NotOwnersReviewException()
		{
		}

		public NotOwnersReviewException(string message)
			: base(message)
		{
		}

		public NotOwnersReviewException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}