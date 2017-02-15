﻿using EntityFramework.SharedRepository;
using EntityFrameworkRepository6.Base;
using PersistentLayer.Contexts;
using PersistentLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistentLayer.Repositories
{
    public interface ISimpleCompositeKeyEntityRepository : IBaseRepository<YourCustomDataContext, SimpleCompositeKeyEntity>
    {

    }

    public class SimpleCompositeKeyEntityRepository : BaseRepository<YourCustomDataContext, SimpleCompositeKeyEntity>, ISimpleCompositeKeyEntityRepository
    {
        public SimpleCompositeKeyEntityRepository(IDatabaseFactory<YourCustomDataContext> dbFactory) : base(dbFactory.GetNewDbContext())
        {
            
        }
    }
}
