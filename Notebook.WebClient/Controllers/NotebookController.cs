﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notebook.Domain.Entity;
using Notebook.DTO.Models.Request;
using Notebook.WebClient.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Notebook.WebClient.Controllers
{
    [Route("api/v1/notebook")]
    //[Produces("application/json", "application/xml")]
    //[Route("api/v{version:apiVersion}/notes")]
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

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

            // TODO check result for null

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

        [HttpPatch()]
        //[HttpPost()]
        public async Task<ActionResult<NoteModel>> UpdateNote(NoteModel recordForUpdate)
        {
            var noteFromService = await _notebookService.GetRecordByIdAsync(recordForUpdate.Id);
            if (noteFromService == null)
            {
                return NotFound();
            }

            var adaptModel = _mapper.Map<Record>(recordForUpdate);
            //_mapper.Map(noteFromService, recordForUpdate);
            await _notebookService.UpdateRecordAsync(adaptModel);
            return Ok(_mapper.Map<NoteModel>(noteFromService));
        }


        
    }
}
