using SS_BlogManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS_BlogManagement.InfraStructure.Repository
{
    public class UnitWork : IUnitWork
    {
        public IUserRepository user { get; }
        public IPostCategory postCategory { get; }
        public IPostRepository post { get; }

        public UnitWork(IUserRepository user_repo,IPostCategory postcategory_repo, IPostRepository post_repo)
        {
            user = user_repo;
            postCategory = postcategory_repo;
            post= post_repo;

        }

    }
}
