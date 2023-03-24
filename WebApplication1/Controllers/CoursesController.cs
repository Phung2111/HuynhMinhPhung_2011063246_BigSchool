using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        // GET: Courses
        public CoursesController()
        {
            _dbContext = new ApplicationDbContext();
        }
        // GET: Courses
        [Authorize] //bat buoc tao tai khoan
        public ActionResult Create()
        {
            var viewModel = new CourseViewModel
            {
                Categories = _dbContext.Categories.ToList(),
                Heading = "Add Course"

            };
            return View(viewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                viewModel.Categories = _dbContext.Categories.ToList();
                return View("Create",viewModel); 
            }
            var course = new Course
            {
                LecturerID = User.Identity.GetUserId(),
                Datetime = viewModel.GetDateTime(),
                CategoryId = viewModel.Category,
                Place = viewModel.Place
            };
        _dbContext.Courses.Add(course);
        _dbContext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public ActionResult Mine()
        {
            var userId =User.Identity.GetUserId();
            var courses =_dbContext.Courses
                .Where(c => c.LecturerID == userId  && c.Datetime >DateTime.Now)
                .Include(l =>l.Lecturer)
                .Include(c =>c.Category)
                .ToList();
            return View(courses);
        }
        [Authorize]
        public ActionResult Edit(int id)
        {
            var userId=User.Identity.GetUserId();
            var course = _dbContext.Courses.Single(c =>c.Id == id && c.LecturerID == userId);

            var viewModel = new CourseViewModel
            {
                Categories = _dbContext.Categories.ToList(),
                Date = course.Datetime.ToString("dd/M/yyyy"),
                Time = course.Datetime.ToString("HH:mm"),
                Category = course.CategoryId,
                Place = course.Place,
                Heading="Edit Course",
                Id=course.Id
            };
            return View("Create",viewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(CourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = _dbContext.Categories.ToList();
                return View("Create",viewModel );
            }
            var userId = User.Identity.GetUserId();
            var course = _dbContext.Courses.Single(c => c.Id == viewModel.Id && c.LecturerID == userId);

            course.Place = viewModel.Place;
            course.Datetime= viewModel.GetDateTime();
            course.CategoryId = viewModel.Category;
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}