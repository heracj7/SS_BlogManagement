using SS_BlogManagement.Shared.Entities;
using SS_BlogManagement.Shared.Entities.InputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_BlogManagement.Application.Interfaces
{
    public interface IPostCategory
    {
        ResCodeMessage CreatePostCategory(CreatePostCategory user);
        ResCodeMessage UpdatePostCategory(UpdatePostCategory category);
        ResCodeMessage DeletePostCategory(DeletePostCategory category);
        ResCodeMessage GetAllPostCategory();
    }
}
