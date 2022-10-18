using Microsoft.Extensions.DependencyInjection;
using SS_BlogManagement.Application.Interfaces;
using SS_BlogManagement.InfraStructure.Repository;
using SS_BlogManagement.InfraStructure.ResponseHandler;
using System;
using System.Collections.Generic;
using System.Text;

namespace SS_BlogManagement.InfraStructure
{
    public static class ServiceRegistration
    {
        public static void AddInfraStructure(this IServiceCollection services)
        {

            services.AddTransient<IUnitWork, UnitWork>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IPostCategory, PostCategoryRepository>();
            services.AddTransient<IPostRepository, PostRepository>();
        }

        public static void AddResponseConfig(this IServiceCollection services)
        {
            services.AddSingleton<IResponseHandler, ResponseDataHandler>();
        }
    }
}
