using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunGroupMVCPractise.Data;
using RunGroupMVCPractise.Interfaces;
using RunGroupMVCPractise.Models;
using RunGroupMVCPractise.ViewModels;

namespace RunGroupMVCPractise.Controllers
{
    public class ClubController : Controller
    {
        private readonly IClubRepository _repository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClubController(IClubRepository repository, IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var clubs = await _repository.GetAll();
            return View(clubs);
        }

        public async Task<IActionResult> DetailAsync(int id)
        {
            var club = await _repository.GetByIdAsync(id);
            return View(club);
        }

        public IActionResult Create()
        {
            var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            var createClubVM = new CreateClubViewModel() { AppUserId = curUser };
            return View(createClubVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClubViewModel clubView)
        {
            if(!ModelState.IsValid)
            {
                return View(clubView);
            }
            var result = await _photoService.AddPhotoAsync(clubView.Image);
            var club = new Club
            {
                Title = clubView.Title,
                Description = clubView.Description,
                Image = result.Url.ToString(),
                Address = new Address
                {
                   City=clubView.Address.City,
                   State=clubView.Address.State,
                   Street=clubView.Address.Street,
                },
                ClubCategory=clubView.ClubCategory,
                AppUserId=clubView.AppUserId
            };

            _repository.Add(club);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var club = await _repository.GetByIdAsync(id);
            if (club == null) return View("Error");


            var curUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            var clubVM = new EditClubViewModel
            {
                Title=club.Title,
                Description=club.Description,
                Address=club.Address,
                AddressId=club.AddressId,
                ClubCategory=club.ClubCategory,
                URL=club.Image,
                AppUserId= curUser

            };

            return View(clubVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit club.");
                return View("Edit", clubVM);
            }

            var userClub = await _repository.GetByIdAsyncNoTracking(id);

            if(userClub is not null)
                try
                {
                    await _photoService.DeletePhotoAsync(userClub.Image);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Could not delete photo");
                    return View(clubVM);
                }

            var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);

            var club = new Club
            {
                Id=id,
                Title = clubVM.Title,
                Description = clubVM.Description,
                Image = photoResult.Url.ToString(),
                AddressId=clubVM.AddressId,
                Address = clubVM.Address,
                ClubCategory = clubVM.ClubCategory,
                AppUserId = clubVM.AppUserId
            };
            _repository.Update(club);
            return RedirectToAction("Index");
        }

    }
}
