﻿using MediaStreamer.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MediaStreamer.DataAccess.NetStandard.Extensions
{
    public static class DbContextExtensions
    {
        public static void DetachLocal<T>(this DbContext context, T t, string entryId) 
            where T : MediaEntity
        {
            var local = context.Set<T>()
                .Local
                .FirstOrDefault(entry => entry.GetId().Equals(entryId));
            if (local != null)
            {
                context.Entry(local).State = EntityState.Detached;
            }
            context.Entry(t).State = EntityState.Modified;
        }
    }
}