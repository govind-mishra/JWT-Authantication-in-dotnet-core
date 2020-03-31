using RestApiUsingCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiUsingCore.Services
{
    public interface IPostService
    {
         Task<List<Post>> GetPostsAsync();

        Task<Post> GetPostByIdAsync(Guid id);


        Task<bool> UpdatePostAsync(Post posttoUpdate);

        Task<bool> DeletePostAsync(Guid postid);

        Task<bool> CreatePostAsync(Post posttocreate);
        Task<bool> UserOwnPostAsync(Guid postId, string userId);
    }
}
