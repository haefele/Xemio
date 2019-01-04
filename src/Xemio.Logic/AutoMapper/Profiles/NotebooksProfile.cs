using AutoMapper;
using Xemio.Client.Data.Entities;
using Xemio.Logic.AutoMapper.ValueConverter;
using Xemio.Logic.Database.Entities;

namespace Xemio.Logic.AutoMapper.Profiles
{
    public class NotebooksProfile : Profile
    {
        public NotebooksProfile()
        {
            this.CreateMap<NotebookHierarchy, NotebookHierarchyDTO>()
                .ForMember(f => f.UserId, o => o.ConvertUsing<TrimCollectionNameFromIdConverter<User>, string>(f => f.UserId));

            this.CreateMap<NotebookHierarchyItem, NotebookHierarchyItemDTO>()
                .ForMember(f => f.Id, o => o.ConvertUsing<TrimCollectionNameFromIdConverter<Notebook>, string>(f => f.NotebookId))
                .ForMember(f => f.Name, o => o.MapFrom(f => f.NotebookName));
        }
    }
}