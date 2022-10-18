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
   
    public class PostCategoryRepository : IPostCategory
    {
        private readonly IDatabaseConnectionFactory _connectionfactory;

        public PostCategoryRepository(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionfactory = connectionFactory;
        }

        public ResCodeMessage CreatePostCategory(CreatePostCategory category)
        {
            try
            {
                using (IDbConnection conn = _connectionfactory.createConnection())
                {
                    string postCategory_id = Guid.NewGuid().ToString();
                    var res = new ResCodeMessage();
                    PostCategory post_category = new PostCategory
                    {
                        PostCategoryID = postCategory_id,
                        PostCategoryName = category.PostCategoryName,
                        CreatedOn = DateTime.Now,
                        CreatedBy = "1",
                        ModifiedOn = DateTime.Now,
                        ModifiedBy = "1",
                        Active = true
                    };
                    var check_category = conn.Query<PostCategory>("select * from BM_PostCategory where PostCategoryName=@PostCategoryName and Active=1", post_category).FirstOrDefault();
                    if (check_category != null)
                    {
                        res = new ResCodeMessage()
                        {
                            v_data = "",
                            v_rescode = "400",
                            v_resmessage = "Duplicate post category"
                        };
                       
                    }
                    else
                    {
                        var query = "INSERT INTO BM_PostCategory (PostCategoryID,PostCategoryName,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy,Active)" +
                                "VALUES(@PostCategoryID,@PostCategoryName,@CreatedOn,@CreatedBy,@ModifiedOn,@ModifiedBy,@Active)";

                        var result = conn.Execute(query, post_category, commandType: CommandType.Text);
                        if (result > 0)
                        {
                            res = new ResCodeMessage()
                            {
                                v_data = post_category,
                                v_rescode = "201",
                                v_resmessage = "Success"
                            };
                           
                        }
                        else
                        {
                            throw new Exception("Not success for creating category.");
                        }
                    }

                    return res;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResCodeMessage UpdatePostCategory(UpdatePostCategory category)
        {
            try
            {
                using (IDbConnection conn = _connectionfactory.createConnection())
                {
                    var query = "UPDATE BM_PostCategory " +
                                "SET PostCategoryName=@name , ModifiedOn=@modified_on " +
                                "WHERE PostCategoryID=@id AND Active=1";
                    var result = conn.Execute(query, param: new { id = category.PostCategoryID, name = category.PostCategoryName , modified_on=DateTime.Now });
                    if (result > 0)
                    {
                        var results = conn.QueryFirst<PostCategory>("SELECT * FROM BM_PostCategory WHERE PostCategoryID=@id AND Active=1", new { id = category.PostCategoryID });
                        //var jsonResult = JsonConvert.SerializeObject(results);
                        var res = new ResCodeMessage()
                        {
                            v_data = results,
                            v_rescode = "201",
                            v_resmessage = "Success"
                        };
                        return res;
                    }
                    else
                    {
                        throw new Exception("Not success for updating category.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResCodeMessage DeletePostCategory(DeletePostCategory category)
        {
            try
            {
                using (IDbConnection conn = _connectionfactory.createConnection())
                {
                    var res = new ResCodeMessage();
                    var query = "Update BM_PostCategory " +
                                "SET Active=0 , ModifiedOn=@modified_on " +
                                "WHERE PostCategoryID=@id AND Active=1";
                    var result = conn.Execute(query, param: new { id = category.PostCategoryID , modified_on = DateTime.Now }, commandType: CommandType.Text);
                    if(result > 0)
                    {
                        query = "Update BM_Post " +
                           "SET Active=0 , ModifiedOn=@modified_on " +
                           "WHERE PostCategoryID=@categoryID AND Active=1";
                        result = conn.Execute(query, param: new { categoryID = category.PostCategoryID, modified_on = DateTime.Now });
                        res = new ResCodeMessage()
                        {
                            v_data = "",
                            v_rescode = "201",
                            v_resmessage = "Success"
                        };
                      
                    }
                    else
                    {
                        res = new ResCodeMessage()
                        {
                            v_data = "",
                            v_rescode = "003",
                            v_resmessage = "Not found this category."
                        };
                    }
                    return res;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResCodeMessage GetAllPostCategory()
        {
            try
            {
                using (IDbConnection conn = _connectionfactory.createConnection())
                {
                    var sql = "select * from BM_PostCategory Where Active=1";
                    var results = conn.Query<dynamic>(sql).ToList();
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
    }
}
