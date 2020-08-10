using System.Collections.Generic;

namespace Notebook.DTO.Models.Response
{
    public class UpdateContactInNotes : NoteModel
    {
        /// <summary>
        /// Id or record
        /// </summary>
        public long Id { get; set; }

        public List<ResponseContact> Contacts { get; set; }
    }
}