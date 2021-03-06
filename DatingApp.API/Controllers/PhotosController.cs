using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOS;
using DatingApp.API.helpers;
using DatingApp.API.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]

    public class PhotosController: ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private IOptions<CloudinarySettings> _options;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _repo = repo;
            _mapper = mapper;
            _options = cloudinaryConfig;

            Account acc = new Account(
                cloudinaryConfig.Value.CloudName,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret
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
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDto photoForCreationDto)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);
            
            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if(file.Length > 0) {
                // 讀取傳進來的file
                using (var stream = file.OpenReadStream()) {
                    var uploadParams = new ImageUploadParams() {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            // mutate the Dto with the response.
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            // map the Dto to Photo entity.
            var photo = _mapper.Map<Photo>(photoForCreationDto);

            if(!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;
            
            // add the newly created photo model to the user.
            userFromRepo.Photos.Add(photo);

            // save the userFromRepo entity because we mutate it.
            if(await _repo.SaveAll()) {
                // we map here because SQLite is responsible for generating the id of the "photo" object, we have to wait till it's saved.
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                // CreatedAtRoute is a symbol of 201(user successfully created something)
                return CreatedAtRoute("GetPhoto", new { userId = userId, id = photo.Id}, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id) {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            var user = await _repo.GetUser(userId);
            if(!user.Photos.Any(p => p.Id == id)) {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);

            if(photoFromRepo.IsMain == true) {
                return BadRequest("This is already the main photo");
            }

            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);

            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if(await _repo.SaveAll()) {
                return NoContent();
            }

            return BadRequest("Cannot set the photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            var user = await _repo.GetUser(userId);
            if(!user.Photos.Any(p => p.Id == id)) {
                return Unauthorized();
            }

            var photoFromRepo = await _repo.GetPhoto(id);

            if(photoFromRepo.IsMain == true) {
                return BadRequest("You cannot delete your main photo");
            }

            if(photoFromRepo.PublicId != null) {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);

                if(result.Result == "ok") {
                    _repo.Delete(photoFromRepo);
                }
            }

            if(photoFromRepo.PublicId == null) {
                _repo.Delete(photoFromRepo);

            }

            if(await _repo.SaveAll()) {
                return Ok();
            }

            return BadRequest("failed to delete the photo");
        }
        
    }
}