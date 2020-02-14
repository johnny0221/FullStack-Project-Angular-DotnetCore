using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.helpers;
using DatingApp.API.models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
        // T must be a reference type e.g. class, struct, delegate.
         void Add<T>(T entity) where T: class;

         void Delete<T>(T entity) where T: class;

         Task<bool> SaveAll();

         Task<pagedList<User>> GetUsers(UserParams userParams);

         Task<User> GetUser(int id);

         Task<Photo> GetPhoto(int id);

         Task<Photo> GetMainPhotoForUser(int userId);

         Task<Like> GetLike(int userId, int recipientId);
    }
}