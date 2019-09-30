using System.Linq;
using AutoMapper;
using datingapp.api.Dtos;
using datingapp.api.Models;
using datingApp.api.Dtos;
using DatingApp.Api.Dtos;
using DatingApp.Api.Models;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.Api.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            
            AutoMapper.Mapper.Reset();      
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
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(m => m.SenderPhotoUrl,
                opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p=> p.IsMain).Url))
                .ForMember(m => m.RecipientPhotoUrl,
                opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p=> p.IsMain).Url));
            
        }
    }
}