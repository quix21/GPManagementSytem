using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GPManagementSytem.Helper
{
    public static class ListExtensions
    {
        public static List<SelectListItem> ToSelectList<T>(this List<T> list, string idPropertyName, string namePropertyName = "Value")
            where T : class, new()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();

            selectListItems.Add(new SelectListItem { Text = " -- Please Select -- ", Value = "0" });

            list.ForEach(item =>
            {
                selectListItems.Add(new SelectListItem
                {
                    Text = item.GetType().GetProperty(namePropertyName).GetValue(item).ToString(),
                    Value = item.GetType().GetProperty(idPropertyName).GetValue(item).ToString()
                });
            });

            return selectListItems;
        }
    }
}