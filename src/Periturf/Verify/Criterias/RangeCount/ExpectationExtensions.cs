/*
 *     Copyright 2019 Adam Burton (adz21c@gmail.com)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using Periturf.Verify;
using Periturf.Verify.Criterias.RangeCount;
using System;

namespace Periturf
{
    /// <summary>
    /// Extensions for configuring criteria
    /// </summary>
    public static class ExpectationExtensions
    {
        /// <summary>
        /// Must occur the minimum number of times.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="minimum">The minimum times.</param>
        /// <returns></returns>
        public static IExpectationConfigurator MustOccurMinimumTimes(this IExpectationConfigurator configurator, int minimum)
        {
            configurator.Must(
                new RangeCountCriteriaSpecification(
                    minimum,
                    null,
                    null,
                    $"Must occur a minimum of {minimum} times"));
            return configurator;
        }


        /// <summary>
        /// Must occur the minimum number of times within the specified timescale.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="minimum">The minimum times.</param>
        /// <param name="timeScale">The time scale.</param>
        /// <returns></returns>
        public static IExpectationConfigurator MustOccurMinimumTimesWithin(this IExpectationConfigurator configurator, int minimum, TimeSpan timeScale)
        {
            configurator.Must(
                new RangeCountCriteriaSpecification(
                    minimum,
                    null,
                    timeScale,
                    $"Must occur a minimum of {minimum} times within {timeScale.TotalMilliseconds}ms"));
            return configurator;
        }

        /// <summary>
        /// Must occur the maximum times.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="maximum">The maximum times.</param>
        /// <returns></returns>
        public static IExpectationConfigurator MustOccurMaximumTimes(this IExpectationConfigurator configurator, int maximum)
        {
            configurator.Must(
                new RangeCountCriteriaSpecification(
                    null,
                    maximum,
                    null,
                    $"Must occur a maximum of {maximum} times"));
            return configurator;
        }


        /// <summary>
        /// Must occur the maximum times within the time scale.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="maximum">The maximum times.</param>
        /// <param name="timeScale">The time scale.</param>
        /// <returns></returns>
        public static IExpectationConfigurator MustOccurMaximumTimesWithin(this IExpectationConfigurator configurator, int maximum, TimeSpan timeScale)
        {
            configurator.Must(
                new RangeCountCriteriaSpecification(
                    null,
                    maximum,
                    timeScale,
                    $"Must occur a maximum of {maximum} times within {timeScale.TotalMilliseconds}ms"));
            return configurator;
        }

        /// <summary>
        /// Must never occur.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IExpectationConfigurator MustNeverOccur(this IExpectationConfigurator configurator)
        {
            configurator.Must(
                new RangeCountCriteriaSpecification(
                    null,
                    0,
                    null,
                    "Must never occur"));
            return configurator;
        }

        /// <summary>
        /// Must never occur within the time scale.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="timeScale">The time scale.</param>
        /// <returns></returns>
        public static IExpectationConfigurator MustNeverOccurWithin(this IExpectationConfigurator configurator, TimeSpan timeScale)
        {
            configurator.Must(
                new RangeCountCriteriaSpecification(
                    null,
                    0,
                    timeScale,
                    $"Must never occur within {timeScale.TotalMilliseconds}ms"));
            return configurator;
        }

        /// <summary>
        /// Must occur.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns></returns>
        public static IExpectationConfigurator MustOccur(this IExpectationConfigurator configurator)
        {
            configurator.Must(
                new RangeCountCriteriaSpecification(
                    1,
                    null,
                    null,
                    "Must occur"));
            return configurator;
        }

        /// <summary>
        /// Must occur within the time scale.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="timeScale">The time scale.</param>
        /// <returns></returns>
        public static IExpectationConfigurator MustOccurWithin(this IExpectationConfigurator configurator, TimeSpan timeScale)
        {
            configurator.Must(
                new RangeCountCriteriaSpecification(
                    1,
                    null,
                    timeScale,
                    $"Must occur within {timeScale.TotalMilliseconds }ms"));
            return configurator;
        }

        /// <summary>
        /// Must occur between a number of times.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="minimum">The minimum times.</param>
        /// <param name="maximum">The maximum times.</param>
        /// <returns></returns>
        public static IExpectationConfigurator MustOccurBetweenTimes(this IExpectationConfigurator configurator, int minimum, int maximum)
        {
            configurator.Must(
                new RangeCountCriteriaSpecification(
                    minimum,
                    maximum,
                    null,
                    $"Must occur between {minimum} and {maximum} times"));
            return configurator;
        }

        /// <summary>
        /// Must occur between a number of times within a time scale.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="minimum">The minimum times.</param>
        /// <param name="maximum">The maximum times.</param>
        /// <param name="timeScale">The time scale.</param>
        /// <returns></returns>
        public static IExpectationConfigurator MustOccurBetweenTimesWithin(this IExpectationConfigurator configurator, int minimum, int maximum, TimeSpan timeScale)
        {
            configurator.Must(
                new RangeCountCriteriaSpecification(
                    minimum,
                    maximum,
                    timeScale,
                    $"Must occur between {minimum} and {maximum} times within {timeScale.TotalMilliseconds}ms"));
            return configurator;
        }
    }
}
