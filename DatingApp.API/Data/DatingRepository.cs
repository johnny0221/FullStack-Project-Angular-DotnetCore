using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.helpers;
using DatingApp.API.models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<pagedList<User>> GetUsers(UserParams userparams)
        {
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.Id != userparams.userId);

            users = users.Where(u => u.Gender == userparams.Gender);

            if (userparams.Likers)
            {
                var userLikers = await GetUserLikes(userparams.userId, userparams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userparams.Likees) 
            {   
                var userLikees = await GetUserLikes(userparams.userId, userparams.Likees);
                //to see which users do the current user like.
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            if(userparams.MinAge != 18 || userparams.MaxAge != 99) {
                var maxDob = DateTime.Now.AddYears(-userparams.MinAge);
                var minDob = DateTime.Now.AddYears(-userparams.MaxAge - 1);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if(!string.IsNullOrEmpty(userparams.OrderBy)) {
                switch(userparams.OrderBy) {
                    case "created": 
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            return await pagedList<User>.CreateAsync(users, userparams.PageNumber, userparams.PageSize);
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.Include(u => u.Likers)
                                     .Include(u => u.Likees)
                                     .FirstOrDefaultAsync(u => u.Id == id);
            
            if(likers) {
                // return which current user is a Likee and we pick its Likers.
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            } else {
                // return which current user is a Liker and we pick its Likees.
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }

        public async Task<bool> SaveAll()
        {   
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId) 
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<pagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .AsQueryable();

            // depends on which user input, get which userId.
            switch(messageParams.MessageContainer) {
                case "Inbox":
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
                    break;
                case "Outbox":
                    messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted == false);
                    break;
                default:
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false && u.IsRead == false);
                    break;
            }

            messages = messages.OrderByDescending(m => m.MessageSent);
            return await pagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        // get all the messages instance from the sender and the recepient
        // which means grab all the messages that the senderId is either the user or the recipient
        // and the                                    recipientId is either the user or the reciepient
        public async Task<IEnumerable<Message>> GetMessagesThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m => m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId || 
                m.RecipientId == recipientId && m.SenderDeleted == false && m.SenderId == userId)
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync();

            return messages;
        }
    }
}