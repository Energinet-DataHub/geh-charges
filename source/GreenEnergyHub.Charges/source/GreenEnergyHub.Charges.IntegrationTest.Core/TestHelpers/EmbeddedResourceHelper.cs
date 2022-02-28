﻿// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using NodaTime;

namespace GreenEnergyHub.Charges.IntegrationTest.Core.TestHelpers
{
    public static class EmbeddedResourceHelper
    {
        private const string TimeAndPriceSeriesDateTimeFormat = "yyyy-MM-dd\\THH:mm\\Z";
        private const string CreatedDateTimeFormat = "yyyy-MM-dd\\THH:mm:ss\\Z";

        public static string GetEmbeddedFile(string filePath, [NotNull] IClock clock)
        {
            var basePath = Assembly.GetExecutingAssembly().Location;
            var path = Path.Combine(Directory.GetParent(basePath)!.FullName, filePath);
            var fileText = File.ReadAllText(path);
            return ReplaceMergeFields(clock, fileText);
        }

        private static string ReplaceMergeFields(IClock clock, string file)
        {
            var currentInstant = clock.GetCurrentInstant();
            var now = currentInstant.ToString();
            var inThirtyoneDays = currentInstant.Plus(Duration.FromDays(31));

            // cim:timeInterval does not allow seconds.
            var ymdhmTimeInterval = inThirtyoneDays
                .ToString(TimeAndPriceSeriesDateTimeFormat, CultureInfo.InvariantCulture);

            // cim:createdDateTime and effective date must have seconds
            var ymdhmsTimeInterval = currentInstant.ToString(CreatedDateTimeFormat, CultureInfo.InvariantCulture);

            var replacementIndex = 0;
            var mergedFile = Regex.Replace(file, "[{][{][$]increment5digits[}][}]", _ =>
            {
                replacementIndex++;
                return replacementIndex.ToString("D5");
            });

            return mergedFile
                .Replace("{{$randomCharacters}}", Guid.NewGuid().ToString("n")[..10])
                .Replace("{{$randomCharactersShort}}", Guid.NewGuid().ToString("n")[..5])
                .Replace("{{$isoTimestamp}}", now)
                .Replace("{{$isoTimestampPlusOneMonth}}", inThirtyoneDays.ToString())
                .Replace("{{$YMDHM_TimestampPlusOneMonth}}", ymdhmTimeInterval)
                .Replace("ISO8601Timestamp", ymdhmsTimeInterval);
        }
    }
}