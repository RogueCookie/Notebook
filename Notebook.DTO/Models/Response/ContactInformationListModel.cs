using System.Collections.Generic;

namespace Notebook.DTO.Models.Response
{
    /// <summary>
    /// Represent a collection of contact information
    /// </summary>
    public class ContactInformationListModel : BaseModel
    {
        /// <summary>
        /// Collection of contact information
        /// </summary>
        public List<ContactInformationModel> ContactInformationList;
    }
}