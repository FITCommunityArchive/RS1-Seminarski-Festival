﻿using Festival.Data.Models;
using Festival.Data.Repositories;
using Festival.Web.Helper;
using FestivalWebApplication.ViewModels.Performer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FestivalWebApplication.Controllers
{
    public class PerformerController : Controller
    {
        private readonly IPerformerRepository _repo;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PerformerController(IWebHostEnvironment environment, IPerformerRepository repo, IWebHostEnvironment webHostEnvironment)
        {
            _hostingEnvironment = environment;
            _repo = repo;
        }
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public IActionResult List()
        {
            //fetching all records from Performer
            var model = _repo.GetAll().Select(p => new PerformersListVM
            {
                PerformerID = p.ID,
                PerformerName = p.Name,
                ManagerName = p.Manager.Name,
                Fee = p.Fee,
                Contact = p.Manager.PhoneNumber
            }).ToList();

            return View(model);
        }

        public IActionResult New()
        {
            NewPerformerVM Model = new NewPerformerVM();
            return View(Model);
        }

        public IActionResult SaveNew(NewPerformerVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("New");
            }
            //creating new manager object
            var manager = new Manager
            {
                Name = model.ManagerName,
                PhoneNumber = model.ManagerPhoneNumber,
                Email = model.ManagerEmail
            };
            //adding new manager to db first, to be able to assign managerID to a performer
            _repo.AddManager(manager);

            var uniqueFileName = ImageUpload.UploadImage(model.Image, _hostingEnvironment, "performers");

            var performer = new Performer
            {
                Name = model.Name,
                Fee = model.Fee,
                PromoText = model.PromoText,
                ManagerID = manager.ID,
                Picture = uniqueFileName
            };

            _repo.Add(performer);

            return RedirectToAction("List");
        }

        public IActionResult Edit(int id)
        {
            var performer = _repo.GetByID(id);

            var model = new EditPerformerVM
            {
                Id = performer.ID,
                Name = performer.Name,
                Fee = performer.Fee,
                PromoText = performer.PromoText,
                ManagerId = performer.Manager.ID,
                ManagerName = performer.Manager.Name,
                ManagerPhoneNumber = performer.Manager.PhoneNumber,
                ManagerEmail = performer.Manager.Email
            };

            return View("Edit", model);

        }
        public IActionResult Save(EditPerformerVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit");
            }

            var manager = _repo.FindManagerById(model.ManagerId);

            manager.Name = model.ManagerName;
            manager.Email = model.ManagerEmail;
            manager.PhoneNumber = model.ManagerPhoneNumber;

            _repo.Save();

            var performer = _repo.GetByID(model.Id);
            performer.Name = model.Name;
            performer.Fee = model.Fee;
            performer.PromoText = model.PromoText;

            if (model.Image != null)
            {
                var uniqueFileName = ImageUpload.UploadImage(model.Image, _hostingEnvironment, "performers");
                performer.Picture = uniqueFileName;
            }

            _repo.Save();
            return RedirectToAction("List");

        }

        public IActionResult Detail(int id)
        {
            var performer = _repo.GetByID(id);
            var model = new DetailPerformerVM
            {
                Id = performer.ID,
                Fee = performer.Fee,
                Name = performer.Name,
                PromoText = performer.PromoText,
                Picture = performer.Picture,
                ManagerName = performer.Manager.Name,
                ManagerPhoneNumber = performer.Manager.PhoneNumber,
                ManagerEmail = performer.Manager.Email
            };

            return View("Detail", model);
        }
        public IActionResult Delete(int id)
        {
            _repo.Delete(id);

            return RedirectToAction("List");
        }
    }


}
