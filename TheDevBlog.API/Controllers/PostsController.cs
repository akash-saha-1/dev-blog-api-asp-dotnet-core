using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using TheDevBlog.API.Data;
using TheDevBlog.API.Models.DTO;
using TheDevBlog.API.Models.Entities;

namespace TheDevBlog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly DevBlogDbContext devBlogDbContext;

        public PostsController(DevBlogDbContext devBlogDbContext)
        {
            this.devBlogDbContext = devBlogDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await devBlogDbContext.Posts.ToListAsync();
            return Ok(posts);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        [ActionName("GetPostById")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var post = await devBlogDbContext.Posts.FirstOrDefaultAsync(x => x.Id == id);

            if (post != null)  return Ok(post);
            else return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(AddPostRequest addPostRequest) {
            var post = new Post
            {
                Title = addPostRequest.Title,
                Content = addPostRequest.Content,
                Author = addPostRequest.Author,
                FeaturedImageUrl = addPostRequest.FeaturedImageUrl,
                PublishedDate = addPostRequest.PublishedDate,
                UpdatedDate = addPostRequest.UpdatedDate,
                Summary = addPostRequest.Summary,
                UrlHandle = addPostRequest.UrlHandle,
                Visible = addPostRequest.Visible,
                Id = Guid.NewGuid(), // this is optional, entity framework core can generate by its own also
            };

            await devBlogDbContext.Posts.AddAsync(post);
            await devBlogDbContext.SaveChangesAsync();

            return CreatedAtAction("GetPostById", new { id = post.Id }, post);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdatePost([FromRoute] Guid id, UpdatePostRequest updatePostRequest)
        {
            var existingPost = await devBlogDbContext.Posts.FindAsync(id);

            if(existingPost != null)
            {
                existingPost.Title = updatePostRequest.Title;
                existingPost.Content = updatePostRequest.Content;
                existingPost.Author = updatePostRequest.Author;
                existingPost.FeaturedImageUrl = updatePostRequest.FeaturedImageUrl;
                existingPost.PublishedDate = updatePostRequest.PublishedDate;
                existingPost.UpdatedDate = updatePostRequest.UpdatedDate;
                existingPost.Summary = updatePostRequest.Summary;
                existingPost.UrlHandle = updatePostRequest.UrlHandle;
                existingPost.Visible = updatePostRequest.Visible;

                await devBlogDbContext.SaveChangesAsync();

                return Ok(existingPost);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid id)
        {
            var existingPost = await devBlogDbContext.Posts.FindAsync(id);

            if (existingPost != null)
            {
                devBlogDbContext.Posts.Remove(existingPost);
                await devBlogDbContext.SaveChangesAsync();

                return Ok(existingPost);
            }

            return NotFound();
        }

    }
}
