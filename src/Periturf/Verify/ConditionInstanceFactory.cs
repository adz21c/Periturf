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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Periturf.Verify
{
    class ConditionInstanceFactory : IConditionInstanceFactory
    {
        private readonly ITime _time;
        private DateTime? _start;

        public ConditionInstanceFactory(ITime time)
        {
            _time = time;
        }

        public void Start()
        {
            _start = _time.Now;
        }

        public ConditionInstance Create(string id)
        {
            return Create(id, _time.Now);
        }

        public ConditionInstance Create(string id, DateTime eventDateTime)
        {
            var timespan = _start.HasValue ? eventDateTime - _start.Value : TimeSpan.Zero;
            return new ConditionInstance(timespan, id);
        }
    }
}
