using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Notebook.WebClient.Models
{
    public class ContactInformationModel
    {
        public long Id { get; set; }
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [Display(Name = "Email", Prompt = "example@gmail.com")]
        public string Email { get; set; }
        public string Skype { get; set; }
        public string Other { get; set; }

    }
}
