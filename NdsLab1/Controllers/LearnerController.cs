using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NdsLab1.Data;
using NdsLab1.Models;

namespace NdsLab1.Controllers
{
    public class LearnerController : Controller
    {
        private readonly SchoolContext db;

        public LearnerController(SchoolContext context)
        {
            db = context;
        }
        private int pageSize = 3;
        public IActionResult Index(int? mid)
        {
            if (mid == null)
            {
                var learners = db.Learners.Include(m => m.Major).ToList();
                return View(learners);
            }
            else
            {
                var learners = db.Learners
                                 .Where(l => l.MajorID == mid)
                                 .Include(m => m.Major).ToList();
                return View(learners);
            }
        }
        public IActionResult LearnerFilter(int? mid, string? keyword, int? pageIndex)
        {
            // Lấy toàn bộ learners trong dbset chuyển về IQueryable<Learner> để query[cite: 36]
            var learners = (IQueryable<Learner>)db.Learners;

            // Lấy chỉ số trang, nếu chỉ số trang null thì gán ngầm định bằng 1[cite: 36]
            int page = (int)(pageIndex == null || pageIndex <= 0 ? 1 : pageIndex);

            // Nếu có mid thì lọc learner theo mid (chuyên ngành)[cite: 36]
            if (mid != null)
            {
                learners = learners.Where(l => l.MajorID == mid);
                ViewBag.mid = mid; // Gửi mid về view để ghi lại trên nav-phân trang[cite: 36]
            }

            // Nếu có keyword thì tìm kiếm theo tên[cite: 36]
            if (keyword != null)
            {
                learners = learners.Where(l => l.FirstMidName.ToLower().Contains(keyword.ToLower()));
                ViewBag.keyword = keyword; // Gửi keyword về view để ghi lại trên nav-phân trang[cite: 36]
            }

            // Tính số trang[cite: 36]
            int pageNum = (int)Math.Ceiling(learners.Count() / (float)pageSize);
            ViewBag.pageNum = pageNum; // Gửi số trang về view để hiển thị nav-trang[cite: 36]

            // Chọn dữ liệu trong trang hiện tại[cite: 36]
            var result = learners.Skip(pageSize * (page - 1))
                                 .Take(pageSize)
                                 .Include(m => m.Major);

            return PartialView("LearnerTable", result);
        }
        public IActionResult LearnerByMajorID(int mid)
        {
            var learners = db.Learners
                             .Where(l => l.MajorID == mid)
                             .Include(m => m.Major).ToList();
            return PartialView("LearnerTable", learners);
        }
        public IActionResult Create()
        {
            // Tạo SelectList để hiển thị danh sách chuyên ngành ra DropdownList
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName");
            return View();
        }
        public IActionResult Edit(int id)
        {
            if (id == null || db.Learners == null)
            {
                return NotFound();
            }

            var learner = db.Learners.Find(id);
            if (learner == null)
            {
                return NotFound();
            }
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName", learner.MajorID);
            return View(learner);
        }
        public IActionResult Delete(int id)
        {
            if (id == null || db.Learners == null)
            {
                return NotFound();
            }

            var learner = db.Learners.Include(l => l.Major)
                                     .Include(l => l.Enrollments)
                                     .FirstOrDefault(m => m.LearnerID == id);
            if (learner == null)
            {
                return NotFound();
            }

            // Ràng buộc: nếu người học đã có đăng ký học phần thì không cho xóa
            if (learner.Enrollments.Count() > 0)
            {
                return Content("This learner has some enrollments, can't delete!");
            }

            return View(learner);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("FirstMidName,LastName,MajorID,EnrollmentDate")] Learner learner)
        {
            if (ModelState.IsValid)
            {
                db.Learners.Add(learner);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName", learner.MajorID);
            return View(learner);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("LearnerID,FirstMidName,LastName,MajorID,EnrollmentDate")] Learner learner)
        {
            if (id != learner.LearnerID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(learner);
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearnerExists(learner.LearnerID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MajorID = new SelectList(db.Majors, "MajorID", "MajorName", learner.MajorID);
            return View(learner);
        }

        private bool LearnerExists(int id)
        {
            return (db.Learners?.Any(e => e.LearnerID == id)).GetValueOrDefault();
        }
    [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (db.Learners == null)
            {
                return Problem("Entity set 'Learners' is null.");
            }
            var learner = db.Learners.Find(id);
            if (learner != null)
            {
                db.Learners.Remove(learner);
            }

            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}