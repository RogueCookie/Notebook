using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notebook.WebClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Notebook.Domain.Entity;
using Notebook.DTO.Models.Request;

namespace Notebook.WebClient.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NotebookController : Controller
    {
        private readonly NotebookService _notebookService;
        private readonly ILogger<NotebookService> _logger;
        private readonly IMapper _mapper;


        public NotebookController(NotebookService notebookService, ILogger<NotebookService> logger, IMapper mapper)
        {
            _notebookService = notebookService ?? throw new ArgumentNullException(nameof(notebookService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<List<NoteModel>>> GetNotes(DateTime? from, DateTime? to)
        {
            var allFromService = await _notebookService.GetAllNotDeletedRecordsAsync(from, to);
            return Ok(_mapper.Map<List<NoteModel>>(allFromService));
        }

        [HttpPost()]
        public async Task<ActionResult<NoteModel>> CreateNote(NoteModel model)
        {
            var noteToAdd = _mapper.Map<Record>(model);
            var addedNote = await _notebookService.AddRecordAsync(noteToAdd);
            var newNote = await _notebookService.GetRecordByIdAsync(addedNote);

            return CreatedAtRoute(
                "GetNote",
                new { model.Id },
                _mapper.Map<NoteModel>(noteToAdd));
        }

        [HttpGet("{noteId}")]
        public async Task<ActionResult<NoteModel>> GetNote(long noteId)
        {
            var noteFromService = await _notebookService.GetRecordByIdAsync(noteId);
            if (noteFromService == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<NoteModel>(noteFromService));
        }
    }
}
