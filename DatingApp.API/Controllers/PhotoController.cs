using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using datingapp.api.Dtos;
using datingapp.api.Helpers;
using DatingApp.Api.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace datingapp.api.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;


        public IDatingRepository _repo ;
        private Cloudinary _cloudinary;

        public PhotoController(IDatingRepository repo, IMapper mapper,
            IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _repo = repo;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                
                _cloudinaryConfig.Value.ApiSecret
                
            );
            _cloudinary = new Cloudinary(acc);
        }
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId,
            [FromForm]PhotoForCreationDto photoForCreationDto)
            {
                if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();  

                var userFromRepo = await _repo.GetUser(userId);

                var file = photoForCreationDto.File;
                var uploadResult = new ImageUploadResult();
                
                if(file.Length > 0 )
                {
                    using(var stream = file.OpenReadStream()){
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.Name, stream),
                            Transformation = new Transformation()
                            .Width(500).Height(500).Crop("fill").Gravity("face")
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);

                    }
                }
                photoForCreationDto.Url = uploadResult.Uri.ToString();
                photoForCreationDto.PublicId = uploadResult.PublicId;
                var _photo = _mapper.Map<Photo>(photoForCreationDto);

                if(userFromRepo.Photos.Count == 0 || userFromRepo.Photos.Any(u => u.IsMain))
                {
                    _photo.IsMain = true;
                }

                userFromRepo.Photos.Add(_photo);

                if(await _repo.SaveAll())
                {
                    var photoForReturn = _mapper.Map<PhotoForReturnDto>(_photo);
                    return CreatedAtRoute("GetPhoto", new {id = _photo.Id}, photoForReturn);
                }

                return BadRequest("Cound not add photo");
            }
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();  
            var user = await _repo.GetUser(userId);
            if(!user.Photos.Any(p => p.Id == id))
                {
                    return Unauthorized();  
                }
            var photoFromRepo = await _repo.GetPhoto(id);
            if(photoFromRepo.IsMain)
                return BadRequest("This photo is already main");

            var currentMainPhoto = await  _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if(await _repo.SaveAll())
             return NoContent();

            return BadRequest("Could not set main photo"); 

        }   
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();  
            var user = await _repo.GetUser(userId);
            if(!user.Photos.Any(p => p.Id == id))
                {
                    return Unauthorized();  
                }
            var photoFromRepo = await _repo.GetPhoto(id);
            if(photoFromRepo.IsMain)
                return BadRequest("You cannot delete the main photo");

           if(photoFromRepo.PublicId != null)
           { 
               var deleteParam = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParam);

                if(result.Result == "ok"){
                 _repo.Delete(photoFromRepo);
              
                }
           } 
           if(photoFromRepo.PublicId != null)
           { 
               _repo.Delete(photoFromRepo);
           }   
          

            if(await _repo.SaveAll()){
                   return Ok();
               }
               
            return BadRequest("Failed to delete photo");
        } 
    }
}