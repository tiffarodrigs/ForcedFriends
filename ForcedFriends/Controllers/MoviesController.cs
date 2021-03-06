using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ForcedFriends.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;



namespace ForcedFriends.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ForcedFriendsContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public MoviesController(UserManager<ApplicationUser> userManager,ForcedFriendsContext db)
        {
          _userManager = userManager;
          _db = db;
        } 

        public IActionResult Index()
        {
          var allMovies = Movie.GetMovies(EnvironmentVariables.apiKey, 1);
          if (!_db.Movies.Any())
          {
            foreach(var movie in allMovies )
            {
              _db.Movies.Add(movie);
              _db.SaveChanges();
            }
            List<Movie> movieModel=_db.Movies.ToList();
              return View(movieModel);
          }
          else{
            return View(allMovies);
          }
        }
        public IActionResult Index2()
        {
          var allMovies = Movie.GetMovies(EnvironmentVariables.apiKey, 2);
         
            foreach(var movie in allMovies )
            {
              //if(!((_db.Movies.ToList()).Contains(movie)))
             if(!(_db.Movies.Any(u => u.Id == movie.Id)))
              {
              _db.Movies.Add(movie);
              _db.SaveChanges();
              }
            }
            List<Movie> movieModel=_db.Movies.ToList();
            return View(allMovies);

        }
        public IActionResult Index3()
        {
          var allMovies = Movie.GetMovies(EnvironmentVariables.apiKey, 3);
            foreach(var movie in allMovies )
            {
              //if(!((_db.Movies.ToList()).Contains(movie)))
             if(!(_db.Movies.Any(u => u.Id == movie.Id)))
              {
              _db.Movies.Add(movie);
              _db.SaveChanges();
              }
            }
            List<Movie> movieModel=_db.Movies.ToList();
            return View(allMovies);
        }

        public async Task <ActionResult> Details(int id)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var currentUser = await _userManager.FindByIdAsync(userId);
            ViewBag.ApplicationUser = currentUser;
            var thisMovie = Movie.GetMovie(EnvironmentVariables.apiKey, id);
            return View(thisMovie);
        }
        [Authorize]
        [HttpPost]


        public ActionResult AddMovie(int Id)

        {
          var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
          _db.ApplicationUserMovies.Add(new ApplicationUserMovie() { MovieId = Id, ApplicationUserId = userId });
          _db.SaveChanges();
          return RedirectToAction("GetWatchList");
        }
        [Authorize]
        [HttpGet]
        public ActionResult GetWatchList()
        {
          var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
          var watchList = _db.ApplicationUserMovies.Where(m => m.ApplicationUserId == userId).ToList();
          return View(watchList);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteMovie(int joinId)
        {
          var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
          var currentUser = await _userManager.FindByIdAsync(userId);
          if(userId == currentUser.Id){
            var joinEntry = _db.ApplicationUserMovies.FirstOrDefault(entry => entry.ApplicationUserMovieId == joinId);
            _db.ApplicationUserMovies.Remove(joinEntry);
            _db.SaveChanges();
          }
          return RedirectToAction("GetWatchList");
        }
    }
}
