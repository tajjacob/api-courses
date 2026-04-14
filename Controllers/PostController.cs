using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using Dapper;
using System.Data;

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
            string stringParameters = "";

            DynamicParameters sqlParameters = new DynamicParameters();

            if (postId != 0)
            {
                stringParameters += ", @PostId=@PostIdParameter";
                sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
            }
            if (userId != 0)
            {
                stringParameters += ", @UserId=@UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }
            if (searchParam != "None")
            {
                stringParameters += ", @SearchValue=@SearchValueParameter";
                sqlParameters.Add("@SearchValueParameter", searchParam, DbType.String);
            }
            if (stringParameters.Length > 0)
            {
                            sql += stringParameters.Substring(1); // remove the first comma from the parameters string and append to the SQL query
            }
            Console.WriteLine(sql);

            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
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
            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId=@UserIdParameter";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
            return _dapper.LoadData<Post>(sql);

        }

        [HttpPost("UpsertPost")]
        public IActionResult UpsertPost(PostToAddDto postUpsert)
        {
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
            @UserId=@PostUserIdParameter,
            @PostTitle=@PostTitleParameter,
            @PostContent=@PostContentParameter";

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@PostUserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameters.Add("@PostTitleParameter", postUpsert.PostTitle, DbType.String);
            sqlParameters.Add("@PostContentParameter", postUpsert.PostContent, DbType.String);

    if (postUpsert.PostId > 0)
            {
                sql += ", @PostId=@PostIdParameter";
                sqlParameters.Add("@PostIdParameter", postUpsert.PostId, DbType.Int32);
                
            }
    
            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))

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
            string sql = @"EXEC TutorialAppSchema.spPosts_Delete 
            @PostId=@PostIdParameter, 
            @UserId=@UserIdParameter";
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@PostIdParameter", postId, DbType.Int32);
            sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
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
