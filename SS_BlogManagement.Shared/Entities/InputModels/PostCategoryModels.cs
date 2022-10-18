using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_BlogManagement.Shared.Entities.InputModels
{
    public class CreatePostCategory
    {
        [Required]
        public string PostCategoryName { get; set; }
        
    }

    public class DeletePostCategory
    {
        [Required]
        public string PostCategoryID { get; set; }
    }

    public class UpdatePostCategory
    {
        [Required]
        public string PostCategoryID { get; set; }
        [Required]
        public string PostCategoryName { get; set; }
    }

   
}
