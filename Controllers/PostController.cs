using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using DotnetAPI.Dtos;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;


        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        
        }

        [HttpGet("Posts/{postId}/{userId}/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "None") 
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get";
            string parameters = "";
            if (postId != 0)
            {
                parameters += ", @PostId = " + postId.ToString();
            }
            if (userId != 0)
            {
                parameters += ", @UserId = " + userId.ToString();
            }
            if (searchParam != "None")
            {
                parameters += ", @SearchValue= '" + searchParam + "'";
            }
            if (parameters.Length > 0)
            {
                            sql += parameters.Substring(1); // remove the first comma from the parameters string and append to the SQL query
            }
            Console.WriteLine(sql);

            return _dapper.LoadData<Post>(sql);
        }

        [HttpGet("PostSingle/{postId}")]
        public Post GetSinglePost(int postId)
        {
            string sql = @"SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated]
                        FROM TutorialAppSchema.Posts
                        WHERE PostId = " + postId.ToString();
            return _dapper.LoadDataSingle<Post>(sql);
        }

        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            string sql = @"SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated]
                        FROM TutorialAppSchema.Posts
                        WHERE UserId = " + userId.ToString();
            return _dapper.LoadData<Post>(sql);
        }

        [HttpPost("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId = " 
            + this.User.FindFirst("userId")?.Value ?? "";
            return _dapper.LoadData<Post>(sql);

        }

        [HttpPost("UpsertPost")]
        public IActionResult UpsertPost(PostToAddDto postUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
    @UserId = " + this.User.FindFirst("userId")?.Value + 
    ", @PostTitle = '" + postUpsert.PostTitle + @"',
    @PostContent = '" + postUpsert.PostContent + "'";

    if (postUpsert.PostId > 0)
            {
                sql += ", @PostId = " + postUpsert.PostId.ToString();
                
            }
    
            if (_dapper.ExecuteSql(sql))
            {
                return Ok(postUpsert);
            }
            throw new Exception("Adding Post failed on save");
        }

        // [HttpPut("EditPost")]
        // public IActionResult EditPost(PostToEditDto post)
        // {
        //     string sql = "UPDATE TutorialAppSchema.Posts SET PostTitle='" 
        //         + post.PostTitle
        //         + "', PostContent='"
        //         + post.PostContent
        //         + "', PostUpdated=GETDATE() WHERE PostId=" + post.PostId.ToString()
        //         + " AND UserId=" + this.User.FindFirst("userId")?.Value;
        //     if (_dapper.ExecuteSql(sql))
        //     {
        //         return Ok(post);
        //     }
        //     throw new Exception("Updating Post failed on save");
        // }

        [HttpDelete("DeletePost/{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = "EXEC TutorialAppSchema.spPosts_Delete @PostId=" 
            + postId.ToString() + ", @UserId=" + this.User.FindFirst("userId")?.Value;
               
            if (_dapper.ExecuteSql(sql))
            {
                return Ok("Post Deleted Successfully");
            }
            throw new Exception("Deleting Post failed on save");
        }

        [HttpGet("PostsBySearch/{searchString}")]
        public IEnumerable<Post> GetPostsBySearch(string searchString)
        {
            string sql = @"SELECT [PostId],
                        [UserId],
                        [PostTitle],
                        [PostContent],
                        [PostCreated],
                        [PostUpdated]
                        FROM TutorialAppSchema.Posts
                        WHERE PostTitle LIKE '%" + searchString + "%'"
                        + " OR PostContent LIKE '%" + searchString + "%'";
            return _dapper.LoadData<Post>(sql);
        }


            




    }
}
