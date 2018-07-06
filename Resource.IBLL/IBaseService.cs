﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Resource.IBLL
{
    public partial interface IBaseService<T> where T :class ,new ()
    {

        bool Add(T t);
        bool Delete(T t);
        bool Update(T t);
        IQueryable<T> GetModels(Expression<Func<T, bool>> whereLambda);
        IQueryable<T> GetModelsByPage<type>(int pageSize, int pageIndex, bool isAsc, Expression<Func<T, bool>> WhereLambda,Expression<Func<T, type>> OrderLambda);
        int Update(string sqlText, params SqlParameter[] parameter);
        DataSet ExecuteSql(string sqlText, CommandType cmdType, params SqlParameter[] parameters);
    }
}
