﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcessLayer.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        DbSet<TEntity> Get();

        Task<TEntity> GetById(int id);

        Task Add(TEntity entity);

        void Update(TEntity entity);

        Task Delete(int id);
        Task DeleteComplex(object firstKey, object secondKey);
    }
}
