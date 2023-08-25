using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupMVCPractise.Data;
using RunGroupMVCPractise.Interfaces;
using RunGroupMVCPractise.Models;
using RunGroupMVCPractise.ViewModels;

namespace RunGroupMVCPractise.Controllers
{
    public class RaceController : Controller
    {
        private readonly IRaceRepository _repository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RaceController(IRaceRepository repository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index()
        {
            var races = await _repository.GetAll();
            return View(races);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var race = await _repository.GetByIdAsync(id);
            return View(race);
        }
        public IActionResult Create()
        {
            var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            var createRaceVM = new CreateRaceViewModel() { AppUserId = curUser };
            return View(createRaceVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel raceVM)
        {
            if (!ModelState.IsValid)
            {
                return View(raceVM);
            }
            var result = await _photoService.AddPhotoAsync(raceVM.Image);
            var race = new Race
            {
                Title = raceVM.Title,
                Description = raceVM.Description,
                Image = result.Url.ToString(),
                Address = new Address
                {
                    City = raceVM.Address.City,
                    State = raceVM.Address.State,
                    Street = raceVM.Address.Street,
                },
                RaceCategory = raceVM.RaceCategory,
                AppUserId=raceVM.AppUserId
            };
            _repository.Add(race);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var race = await _repository.GetByIdAsync(id);
            if (race == null) return View("Error");

            var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            var raceVM = new EditRaceViewModel
            {
                Title = race.Title,
                Description = race.Description,
                Address = race.Address,
                AddressId = race.AddressId,
                RaceCategory = race.RaceCategory,
                URL = race.Image,
                AppUserId= curUser
            };

            return View(raceVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club.");
                return View("Edit", raceVM);
            }

            var userRace = await _repository.GetByIdAsyncNoTracking(id);

            if (userRace is not null)
                try
                {
                    await _photoService.DeletePhotoAsync(userRace.Image);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(raceVM);
                }

            var photoResult = await _photoService.AddPhotoAsync(raceVM.Image);

            var race = new Race
            {
                Id = id,
                Title = raceVM.Title,
                Description = raceVM.Description,
                Image = photoResult.Url.ToString(),
                AddressId = raceVM.AddressId,
                Address = raceVM.Address,
                RaceCategory = raceVM.RaceCategory,
                AppUserId=raceVM.AppUserId
            };
            _repository.Update(race);
            return RedirectToAction("Index");
        }
    }
}
