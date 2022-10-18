using System;
using System.Collections.Generic;
using System.Text;

namespace SS_BlogManagement.Application.Interfaces
{
    public interface IUnitWork
    {
        IUserRepository user { get; }
        IPostCategory postCategory { get; }
        IPostRepository post { get; }
    }
}
