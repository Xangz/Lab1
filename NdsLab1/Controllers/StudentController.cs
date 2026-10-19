using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NdsLab1.Models;
using static System.Net.Mime.MediaTypeNames;
namespace NdsLab1.Controllers
{
    public class StudentController : Controller
    {
        private static List<Student> listStudents = new List<Student>();
        public StudentController()
        {
            listStudents = new List<Student>()
            {
                new Student(){Id=101,Name ="Hải Nam",Branch=Branch.IT,Gender=Gender.Male,IsRegular=true,Address="A1-2018",Email="nam@g.com"},
                new Student(){Id=102,Name ="Minh Tú",Branch=Branch.BE,Gender=Gender.Female,IsRegular=true,Address="A1-2019",Email="tu@g.com"},
                new Student(){Id=103,Name ="Hoàng Phong",Branch=Branch.CE,Gender=Gender.Male,IsRegular=false,Address="A1-2020",Email="phon@g.com"},
                new Student(){Id=104,Name ="Xuân Mai",Branch=Branch.EE,Gender=Gender.Female,IsRegular=false,Address="A1-2021",Email="mai@g.com"}
            };  
        }
        public IActionResult Index()
        {
            return View(listStudents);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.AllGenders = Enum.GetValues(typeof(Gender)).Cast<Gender>().ToList();
            ViewBag.AllBranches = new List<SelectListItem>()
            {
                new SelectListItem {Text = "IT", Value = "1"},
                new SelectListItem {Text = "BE", Value = "2"},
                new SelectListItem {Text = "CE", Value = "3"},
                new SelectListItem {Text = "EE", Value = "4"}
            };
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Student s)
        {
            if (s.Avatar != null && s.Avatar.Length > 0)
            {
                var fileName = Path.GetFileName(s.Avatar.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await s.Avatar.CopyToAsync(stream);
                }
                s.AvatarPath = "/images/" + fileName;
            }

            s.Id = listStudents.Last().Id + 1;
            listStudents.Add(s);

            return View("Index", listStudents);
        }
    }
}
