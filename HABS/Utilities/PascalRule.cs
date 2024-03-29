﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class PascalRule : IRule
    {
        public void ApplyRule(RewriteContext context)
        {
            if (context.HttpContext.Request.Path.HasValue)
            {
                var path = context.HttpContext.Request.Path.Value;
                if (path.Contains("payments/vnpay"))
                {
                    return;
                }
            }
            Dictionary<string, StringValues> newQueryCollection = context.HttpContext.Request.Query.ToDictionary(
                kv => ToPascalCase(kv.Key),
                kv => kv.Value
            );

            context.HttpContext.Request.Query = new QueryCollection(newQueryCollection);
        }

        private static string ToPascalCase(string kebabCase)
        {
            return string.Join(string.Empty,
                kebabCase.Split(new char[] { '-', '_' })
                    .Select(str => str.Length > 0 ? char.ToUpper(str[0]) + str.Substring(1) : str));
        }
    }
}
