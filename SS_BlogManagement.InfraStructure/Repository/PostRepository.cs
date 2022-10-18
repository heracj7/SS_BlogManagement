using Dapper;
using Newtonsoft.Json;
using SS_BlogManagement.Application.Interfaces;
using SS_BlogManagement.InfraStructure.ConnectionHelper;
using SS_BlogManagement.Shared.Entities;
using SS_BlogManagement.Shared.Entities.InputModels;
using SS_BlogManagement.Shared.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_BlogManagement.InfraStructure.Repository
{
    public class PostRepository: IPostRepository
    {
        private readonly IDatabaseConnectionFactory _connectionfactory;

        public PostRepository(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionfactory = connectionFactory;
        }

        public ResCodeMessage CreatePost(CreatePost post , string post_user_id)
        {
            try
            {
                using (IDbConnection conn = _connectionfactory.createConnection())
                {
                    string post_id = Guid.NewGuid().ToString();
                    var post_param = new 
                    {
                        PostID = post_id,
                        UserID = post_user_id,
                        PostTitle = post.PostTitle,
                        PostContent = post.PostContent,
                        PostStatus = post.PostStatus,
                        PostCategoryID = post.PostCategoryID,
                    };
                   
                    var result = conn.Query<dynamic>("CreatePost", post_param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    var jsonResult = JsonConvert.SerializeObject(result);
                    var res = new ResCodeMessage()
                    {
                        v_data = result,
                        v_rescode = "201",
                        v_resmessage = "Success"
                    };
                    return res;
                }
            }
            catch (Exception ex)
            {
               throw ex;
            }
        }

        public ResCodeMessage UpdatePost(UpdatePost post)
        {
            try
            {
                using (IDbConnection conn = _connectionfactory.createConnection())
                {
                    string post_id = Guid.NewGuid().ToString();
                    var post_param = new 
                    {
                        PostID = post.PostID,
                        PostTitle = post.PostTitle,
                        PostContent = post.PostContent,
                        PostStatus = post.PostStatus,
                        PostCategoryID = post.PostCategoryID,
                    };

                    var result = conn.Query<dynamic>("UpdatePost", post_param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if(result != null)
                    {
                        var jsonResult = JsonConvert.SerializeObject(result);
                        var res = new ResCodeMessage()
                        {
                            v_data = result,
                            v_rescode = "201",
                            v_resmessage = "Success"
                        };
                        return res;
                    }
                    throw new Exception("No success for updating post.");
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResCodeMessage DeletePost(DeletePost post)
        {
            try
            {
                using (IDbConnection conn = _connectionfactory.createConnection())
                {
                    var query = "Update BM_Post " +
                                "SET Active=0 , ModifiedOn=@modified_on " +
                                "WHERE PostID=@id AND Active=1";
                    var result = conn.Execute(query, param: new { id = post.PostID, modified_on = DateTime.Now }, commandType: CommandType.Text);
                    if (result == 0) {
                        throw new Exception("Not success for deleting post.");
                    }
                    var res = new ResCodeMessage()
                    {
                        v_data = "",
                        v_rescode = "201",
                        v_resmessage = "Success"
                    };
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResCodeMessage GetAllPost(GetSearchPost post)
        {
            try
            {
                using (IDbConnection conn = _connectionfactory.createConnection())
                {
                    int _page_no = post.pageNo;
                    int _page_record_count = post.pageSize == 0 ? 10 : post.pageSize;
                    int _skip_record = _page_no * _page_record_count;

                    post.postCategoryID = post.postCategoryID ?? "all";
                    post.userID = post.userID ?? "all";
                    post.postStatus= String.IsNullOrEmpty(post.postStatus) ? "published" : post.postStatus.ToLower();
                    post.searchText = String.IsNullOrEmpty(post.searchText) ? "" : post.searchText.ToLower();

                    var sql = "select * from BM_PostView " +
                              "Where Active=1 " +
                              "and LOWER(PostStatus)=@postStatus " +
                              "and (@userID='all' Or UserID=@userID )  " +
                              "and (@postCategoryID='all' Or PostCategoryID=@postCategoryID )  "+
                              "and (IsNull(@searchText, '') = '' Or (LOWER(PostTitle) LIKE '%@searchText%' OR  LOWER(PostCategoryName) LIKE '%@searchText%' OR LOWER(AuthorName) LIKE '%@searchText%')) " +
                              "order by CreatedOn desc";

                    var result_query = conn.Query<dynamic>(sql,post).ToList();
                    var result_list = result_query.Skip(_skip_record).Take(_page_record_count).ToList();
                    int total_record_count = result_query.Count();
                    int total_page = total_record_count / _page_record_count;
                    if (total_record_count % _page_record_count > 0) total_page++;

                    dynamic return_result = new System.Dynamic.ExpandoObject();
                    return_result.totalRecord = total_record_count.ToString();
                    return_result.totalPage = total_page.ToString();
                    return_result.currentPageNo = (++_page_no).ToString();
                    return_result.dataList = result_list;

                    var res = new ResCodeMessage()
                    {
                        v_data = return_result,
                        v_rescode = "201",
                        v_resmessage = "Success"
                    };
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResCodeMessage GetPostByID(string id)
        {
            try
            {
                using (IDbConnection conn = _connectionfactory.createConnection())
                {
                    var parameter = new
                    {
                        post_id = id
                    };
                    var sql = "select * from BM_PostView Where PostID=@post_id and Active=1";
                    var results = conn.Query<dynamic>(sql,parameter).FirstOrDefault();
                    var jsonResult = JsonConvert.SerializeObject(results);
                    var res = new ResCodeMessage()
                    {
                        v_data = results,// jsonResult,
                        v_rescode = "201",
                        v_resmessage = "Success"
                    };
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResCodeMessage DeleteMultiPostCategory(List<DeletePost> posts)
        {
            try
            {
                using (IDbConnection conn = _connectionfactory.createConnection())
                {
                   
                    var query = "Update FROM Post Acitve=1 and ModifiedOn=@modified_on WHERE Id IN @ids";
                    var result = conn.Execute(query, param: new { ids = posts.Select(m => m.PostID) , modified_on=DateTime.Now }, commandType: CommandType.Text);
                    if (result > 0)
                    {
                        var res = new ResCodeMessage()
                        {
                            v_data = "",
                            v_rescode = "201",
                            v_resmessage = "Success"
                        };
                        return res;
                    }
                    else
                    {
                        throw new Exception("Not success for creating category.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
     
    }
}
