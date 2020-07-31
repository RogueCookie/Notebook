using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Notebook.DTO.Models.Request
{
    /// <summary>
    /// Represents contact information list for call
    /// </summary>
    public class CreateContactInformationModel : ContactInformationModel
    {
        public IEnumerable<ContactInformationModel> ContactInformationModels { get; set; }
    }
}