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

namespace SS_BlogManagement.InfraStructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseConnectionFactory _connectionfactory;

        public UserRepository(IDatabaseConnectionFactory connectionFactory)
        {
            _connectionfactory = connectionFactory;
        }

        public ResCodeMessage CreateUser(RegisteredUser user)
        {
            try
            {
                using (IDbConnection conn = _connectionfactory.createConnection())
                {
                    var res = new ResCodeMessage();
                    string user_id = Guid.NewGuid().ToString();
                    var parameters = new {
                        UserID = user_id,
                        Name = user.Name,
                        Phone = user.PhoneNumber,
                        Email = user.Email,
                        PasswordHash = user.Password,
                        RegisteredOn = DateTime.Now,
                        CreatedOn = DateTime.Now,
                        CreatedBy = user_id,
                        ModifiedOn = DateTime.Now,
                        ModifiedBy = user_id,
                        LastLogin = DateTime.Now,
                        Active = true
                    };

                    var check_user = conn.Query<dynamic>("select * from BM_User where (Phone=@Phone or Email=@Email) and Active=1", parameters).FirstOrDefault();
                    if (check_user!=null)
                    {
                        res = new ResCodeMessage()
                        {
                            v_data = "",
                            v_rescode = "400",
                            v_resmessage = "Duplicate email or phone."
                        };
                    }
                    else
                    {
                        var query = "INSERT INTO BM_User (UserID, Name,Phone,Email,PasswordHash,RegisteredOn,CreatedOn,CreatedBy,ModifiedOn,ModifiedBy,LastLogin,Active)" +
                                    "VALUES(@UserID, @Name,@Phone,@Email,@PasswordHash,@RegisteredOn,@CreatedOn,@CreatedBy,@ModifiedOn,@ModifiedBy,@LastLogin,@Active)";

                        var result = conn.Execute(query, parameters, commandType: CommandType.Text);
                        if (result > 0)
                        {
                            res = new ResCodeMessage()
                            {
                                v_data = parameters,
                                v_rescode = "201",
                                v_resmessage = "Success"
                            };
                        
                        }
                        else
                        {
                            throw new Exception("Not success for creating user.");
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

        public ResCodeMessage LoginUser(LoginUser user)
        {
            try
            {
                using (IDbConnection conn = _connectionfactory.createConnection())
                {
                    User responseData = conn.Query<User>("UserLogin",
                         new
                         {
                             EmailOrPhone = user.EmailOrPhone,
                             PasswordHash = user.Password
                         },
                         commandType: CommandType.StoredProcedure).FirstOrDefault();
                    _connectionfactory.closeConnection();
                    ResCodeMessage res = new ResCodeMessage();
                    if (responseData != null)
                    {
                        res = new ResCodeMessage()
                        {
                            v_data = responseData,// JsonConvert.SerializeObject(responseData),
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
                            v_resmessage = "Not Found"
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
    }
}
