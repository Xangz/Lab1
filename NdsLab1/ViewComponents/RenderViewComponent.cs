using Microsoft.AspNetCore.Mvc;
using NdsLab1.Models;

namespace NdsLab1.ViewComponents
{
    public class RenderViewComponent : ViewComponent
    {
        private List<MenuItem> MenuItems = new List<MenuItem>();

        public RenderViewComponent()
        {
            MenuItems = new List<MenuItem>()
            {
                new MenuItem() { Id = 1, Name = "Branches", Link = "Admin/Student/List" },
                new MenuItem() { Id = 2, Name = "Students", Link = "Admin/Student/List" },
                new MenuItem() { Id = 3, Name = "Subjects", Link = "Admin/Student/List" },
                new MenuItem() { Id = 4, Name = "Courses", Link = "Admin/Student/List" }
            };
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("RenderLeftMenu", MenuItems);
        }
    }
}