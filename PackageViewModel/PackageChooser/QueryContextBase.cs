﻿using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;

namespace PackageExplorerViewModel
{
    internal abstract class QueryContextBase<T>
    {
        private int? _totalItemCount;

        public int TotalItemCount 
        {
            get
            {
                return _totalItemCount ?? 0;
            }
        }

        protected bool TotalItemCountReady
        {
            get
            {
                return _totalItemCount.HasValue;
            }
        }

        public IQueryable<T> Source { get; private set; }

        protected QueryContextBase(IQueryable<T> source)
        {
           
            Source = source;
        }

        protected IEnumerable<T> LoadData(IQueryable<T> query)
        {
            var dataServiceQuery = query as DataServiceQuery<T>;
            if (!TotalItemCountReady && dataServiceQuery != null)
            {
                var queryResponse = (QueryOperationResponse<T>)dataServiceQuery.Execute();
                try
                {
                    _totalItemCount = (int)queryResponse.TotalCount;
                }
                catch (InvalidOperationException)
                {
                    // the server doesn't return $inlinecount value,
                    // fall back to using $count query
                    _totalItemCount = Source.Count();
                }
                return queryResponse;
            }
            else
            {
                if (!TotalItemCountReady)
                {
                    _totalItemCount = Source.Count();
                }

                return query;
            }
        }
    }
}