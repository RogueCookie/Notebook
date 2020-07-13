namespace Notebook.Domain.Entity
{
    public class ContactInformation
    {
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Skype { get; set; }
        public string Other { get; set; }
       
        //navigation prop

        public long ContactId { get; set; }
        public Contact Contact { get; set; }
    }
}
