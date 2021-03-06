﻿using System;
using Data.UnitOfWork.EF.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Data.UnitOfWork.EF
{
    public static class DependencyInjection
    {
        public static void AddUnitOfWork<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null) where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>(optionsAction);

            services.AddScoped<DbContext, TDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IRepository<,>), typeof(Repository<,>));
        }
    }
}
