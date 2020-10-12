using Notebook.Domain.Entity;
using System;
using System.Linq.Expressions;

namespace Notebook.Database
{
    public static class Helpers
    {
        /// <summary>
        /// Filtering by time of records (weekly, monthly...)
        /// </summary>
        /// <param name="start">Start date for taking time range</param>
        /// <param name="end">End date for taking range of time</param>
        /// <returns>Part of the filter expression which will be pass in where part</returns>
        public static Expression<Func<Record, bool>> RecordsFilteredByDate(DateTime? start, DateTime? end)  //TODO 
        {
            Expression<Func<Record, bool>> temp = x => true; //what will be pass in .Where()
            if (start != null && end != null)
            {
                temp = x =>
                    x.StartDate >= start && x.EndDate <= end; 
            }

            if (start != null && end == null)
            {
                temp = x =>
                    x.StartDate >= start; 
            }

            if (start == null && end != null)
            {
                temp = x =>
                    x.EndDate <= end;
            } 

            return temp;
        }
    }
}