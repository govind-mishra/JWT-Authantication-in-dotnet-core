using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestApiUsingCore.Contracts.v1.Requests;
using RestApiUsingCore.Data;
using RestApiUsingCore.Domain;

namespace RestApiUsingCore.Services
{
    //make this class to make object single time only 
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _dataContext;

        public PostService(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async  Task<bool> DeletePostAsync(Guid postid)
        {
            var post = await GetPostByIdAsync(postid);
            if (post == null)
                return false;
           _dataContext.Posts.Remove(post);
            var deleted =   await _dataContext.SaveChangesAsync();
            return deleted > 0 ;

        }

        public async Task<Post> GetPostByIdAsync(Guid id)
        {
            return await _dataContext.Posts.SingleOrDefaultAsync(x => x.id == id);
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await _dataContext.Posts.ToListAsync();
        }

        public async Task<bool> UpdatePostAsync(Post posttoUpdate)
        {
            _dataContext.Posts.Update(posttoUpdate);
            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0;

        }

        public async Task<bool> CreatePostAsync(Post posttocreate)
        {
            await _dataContext.Posts.AddAsync(posttocreate);
            var added = await _dataContext.SaveChangesAsync();
            return added > 0;

        }

        public async Task<bool> UserOwnPostAsync(Guid postId, string userId)
        {
            var post = await _dataContext.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.id == postId);
            //Above we use asnotracking because we don't want to track this object to remove any conflict of same object for update or delete
            if (post == null)
            {
                return false;
            }
            if (post.userId != userId)
            {
                return false;
            }
            else
                return true;
        }
    }
}
