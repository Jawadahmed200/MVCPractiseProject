﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroupMVCPractise.Interfaces;
using RunGroupMVCPractise.Services;
using RunGroupMVCPractise.ViewModels;

namespace RunGroupMVCPractise.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Index()
        {
            var users = await _userRepository.GetAllUsers();
            List<UserViewModel> result = new List<UserViewModel>();

            foreach (var item in users)
            {
                var userViewModel = new UserViewModel()
                {
                    UserName = item.UserName,
                    Id = item.Id,
                    Pace = item.Pace,
                    Mileage = item.Mileage,
                };
                result.Add(userViewModel);

            }

            return View(result);
        }
        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var userDetailViewModel = new UserDetailViewModel()
            {
                Id = user.Id,
                Pace = user.Pace,
                //City = user.City,
                //State = user.State,
                Mileage = user.Mileage,
                UserName = user.UserName,
                //ProfileImageUrl = user.ProfileImageUrl ?? "/img/avatar-male-4.jpg",
            };
            return View(userDetailViewModel);
        }

        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> EditProfile()
        //{
        //    var user = await _userManager.GetUserAsync(User);

        //    if (user == null)
        //    {
        //        return View("Error");
        //    }

        //    var editMV = new EditProfileViewModel()
        //    {
        //        City = user.City,
        //        State = user.State,
        //        Pace = user.Pace,
        //        Mileage = user.Mileage,
        //        ProfileImageUrl = user.ProfileImageUrl,
        //    };
        //    return View(editMV);
        //}

        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> EditProfile(EditProfileViewModel editVM)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ModelState.AddModelError("", "Failed to edit profile");
        //        return View("EditProfile", editVM);
        //    }

        //    var user = await _userManager.GetUserAsync(User);

        //    if (user == null)
        //    {
        //        return View("Error");
        //    }

        //    if (editVM.Image != null) // only update profile image
        //    {
        //        var photoResult = await _photoService.AddPhotoAsync(editVM.Image);

        //        if (photoResult.Error != null)
        //        {
        //            ModelState.AddModelError("Image", "Failed to upload image");
        //            return View("EditProfile", editVM);
        //        }

        //        if (!string.IsNullOrEmpty(user.ProfileImageUrl))
        //        {
        //            _ = _photoService.DeletePhotoAsync(user.ProfileImageUrl);
        //        }

        //        user.ProfileImageUrl = photoResult.Url.ToString();
        //        editVM.ProfileImageUrl = user.ProfileImageUrl;

        //        await _userManager.UpdateAsync(user);

        //        return View(editVM);
        //    }

        //    user.City = editVM.City;
        //    user.State = editVM.State;
        //    user.Pace = editVM.Pace;
        //    user.Mileage = editVM.Mileage;

        //    await _userManager.UpdateAsync(user);

        //    return RedirectToAction("Detail", "User", new { user.Id });
        //}
    }
}