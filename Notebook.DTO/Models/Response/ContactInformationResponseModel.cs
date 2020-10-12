namespace Notebook.DTO.Models.Response
{
    /// <summary>
    /// Information with id of entity
    /// </summary>
    public class ContactInformationResponseModel : ContactInformationModel
    {
        /// <summary>
        /// Id of info record
        /// </summary>
        public long Id { get; set; }
    }
}