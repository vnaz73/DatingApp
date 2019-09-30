using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using datingapp.api.Helpers;
using datingapp.api.Models;
using datingApp.api.Dtos;
using datingApp.api.Helpers;
using DatingApp.Api.Data;
using DatingApp.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace datingApp.api.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMapper _mapper;

        private readonly IDatingRepository _repo;

        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
           
        }
        [HttpGet("{id}", Name= "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))           
                return Unauthorized();
            var messageFromRepo = await _repo.GetMessage(id);

            if(messageFromRepo == null)
                return NotFound();
            return Ok(messageFromRepo);    
        }
        [HttpGet]
        public async Task<IActionResult> GetMessages(int userId,
         [FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))           
                return Unauthorized();

            messageParams.UserId = userId;

            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams)  ;

            var messages =  _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);
            Response.AddPagination(messagesFromRepo.CurrentPage ,messagesFromRepo.PageSize,
                messagesFromRepo.TotalCount, messagesFromRepo.TotalPages );

            return Ok(messages);    

        }
        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))           
                return Unauthorized();
            var messagesFromRepo = await _repo.GetMessageThread(userId, recipientId) ;
            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo)  ;

            return Ok(messageThread);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreation)
        {
            var sender = await _repo.GetUser(userId);
             if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))           
                return Unauthorized();

             messageForCreation.SenderId = userId;
             var recipientFromRepo = await _repo.GetUser(messageForCreation.RecipientId);

             if(recipientFromRepo == null)
                return BadRequest("Recipient not found");

            var message = _mapper.Map<Message>(messageForCreation);    

            _repo.Add(message);
            
            if(await _repo.SaveAll()){
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new {id = message.Id}, messageToReturn);
            }
            throw new System.Exception("Creation new message failed on save");     
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId )
        {
            var sender = await _repo.GetUser(userId);
             if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))           
                return Unauthorized();
            var messageFromRepo = await _repo.GetMessage(id);  

            if(messageFromRepo.SenderId == userId)
                messageFromRepo.SenderDeleted = true;
             if(messageFromRepo.RecipientId == userId)
                messageFromRepo.RecipientDeleted = true;  
            if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)  
                _repo.Delete(messageFromRepo)  ;

            if(await _repo.SaveAll())
                return NoContent();

            throw new Exception("Error deleting message");          

        } 
        [HttpPost("{id}/read")]  
        public async Task<IActionResult>  MarkMessageAsRead( int id, int userId){
             if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))           
                return Unauthorized();

             var messageFromRepo = await _repo.GetMessage(id);  
             if(messageFromRepo.RecipientId != userId)   
                return Unauthorized();
             messageFromRepo.IsRead = true;
             messageFromRepo.DateRead = DateTime.Now;
             
              if(await _repo.SaveAll())
                return NoContent();

            throw new Exception("Error marking read message");          
  
        }   
    }
}