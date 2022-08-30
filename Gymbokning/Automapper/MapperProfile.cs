using AutoMapper;
using Gymbokning.Models;
using Gymbokning.ViewModels;

namespace Gymbokning.Automapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<GymClass, GymClassIndexViewModel>();
            CreateMap<GymClass, GymClassDetailViewModel>()
                .ForMember(
                    dest => dest.GymClassMembers,
                    from => from.MapFrom(g => g.GymClassMembers.Select(m => m.ApplicationUser))); 
        }


    }
}
