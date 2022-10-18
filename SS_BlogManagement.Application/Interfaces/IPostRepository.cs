using SS_BlogManagement.Shared.Entities;
using SS_BlogManagement.Shared.Entities.InputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_BlogManagement.Application.Interfaces
{
    public interface IPostRepository
    {
        ResCodeMessage CreatePost(CreatePost post, string post_user_id);
        ResCodeMessage UpdatePost(UpdatePost post);
        ResCodeMessage DeletePost(DeletePost post);
        ResCodeMessage DeleteMultiPostCategory(List<DeletePost> posts);
        ResCodeMessage GetAllPost(GetSearchPost post);
        ResCodeMessage GetPostByID(string id);
    }
}
