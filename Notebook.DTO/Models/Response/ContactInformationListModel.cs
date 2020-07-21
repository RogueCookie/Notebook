using Notebook.DTO.Models.Request;
using System.Collections.Generic;

namespace Notebook.DTO.Models.Response
{
    /// <summary>
    /// Represent a collection of contact information
    /// </summary>
    public class ContactInformationListModel
    {
        /// <summary>
        /// Collection of contact information
        /// </summary>
        public List<ContactInformationModel> ContactInformationList;
    }
}