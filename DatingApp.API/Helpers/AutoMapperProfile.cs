using System.Linq;
using AutoMapper;
using DatingApp.Api.Dtos;
using DatingApp.Api.Models;
using DatingApp.API.Models;

namespace DatingApp.Api.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            
                  
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl,
                     opt => opt.MapFrom(src => src.Photos.FirstOrDefault( p => p.IsMain).Url))
                .ForMember(dest => dest.Age,
                     opt => opt.ResolveUsing(d => d.DateOfBirth.CalculateAge()));
;

            CreateMap<User, UserForDetailDto>()
                .ForMember(dest => dest.PhotoUrl,
                     opt => opt.MapFrom(src => src.Photos.FirstOrDefault( p => p.IsMain).Url))
                .ForMember(dest => dest.Age,
                     opt => opt.ResolveUsing(d => d.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoForDetailDto>();
        }
    }
}