using System;
using System.Collections.Generic;
using DatingApp.Api.Models;
using DatingApp.API.Models;

namespace DatingApp.Api.Dtos
{
    public class UserForDetailDto
    {
        public UserForDetailDto()
        {
            
        }
        // public UserForDetailDto(int id, string userName, string gender, DateTime dateOfBirth, DateTime created, DateTime lastActive, string introduction, string interests, string city, string country, string photoUrl)
        // {
        //     this.Id = id;
        //     this.UserName = userName;
        //     this.Gender = gender;
        //     this.DateOfBirth = dateOfBirth;
        //     this.Created = created;
        //     this.LastActive = lastActive;
        //     this.Introduction = introduction;
        //     this.Interests = interests;
        //     this.City = city;
        //     this.Country = country;
        //     this.PhotoUrl = photoUrl;

        // }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string LookinFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<PhotoForDetailDto> Photos { get; set; }
    }
}