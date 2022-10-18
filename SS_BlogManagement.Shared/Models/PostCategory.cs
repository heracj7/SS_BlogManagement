using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS_BlogManagement.Shared.Models
{
    public class PostCategory
    {
        public string PostCategoryID { get; set; }
        public string PostCategoryName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public bool Active { get; set; }
    }

    

}
