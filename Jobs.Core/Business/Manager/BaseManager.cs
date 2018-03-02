using Jobs.Core.Entity;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Jobs.Core.Business.Manager
{
    public class BaseManager<T> where T:class ,new ()
    {
        public SqlSugarClient db
        {
            get
            {
                return new DbManager().db;
            }
        }

        public  bool BatchAdd(List<T> tlst)
        {

            return db.Insertable(tlst.ToArray()).ExecuteCommand() > 0;
        }

        public bool Add(T t)
        {
            return db.Insertable(t).ExecuteCommand()>0;
        }

        public int AddReturnId(T t)
        {
            return db.Insertable(t).ExecuteReturnIdentity();
        }

        public List<T> Query(Expression<Func<T,bool>> exp)
        {
            return db.Queryable<T>().Where(exp).ToList();
        }
    }

    public class BaseManager
    {
        public SqlSugarClient db
        {
            get
            {
                return new DbManager().db;
            }
        }

       
    }
}
