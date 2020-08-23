using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AcademicPortal.Attributes
{
	public class AccessAuthorize : AuthorizeAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{

			if (this.AuthorizeCore(filterContext.HttpContext))
				base.OnAuthorization(filterContext);
			else
			{
				var user = filterContext.HttpContext.User;

				if (user.Identity.IsAuthenticated)
					filterContext.Result = new RedirectResult("~/Error/AccessDenied");
				else
					filterContext.Result = new RedirectResult("~/Account/Login");
			}
		}
	}
}