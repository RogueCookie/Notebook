using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notebook.DTO.Models;
using Notebook.DTO.Models.Request;
using Notebook.DTO.Models.Response;
using Notebook.WebClient.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Notebook.WebClient.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [Produces("application/json", "application/xml")]
    [ApiController]
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private readonly ContactService _contactService;
        private readonly ContactInformationService _informationService;

        public ContactController(ILogger<ContactController> logger, ContactService contactService, ContactInformationService  informationService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
            _informationService = informationService ?? throw new ArgumentNullException(nameof(informationService));
        }

        /// <summary>
        /// Get all contacts
        /// </summary>
        /// <returns>All not deleted contacts</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseContact>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<ResponseContact>>> GetAllContacts() 
        {
            var contacts = await _contactService.GetAllContactsAsync();
            if (contacts == null)
            {
                return NotFound();
            }

            return Ok(contacts);
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
            var contactFromService = await _contactService.GetContactByIdAsync(contactId);
            if (contactFromService == null)
            {
                return NotFound();
            }

            return Ok(contactFromService);

        }

        /// <summary>
        /// Get details for particular contact
        /// </summary>
        /// <param name="contactId">Id of contact</param>
        /// <returns>All contact information</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ContactInformationRequestModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ContactInformationRequestModel>>> GetDetails([FromQuery] long contactId)
        {
            var contactInfo = await  _informationService.GetAllInfoForContactAsync(contactId);

            return Ok(contactInfo);
        }

        /// <summary>
        /// Add new contact
        /// </summary>
        /// <param name="model">New contact entity</param>
        /// <returns>An ActionResult of type ContactCreateModel</returns>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)] 
        public async Task<ActionResult<long>> CreateContact([FromBody]ContactCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var addedContact = await _contactService.AddContactAsync(model);
            return Ok($"New contact was added with id {addedContact}");
        }

        /// <summary>
        /// Add a new contact information
        /// </summary>
        /// <param name="model">New contact information entity</param>
        /// <returns>An ActionResult of type ContactInformationModel</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ContactInformationModel), (int)HttpStatusCode.OK)]
        [Consumes("application/json")]
        public async Task<ActionResult<ContactInformationModel>> CreateContactInformation([FromBody] ContactInformationRequestModel model)
        {
            if (ModelState.IsValid) 
            {
                var contactId = await _informationService.AddContactInformationAsync(model);

                return Ok($"New information was added with id {contactId}"); 
            }
            return BadRequest();
        }

        /// <summary>
        /// Update particular contact
        /// </summary>
        /// <param name="model">Entity for update</param>
        /// <returns>An ActionResult of type ContactCreateModel</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ContactCreateModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ContactCreateModel>> EditContact([FromQuery] ResponseContact model)
        {
            var contact = await _contactService.UpdateContact(model);
            return Ok(contact);
        }

        /// <summary>
        /// Edit contact information
        /// </summary>
        /// <param name="model">Entity which need to update</param>
        /// <returns>An ActionResult of type ContactInformationModel</returns>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(ContactInformationResponseModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ContactInformationResponseModel>> EditContactInfo([FromBody]ContactInformationResponseModel model)
        {
            var infoFromService = await _informationService.UpdateContactInformation(model);
            return Ok(infoFromService);
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

            return Ok($"Whether contact with id {contactId} is removed {isRemoved}");
        }

        /// <summary>
        /// Delete particular information about contact
        /// </summary>
        /// <param name="contactInfoId">Id of contact information</param>
        /// <returns>Whether information was successfully removed or not</returns>
        [HttpDelete]
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> RemoveContactInformation([FromQuery]long contactInfoId)
        {
            var isRemovedInfo = await _informationService.RemoveCurrentContactInformationAsync(contactInfoId);
            if (!isRemovedInfo)
            {
                _logger.LogError($"Contact information with id {contactInfoId} can't be removed");
            }
            else
            {
                _logger.LogInformation($"Contact information with id {contactInfoId} was successfully removed");
            }

            return Ok($"Whether contact information with id {contactInfoId} is removed {isRemovedInfo}");
        }
    }
}
