using Notebook.DTO.Models.Responce;
using System.Collections.Generic;

namespace Notebook.DTO.Models.Request
{
    /// <summary>
    /// Get list of all additional contact information
    /// </summary>
    public class ContactPortfolioListModel : BaseModel
    {
        /// <summary>
        /// List of contact information
        /// </summary>
        public IEnumerable<AddNewContact> ContactPortfolioModels;
    }
}
