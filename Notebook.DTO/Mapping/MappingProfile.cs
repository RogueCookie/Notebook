using AutoMapper;
using Notebook.Domain.Entity;
using Notebook.DTO.Models.Request;

namespace Notebook.DTO.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ContactCreateModel, Contact>();
            CreateMap<Contact, ContactCreateModel>();

            CreateMap<NoteModel, Record>();
            CreateMap<Record, NoteModel>();

            CreateMap<ContactInformationModel, ContactInformation>();
            CreateMap<ContactInformation, ContactInformationModel>();
        }
    }
}