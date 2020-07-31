using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notebook.Domain.Entity;
using Notebook.DTO.Models.Request;
using Notebook.WebClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Notebook.DTO.Models;

namespace Notebook.WebClient.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [Produces("application/json", "application/xml")]
   // [Route("api/v1/notebook/contact")]
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
        [ProducesResponseType(typeof(List<ContactCreateModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<ContactCreateModel>>> GetAllContacts()  // TODO or better to do List<ContactModel>
        {
            var contacts = await _contactService.GetAllContactsAsync();
            if (contacts == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<List<ContactCreateModel>>(contacts));
            //return View(model);
        }

        /// <summary>
        /// Get particular contact
        /// </summary>
        /// <param name="contactId">Id of the contact</param>
        /// <returns>An ActionResult of type ContactCreateModel</returns>
        /// <response code="200">Returns the requested contact</response>
        [HttpGet]
        [ProducesResponseType(typeof(ContactCreateModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ContactCreateModel>> GetContact([FromQuery]long contactId)
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
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContactInformationModel>), (int)HttpStatusCode.OK)]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<IEnumerable<ContactInformationModel>>> GetDetails([FromQuery] long contactId)
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
            return Ok(_mapper.Map<IEnumerable<ContactInformationModel>> (contactInfo));

            //return View(model);
        }

        ///// <summary>
        ///// Create new contact
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public async Task<ActionResult<ContactCreateModel>> CreateContact()
        //{
        //    var model = new ContactCreateModel ();
        //    return Ok(model);
        //    //return View(model);
        //}

        /// <summary>
        /// Add new contact
        /// </summary>
        /// <param name="model">New contact entity</param>
        /// <returns>An ActionResult of type ContactCreateModel</returns>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ContactCreateModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ContactCreateModel>> CreateContact([FromBody]ContactCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var contactToAdd = _mapper.Map<Contact>(model);
            var addedContact = await _contactService.AddContactAsync(contactToAdd);
            var result = CreatedAtRoute(
                "GetContact",
                new { model.Id },
                _mapper.Map<ContactCreateModel>(contactToAdd));
            return result;
            //return View(model);
        }

        ///// <summary>
        ///// Add contact information for particular contact
        ///// </summary>
        ///// <returns>An ActionResult of type NoteModel</returns>
        //[HttpGet]
        //[ProducesResponseType(typeof(ContactCreateModel), (int)HttpStatusCode.OK)]
        //public ActionResult CreateContactInformation([FromQuery]long contactId)     // TODO with view i did it without id...how? should be opened with id of contact
        //{
        //    var model = new ContactInformationModel {ContactId = contactId}; // TODO need to check

        //    return Ok(model);
        //}

        /// <summary>
        /// Add a new contact information
        /// </summary>
        /// <param name="model">New contact information entity</param>
        /// <returns>An ActionResult of type ContactInformationModel</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ContactInformationModel), (int)HttpStatusCode.OK)]
        [Consumes("application/json")]
        //[ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ActionResult<ContactInformationModel>> CreateContactInformation([FromBody] ContactInformationModel model)
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
            return BadRequest();
            //return View(model);
        }

        /// <summary>
        /// Get information for updates for particular contact
        /// </summary>
        /// <param name="contactInfoId">Id of information</param>
        /// <returns>An ActionResult of type ContactInformationModel</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ContactInformationModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ContactInformationModel>> EditContactInfo([FromQuery]long contactInfoId)
        {
            var contactInfo = await _contactService.GetCurrentContactInformationAsync(contactInfoId);
            /*var model = new ContactInformationModel
            {
                ContactId = contactInfo.ContactId,
                PhoneNumber = contactInfo.PhoneNumber,
                Email = contactInfo.Email,
                Skype = contactInfo.Skype,
                Other = contactInfo.Other
            };
            //return View(model);*/
            if (contactInfo == null)
            {
                _logger.LogError($"Information id {contactInfoId} cannot be found");
                return NotFound();
            }

            return Ok(_mapper.Map<ContactInformationModel>(contactInfo));
        }

        /// <summary>
        /// Edit contact information
        /// </summary>
        /// <param name="contactInfoId">Id of information</param>
        /// <param name="model">Entity which need to update</param>
        /// <returns>An ActionResult of type ContactInformationModel</returns>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ContactInformationModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ContactInformationModel>> EditContactInfo([FromBody]ContactInformationModel model)
        {
            var infoFromService = await _contactService.GetCurrentContactInformationAsync(model.ContactId);
            return Ok(_mapper.Map<ContactInformationModel>(infoFromService));
        }

        /// <summary>
        /// Delete contact with all related information
        /// </summary>
        /// <param name="contactId">Id of contact</param>
        /// <returns>Whether contact was successfully removed or not</returns>
        [HttpDelete]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> Remove([FromQuery]long contactId)
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

            //return RedirectToAction(nameof(Index));
            return Ok(isRemoved);
        }


        /// <summary>
        /// Delete particular information about contact
        /// </summary>
        /// <param name="contactInfoId">Id of contact information</param>
        /// <returns>Whether information was successfully removed or not</returns>
        [HttpDelete]
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        //[Consumes("application/json")]
        public async Task<ActionResult<bool>> RemoveContactInformation([FromQuery]long contactInfoId)
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

            //return RedirectToAction(nameof(Detail));
            return Ok(isRemovedInfo);
        }
    }
}
