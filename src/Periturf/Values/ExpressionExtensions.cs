//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

using System;
using System.Linq.Expressions;

namespace Periturf.Values
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="context"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IValueProviderSpecification<TContext, TValue> Value<TContext, TValue>(this IValueContext<TContext> context, Expression<Func<TContext, TValue>> expression)
        {
            return new ExpressionValueProviderSpecification<TContext, TValue>(expression);
        }
    }
}
