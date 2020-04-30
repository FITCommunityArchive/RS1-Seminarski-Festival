﻿using Festival.Data.Models;
using Festival.Data.Repositories;
using FestivalWebApplication.ViewModels.Accommodation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FestivalWebApplication.Controllers
{
    [Authorize]
    public class AccommodationsController : Controller
    {
        private readonly IAccommodationRepository _repo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccommodationsController(IAccommodationRepository repo, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _repo = repo;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public IActionResult List(string sortOrder)
        {


            List<AccommodationListVM> model = _repo.GetAll().Select(acc => new AccommodationListVM
            {
                ID = acc.ID,
                Description = acc.Description ?? "No Description",
                Distance = acc.Distance,
                Name = acc.Name,
                PhoneNumber = acc.PhoneNumber,
                Address = acc.Address
            }).ToList();

            return View("List", model);

        }

        public IActionResult New()
        {
            NewAccommodationVM model = new NewAccommodationVM();
            return View(model);
        }

        public IActionResult Detail(int ID)
        {
            Accommodation accomodation = _repo.GetByID(ID);
            DetailAccommodationVM model = new DetailAccommodationVM()
            {
                ID = accomodation.ID,
                Name = accomodation.Name,
                PhoneNumber = accomodation.PhoneNumber,
                Distance = accomodation.Distance,
                Description = accomodation.Description,
                Address = accomodation.Address,
                Picture = accomodation.Picture
            };
            return View(model);
        }

        public IActionResult Delete(int ID)
        {
            _repo.Delete(ID);
            return Redirect("/Accommodations/List");
        }

        public IActionResult Edit(int Id)
        {
            Accommodation accommodation = _repo.GetByID(Id);
            EditAccommodationVM model = new EditAccommodationVM
            {
                ID = Id,
                Name = accommodation.Name,
                PhoneNumber = accommodation.PhoneNumber,
                Distance = accommodation.Distance,
                Description = accommodation.Description,
                Address = accommodation.Address
            };
            return View("Edit", model);
        }


        public IActionResult SaveNew(NewAccommodationVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("New");
            }

            string uniqueFileName = UploadedFile(model);
            Accommodation accommodation = new Accommodation()
            {
                Name = model.Name,
                Distance = model.Distance,
                PhoneNumber = model.PhoneNumber,
                Description = model.Description,
                Address = model.Address,
                Picture = uniqueFileName
            };

            _repo.Add(accommodation);
            return RedirectToAction("List");
        }


        public IActionResult Save(EditAccommodationVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit");
            }

            string uniqueFileName = UploadedFile(model);
            Accommodation acc = _repo.GetByID(model.ID);
            acc.Name = model.Name;
            acc.Description = model.Description;
            acc.Distance = model.Distance;
            acc.PhoneNumber = model.PhoneNumber;
            acc.Address = model.Address;
            if (model.ProfileImage != null)
            {
                acc.Picture = uniqueFileName;
            }
            _repo.Save();
            return RedirectToAction("List");
        }

        private string UploadedFile(NewAccommodationVM model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "accommodations");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private string UploadedFile(EditAccommodationVM model)
        {
            string uniqueFileName = null;

            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "accommodations");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
