// Copyright 2020 Energinet DataHub A/S
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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using GreenEnergyHub.Charges.Core.DateTime;
using NodaTime;

namespace GreenEnergyHub.Charges.IntegrationTest.Core.TestHelpers
{
    public static class EmbeddedResourceHelper
    {
        public static string GetEmbeddedFile(string filePath, Instant currentInstant, IZonedDateTimeService zonedDateTimeService)
        {
            var basePath = Assembly.GetExecutingAssembly().Location;
            var path = Path.Combine(Directory.GetParent(basePath)!.FullName, filePath);
            var fileText = File.ReadAllText(path);
            return ReplaceMergeFields(currentInstant, fileText, zonedDateTimeService);
        }

        private static string ReplaceMergeFields(Instant currentInstant, string file, IZonedDateTimeService zonedDateTimeService)
        {
            var now = currentInstant.ToString();
            var inThirtyoneDays = currentInstant.Plus(Duration.FromDays(31));

            var midnightLocalTime31DaysAhead =
                ConvertCurrentInstantToLocalDateTimeWithDaysAdded(currentInstant, zonedDateTimeService, 31);
            var midnightLocalTime32DaysAhead =
                ConvertCurrentInstantToLocalDateTimeWithDaysAdded(currentInstant, zonedDateTimeService, 32);

            // cim:createdDateTime and effective date must have seconds
            var ymdhmsTimeInterval = currentInstant.GetCreatedDateTimeFormat();

            var chargeIdForMultipleOperations = $"ChgId{Guid.NewGuid().ToString("n")[..5]}";

            var replacementIndex = 0;
            var mergedFile = Regex.Replace(file, "[{][{][$]increment5digits[}][}]", _ =>
            {
                replacementIndex++;
                return replacementIndex.ToString("D5");
            });

            return mergedFile
                .Replace("{{$senderMarketParticipant}}", "8100000000030")
                .Replace("{{$receiverMarketParticipant}}", "5790001330552")
                .Replace("{{$randomCharacters}}", Guid.NewGuid().ToString("n")[..10])
                .Replace("{{$randomCharactersShort}}", Guid.NewGuid().ToString("n")[..5])
                .Replace("{{$chargeIdForMultipleOperations}}", chargeIdForMultipleOperations)
                .Replace("{{$isoTimestamp}}", now)
                .Replace("{{$isoTimestampPlusOneMonth}}", inThirtyoneDays.ToString())
                .Replace("{{$YMDHM_TimestampPlusOneMonth}}", ConvertLocalTimeToUtcAsString(zonedDateTimeService, midnightLocalTime31DaysAhead))
                .Replace("{{$YMDHM_TimestampPlusOneMonthAndOneDay}}", ConvertLocalTimeToUtcAsString(zonedDateTimeService, midnightLocalTime32DaysAhead))
                .Replace("{{$isoTimestampPlusOneMonthAndOneDay}}", inThirtyoneDays.Plus(Duration.FromDays(1)).ToString())
                .Replace("{{$isoTimestampPlusOneMonthAndTwoDays}}", inThirtyoneDays.Plus(Duration.FromDays(2)).ToString())
                .Replace("{{$NextYear}}", DateTime.Now.AddYears(1).Year.ToString())
                .Replace("{{$ISO8601Timestamp}}", ymdhmsTimeInterval);
        }

        private static LocalDateTime ConvertCurrentInstantToLocalDateTimeWithDaysAdded(
            Instant instant,
            IZonedDateTimeService zonedDateTimeService,
            int daysToAdd)
        {
            var zonedTime = zonedDateTimeService.GetZonedDateTime(instant);
            var midnightLocalTime = zonedTime.PlusTicks(-zonedTime.TickOfDay).LocalDateTime;
            return midnightLocalTime.PlusDays(daysToAdd);
        }

        private static string ConvertLocalTimeToUtcAsString(IZonedDateTimeService zonedDateTimeService, LocalDateTime localDateTime)
        {
            var zonedDateTime = zonedDateTimeService.GetZonedDateTime(localDateTime, ResolutionStrategy.Leniently);
            return zonedDateTime.ToInstant().ToString("yyyy-MM-dd\\THH:mm\\Z", CultureInfo.InvariantCulture);
        }
    }
}
