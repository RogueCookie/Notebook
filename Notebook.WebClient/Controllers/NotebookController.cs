using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Notebook.WebClient.Services;
using System;
using System.Threading.Tasks;

namespace Notebook.WebClient.Controllers
{
    [Route("api/v1/[actions]")]
    [ApiController]
    public class NotebookController : Controller
    {
        private readonly NotebookService _notebookService;
        private readonly ILogger<NotebookService> _logger;

        public NotebookController(NotebookService notebookService, ILogger<NotebookService> logger)
        {
            _notebookService = notebookService;
            _logger = logger;
        }

        //[HttpGet]
        //public async Task<ActionResult<string>> GetNotes(DateTime? from, DateTime? to)
        //{
        //    _notebookService.GetAllNotDeletedRecordsAsync(from, to); 
        //    throw new NotImplementedException();
        //}
    }
}
