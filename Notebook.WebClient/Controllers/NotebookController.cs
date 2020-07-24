using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notebook.WebClient.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
    }
}
