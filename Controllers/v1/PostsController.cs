using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApiUsingCore.Contracts;
using RestApiUsingCore.Contracts.v1;
using RestApiUsingCore.Contracts.v1.Requests;
using RestApiUsingCore.Contracts.v1.Responses;
using RestApiUsingCore.Domain;
using RestApiUsingCore.Extensions;
using RestApiUsingCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiUsingCore.Controllers.v1
{
    [Authorize(AuthenticationSchemes =  JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {
        //make the list of posts 
        private readonly IPostService _postService; 

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return Ok( await _postService.GetPostsAsync());
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = _postService.GetPostByIdAsync(postId);
            if (post == null)
                return NotFound();
            return Ok(await post);
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public IActionResult Create([FromBody] CreatePostRequest postRequest)
        {
            //you should not mix up domain with your request because of versioning
            var post = new Post {
                Name = postRequest.Name,
                userId = HttpContext.getUserId()
            };
            _postService.CreatePostAsync(post);

            var baseUrl = HttpContext.Request.Scheme + "/" + HttpContext.Request.Host.ToUriComponent();
            var locationurl = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.id.ToString());
            //seperate response object from real domain
            var postResponse = new PostResponse { Id = post.id.ToString() };
            return Created(locationurl, postResponse);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId,[FromBody] UpdatePostRequest updaterequest)
        {
            var userOwnsPost = await _postService.UserOwnPostAsync(postId,HttpContext.getUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new { error = "You don't own this post" });
            }

            var post = await _postService.GetPostByIdAsync(postId);
            post.Name = updaterequest.Name;
            var updated = await _postService.UpdatePostAsync(post);

            if (updated)
                return Ok(post);
            else
            {
                return NotFound();
            }

        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnPostAsync(postId, HttpContext.getUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new { error = "You don't own this post" });
            }
            var deleted = await _postService.DeletePostAsync(postId);
            if (deleted)
                return NoContent();
            else
                return NotFound();
                
        }
    }
}
