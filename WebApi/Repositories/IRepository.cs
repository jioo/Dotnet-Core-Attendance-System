﻿using System;
using System.Threading.Tasks;

namespace WebApi.Repositories
{
    public interface IRepository<T> : IDisposable
    {
        IContextRepository<T> Context { get; }
        void Save();
        Task<int> SaveAsync();
    }
}