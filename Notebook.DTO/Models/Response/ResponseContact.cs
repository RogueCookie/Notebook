namespace Notebook.DTO.Models.Response
{
    /// <summary>
    /// Contact model with Id
    /// </summary>
    public class ResponseContact : ContactModel
    {
        /// <summary>
        /// Id of contact
        /// </summary>
        public long Id { get; set; }
    }
}