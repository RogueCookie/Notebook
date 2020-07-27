using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notebook.Domain.Entity;
using Notebook.DTO.Models.Request;
using Notebook.WebClient.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Notebook.WebClient.Controllers
{
    //[Route("api/v1/[actions]")]
    [Route("api/v1/notebook/contact")]
    [ApiController]

    //[Produces("application/json", "application/xml")]
    //[Route("api/v{version:apiVersion}/books")]
    //[ApiController]
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private readonly ContactService _contactService;
        private readonly IMapper _mapper;

        public ContactController(ILogger<ContactController> logger, ContactService contactService, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var contacts = await _contactService.GetAllContactsAsync();
            var contactModel = contacts.Select(c => new ContactCreateModel
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName ?? " "
            }).ToList();

            var model = new ContactPortfolioListModel()
            {
                ContactPortfolioModels = contactModel
            };

            return Ok(model);
            //return View(model);
        }

        [HttpGet("{contactId}")]
        public async Task<IActionResult> Detail(long contactId)
        {
            var contactInfo = await  _contactService.GetAllInfoForContactAsync(contactId);
            var contactModel = contactInfo.Select(x => new ContactInformationModel
            {
                ContactId = x.ContactId,
                PhoneNumber = x.PhoneNumber,
                Skype = x.Skype,
                Email = x.Email,
                Other = x.Other
            }).ToList();

            var model = new ContactDetailList
            {
                ContactDetails = contactModel
            };

            return Ok(model);
            //return View(model);
        }

       /* [HttpGet]
        public IActionResult CreateContact()
        {
            var model = new ContactCreateModel ();
            return Ok(model);
            //return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact(ContactCreateModel model)
        {
            if (ModelState.IsValid)
            {
                //var contact = _mapper.Map<ContactCreateModel, Contact>(model);
                var newContact = new Contact
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Patronymic = model.Patronymic,
                    BirthDate = model.BirthDate,
                    OrganizationName = model.OrganizationName,
                    Position = model.Position
                };
                var result = await _contactService.AddContactAsync(newContact);
                _logger.LogInformation($"New contact was added with id {result}");
                return RedirectToAction("Index", "Contact");
            }

            return Ok(model);
            //return View(model);
        }

        [HttpGet]
        public IActionResult CreateContactInformation()
        {
            var model= new ContactInformationModel();
            return Ok(model);
            //return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContactInformation(ContactInformationModel model)
        {
            if (ModelState.IsValid)
            {
                
             
            }

            return Ok(model);
            //return View(model);
        }

        public async Task<IActionResult> Edit(long contactId)
        {
            var contact = await _contactService.GetContactByIdAsync(contactId);
            return Ok(contact);
            //return View(contact);
        }

        [HttpGet]
        public async Task<IActionResult> EditContactInfo(long contactInfoId)
        {
            var contactInfo = await _contactService.GetCurrentContactInformationAsync(contactInfoId);
            var model = new ContactInformationModel
            {
                ContactId = contactInfo.ContactId,
                PhoneNumber = contactInfo.PhoneNumber,
                Email = contactInfo.Email,
                Skype = contactInfo.Skype,
                Other = contactInfo.Other
            };
            return Ok(model);
            //return View(model);
        }


        public async Task<IActionResult> Remove(long contactId)
        {
            var isRemoved = await _contactService.RemoveContactAsync(contactId);
            if (!isRemoved)
            {
                _logger.LogError($"Contact with id {contactId} can't be removed");
            }
            else
            {
                _logger.LogInformation($"Contact with id {contactId} was successfully removed");
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> RemoveContactInformation(long contactInfoId)
        {
            var isRemovedInfo = await _contactService.RemoveCurrentContactInformation(contactInfoId);
            if (!isRemovedInfo)
            {
                _logger.LogError($"Contact information with id {contactInfoId} can't be removed");
            }
            else
            {
                _logger.LogInformation($"Contact information with id {contactInfoId} was successfully removed");
            }

            return RedirectToAction(nameof(Detail));
        }*/
    }
}
