using Microsoft.AspNetCore.Mvc;
using Notebook.WebClient.Models;
using Notebook.WebClient.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Notebook.WebClient.Controllers
{
    public class ContactController : Controller
    {
        //private readonly ILogger<ContactController> _logger;
        private readonly ContactService _contactService;

        public ContactController(/*ILogger<ContactController> logger,*/ ContactService contactService)
        {
            //_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
        }

        public async Task<IActionResult> Index()
        {
            var contacts = await _contactService.GetAllContacts();
            var contactModel = contacts.Select(c => new ContactDetailModel
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName ?? " "
                
            }).ToList();

            var model = new ContactPortfolioListModel()
            {
                ContactPortfolioModels = contactModel
            };

            return View(model);
        }


        public async Task<IActionResult> Detail(long contactId)
        {
            var contactRecord = await _contactService.GetContactById(contactId);

            var model = new ContactDetailModel
            {
                FirstName = contactRecord.FirstName,
                LastName = contactRecord.LastName,
                Patronymic = contactRecord.Patronymic,
                BirthDate = contactRecord.BirthDate,
                OrganizationName = contactRecord.OrganizationName,
                Position = contactRecord.Position,
                ContactInformations = contactRecord.CollectionInformations.Where(x => x.ContactId == contactId).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new ContactCreateModel ();
            return View(model);
        }

     

    }
}
