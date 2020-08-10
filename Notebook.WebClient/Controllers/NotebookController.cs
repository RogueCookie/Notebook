using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Notebook.DTO.Models.Request;
using Notebook.DTO.Models.Response;
using Notebook.WebClient.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Notebook.WebClient.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class NotebookController : Controller
    {
        private readonly NotebookService _notebookService;
       // private readonly ILogger<NotebookService> _logger;


        public NotebookController(NotebookService notebookService/*, ILogger<NotebookService> logger*/)
        {
            _notebookService = notebookService ?? throw new ArgumentNullException(nameof(notebookService));
            //_logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all notes
        /// </summary>
        /// <param name="from">From date</param>
        /// <param name="to">Till date</param>
        /// <returns>All not completed and not deleted notes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<NoteCreateResponseModel>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<NoteCreateResponseModel>>> GetAllNotes(DateTime? from, DateTime? to)
        {
            var allFromService = await _notebookService.GetAllNotDeletedRecordsAsync(from, to);
            if (allFromService == null)
            {
                return NotFound();
            }
            return Ok(allFromService);
        }

        /// <summary>
        /// Add a new note
        /// </summary>
        /// <param name="model">Entity which will be added</param>
        /// <returns>An ActionResult of type long</returns>
        [HttpPost]
        [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<long>> CreateNote([FromBody]NoteCreateModel model)
        {
            var addedNote = await _notebookService.AddRecordAsync(model);
           
            if (model.ContactIds != null)
            {
                await _notebookService.UpdateContactsForNoteAsync(addedNote, model.ContactIds);
            }

            return Ok($"New note was added with id {addedNote}");
        }

        /// <summary>
        /// Get particular note by Id
        /// </summary>
        /// <param name="noteId">The id of note you want to get</param>
        /// <returns>An ActionResult of type NoteModel</returns>
        /// <response code="200">Returns the requested note</response>
        [HttpGet]
        [ProducesResponseType(typeof(NoteCreateModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<NoteCreateModel>> GetNote([FromQuery]long noteId)     
        {
            var noteFromService = await _notebookService.GetRecordByIdAsync(noteId);
            if (noteFromService == null)
            {
                return NotFound();
            }

            return Ok(noteFromService);
        }

        /// <summary>
        /// Update particular note
        /// </summary>
        /// <param name="recordForUpdate">Entity which need to update</param>
        /// <returns>An ActionResult of type NoteModel</returns>
        [HttpPut]
        [ProducesResponseType(typeof(NoteCreateModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<NoteCreateModel>> EditNote([FromBody] NoteCreateResponseModel recordForUpdate)
        {
            var noteFromService = await _notebookService.UpdateRecordAsync(recordForUpdate);
            if (noteFromService == null)
            {
                return NotFound();
            }

            if (recordForUpdate.ContactIds.Any())
            {
                await _notebookService.UpdateContactsForNoteAsync(recordForUpdate.Id, recordForUpdate.ContactIds);
            }

            return Ok(noteFromService);
        }

        /// <summary>
        /// Modify completed status for record
        /// </summary>
        /// <param name="model">Entity for mark</param>
        /// <returns>Model with refresh status</returns>
        [HttpPut]
        [ProducesResponseType(typeof(NoteCreateModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<NoteCreateModel>> MarkRecordAsCompleted([FromBody] NoteCreateResponseModel model)
        {
            var noteFromService = await _notebookService.MarkRecordAsCompletedAsync(model.Id, model.IsComplete);
            return Ok(noteFromService);
        }
    }
}
