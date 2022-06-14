using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace GPManagementSytem.Helper
{
    public static class ActionLinkEntendedMethods
    {
        /// <summary>
        /// Action link wrapped in list tag.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="text">The text.</param>
        /// <param name="action">The action.</param>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        public static MvcHtmlString LiActionLink(this HtmlHelper html, string text, string action, string controller)
        {
            var str = String.Format("<li {0}>{1}</li>",
                ActionLinkIsCurrentPage(html, action, controller) ?
                " class=\"active\"" :
                String.Empty, html.ActionLink(text, action, controller).ToHtmlString()
            );
            return new MvcHtmlString(str);
        } // http://stackoverflow.com/a/29968637/400983

        /// <summary>
        /// Determines if the link created from the parameters would point to the current page.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <param name="action">The action.</param>
        /// <param name="controller">The controller.</param>
        /// <returns></returns>
        public static bool ActionLinkIsCurrentPage(this HtmlHelper html, string action, string controller)
        {
            var context = html.ViewContext;
            if (context.Controller.ControllerContext.IsChildAction)
                context = html.ViewContext.ParentActionViewContext;
            var routeValues = context.RouteData.Values;
            var currentAction = routeValues["action"].ToString();
            var currentController = routeValues["controller"].ToString();

            return currentAction.Equals(action, StringComparison.InvariantCulture) &&
                   currentController.Equals(controller, StringComparison.InvariantCulture);
        }
    }
}