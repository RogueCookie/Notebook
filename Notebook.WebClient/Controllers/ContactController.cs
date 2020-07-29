using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notebook.Domain.Entity;
using Notebook.DTO.Models.Request;
using Notebook.WebClient.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Notebook.WebClient.Controllers
{
    //[Route("api/v1/[actions]")]
    [Produces("application/json", "application/xml")]
    [Route("api/v1/notebook/contact")]
    [ApiController]
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

        /// <summary>
        /// Get all contacts
        /// </summary>
        /// <returns>All not deleted contacts</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ContactPortfolioListModel>> GetAllContacts()  // TODO or better to do List<ContactModel>
        {
            var contacts = await _contactService.GetAllContactsAsync();
            /*var contactModel = contacts.Select(c => new ContactCreateModel
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName ?? " "
            }).ToList();

            var model = new ContactPortfolioListModel()
            {
                ContactPortfolioModels = contactModel
            };
            return Ok(model);*/
            if (contacts == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ContactPortfolioListModel>(contacts));
            //return View(model);
        }

        /// <summary>
        /// Get particular contact
        /// </summary>
        /// <param name="contactId">Id of the contact</param>
        /// <returns>An ActionResult of type ContactCreateModel</returns>
        /// <response code="200">Returns the requested contact</response>
        [HttpGet("{contactId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ContactCreateModel>> GetContact(long contactId)
        {
            var allContactFromService = await _contactService.GetAllContactsAsync();
            var contactFromService = allContactFromService.FirstOrDefault(x => x.Id == contactId);
            if (contactFromService == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ContactCreateModel>(contactFromService));

        }

        /// <summary>
        /// Get details for particular contact
        /// </summary>
        /// <param name="contactId">Id of contact</param>
        /// <returns>All contact information</returns>
        [HttpGet("{contactId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ContactDetailList>> GetDetails(long contactId)
        {
            var contactInfo = await  _contactService.GetAllInfoForContactAsync(contactId);
            /*var contactModel = contactInfo.Select(x => new ContactInformationModel
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
            return Ok(model);*/
            return Ok(_mapper.Map<ContactDetailList>(contactInfo));

            //return View(model);
        }

        /// <summary>
        /// Create new contact
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreateContact()
        {
            var model = new ContactCreateModel ();
            return Ok(model);
            //return View(model);
        }

        /// <summary>
        /// Add new contact
        /// </summary>
        /// <param name="model">New contact entity</param>
        /// <returns>An ActionResult of type ContactCreateModel</returns>
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<ContactCreateModel>> CreateContact(ContactCreateModel model)
        {
            /*if (ModelState.IsValid)
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
                return RedirectToAction("GetAllContacts", "Contact");
            }

            return Ok(model);*/
            var contactToAdd = _mapper.Map<Contact>(model);
            var addedContact = await _contactService.AddContactAsync(contactToAdd);

            return CreatedAtRoute(
                "GetContact",
                new { model.Id },
                _mapper.Map<ContactCreateModel>(contactToAdd));
            //return View(model);
        }

        /// <summary>
        /// Add contact information for particular contact
        /// </summary>
        /// <returns>An ActionResult of type NoteModel</returns>
        [HttpGet]

        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult CreateContactInformation()     // TODO with view i did it without id...how?
        {
            var model= new ContactInformationModel();
            return Ok(model);
            //return View(model);
        }

        /// <summary>
        /// Add a new contact information
        /// </summary>
        /// <param name="model">New contact information entity</param>
        /// <returns>An ActionResult of type ContactInformationModel</returns>
        [HttpPost]
        [Consumes("application/json")]

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ContactInformationModel>> CreateContactInformation(ContactInformationModel model)
        {
            if (ModelState.IsValid) //TODO do we need it here? swagger do not do it for us?
            {
                var infoToAdd = _mapper.Map<ContactInformation>(model);
                var addedInfo = await _contactService.AddContactInformationAsync(infoToAdd);

                return CreatedAtRoute(
                    "GetDetails",
                    new {model.ContactId},
                    _mapper.Map<ContactDetailList>(infoToAdd) //TODO check how it works because there's list
                    );
            }

            return Ok(model);
            //return View(model);
        }

        /*
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
