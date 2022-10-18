using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_BlogManagement.Shared.Entities.InputModels
{
    public class CreatePost
    {
        [Required]
        public string PostTitle { get; set; }
        [Required]
        public string PostContent { get; set; }
        [Required]
        public string PostStatus { get; set; }
        [Required]
        public string PostCategoryID { get; set; }
    }

    public class UpdatePost
    {
        [Required]
        public string PostID { get; set; }
        [Required]
        public string PostTitle { get; set; }
        [Required]
        public string PostContent { get; set; }
        [Required]
        public string PostStatus { get; set; }
        [Required]
        public string PostCategoryID { get; set; }
    }

    public class DeletePost
    {
        [Required]
        public string PostID { get; set; }
    }

    public class GetSearchPost
    {
     
        public int pageSize { get; set; }
        public int pageNo { get; set; }
        public string postCategoryID { get; set; }
        public string userID { get; set; }
        public string postStatus { get; set; }
        public string searchText { get; set; }

    }


}
