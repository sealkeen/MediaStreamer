using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sealkeen.Linq.Extensions
{
    public static class DbSetExtensions
    {
        public static Task<List<T>> CreateListAsync<T>(this IQueryable<T> query)
        {
            var tsk = Task.Factory.StartNew(new Func<List<T>>(delegate 
            {
                return query.ToList();
            }));

            return tsk;
        }
    }
}
