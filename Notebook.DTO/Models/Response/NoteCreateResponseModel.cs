namespace Notebook.DTO.Models.Response
{
    /// <summary>
    /// Represent Note model
    /// </summary>
    public class NoteCreateResponseModel : NoteModel
    {
        /// <summary>
        /// Id of record
        /// </summary>
        public long Id { get; set; }
    }
}