using SS_BlogManagement.Shared.Entities;
using SS_BlogManagement.Shared.Entities.InputModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS_BlogManagement.Application.Interfaces
{
    public interface IUserRepository
    {
        ResCodeMessage CreateUser(RegisteredUser user);
        ResCodeMessage LoginUser(LoginUser user);
    }
}
