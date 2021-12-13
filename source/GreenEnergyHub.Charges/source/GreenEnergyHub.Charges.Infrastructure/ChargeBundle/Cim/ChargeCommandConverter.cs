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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using Energinet.DataHub.Core.Messaging.Transport;
using GreenEnergyHub.Charges.Domain.Charges;
using GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands;
using GreenEnergyHub.Charges.Domain.Dtos.SharedDtos;
using GreenEnergyHub.Charges.Infrastructure.ChargeLinkBundle.Cim;
using GreenEnergyHub.Charges.Infrastructure.MarketDocument.Cim;
using GreenEnergyHub.Charges.Infrastructure.Messaging.Serialization;
using GreenEnergyHub.Iso8601;
using NodaTime;

namespace GreenEnergyHub.Charges.Infrastructure.ChargeBundle.Cim
{
    public class ChargeCommandConverter : DocumentConverter
    {
        private readonly IIso8601Durations _iso8601Durations;

        public ChargeCommandConverter(
            IClock clock,
            IIso8601Durations iso8601Durations)
            : base(clock)
        {
            _iso8601Durations = iso8601Durations;
        }

        protected override async Task<IInboundMessage> ConvertSpecializedContentAsync(
            XmlReader reader,
            DocumentDto document)
        {
            return new ChargeCommand
                {
                    Document = document,
                    ChargeOperation = await ParseChargeOperationAsync(reader).ConfigureAwait(false),
                };
        }

        private async Task<ChargeOperationDto> ParseChargeOperationAsync(XmlReader reader)
        {
            ChargeOperationDto? operation = null;
            var operationId = string.Empty;

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.Is(CimChargeCommandConstants.Id, CimChargeCommandConstants.Namespace))
                {
                    var content = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                    operationId = content;
                }
                else if (reader.Is(CimChargeCommandConstants.ChargeGroup, CimChargeCommandConstants.Namespace))
                {
                    operation = await ParseChargeGroupIntoOperationAsync(reader, operationId).ConfigureAwait(false);
                }
            }

            if (operation == null)
                throw new Exception();

            return operation;
        }

        private async Task<ChargeOperationDto> ParseChargeGroupIntoOperationAsync(XmlReader reader, string operationId)
        {
            ChargeOperationDto? operation = null;
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.Is(CimChargeCommandConstants.ChargeTypeElement, CimChargeCommandConstants.Namespace))
                {
                    operation = await ParseChargeTypeElementIntoOperationAsync(reader, operationId).ConfigureAwait(false);
                }
                else if (reader.Is(
                    CimChargeCommandConstants.ChargeGroup,
                    CimChargeCommandConstants.Namespace,
                    XmlNodeType.EndElement))
                {
                    break;
                }
            }

            return operation!;
        }

        private async Task<ChargeOperationDto> ParseChargeTypeElementIntoOperationAsync(XmlReader reader, string operationId)
        {
            var chargeOwner = string.Empty;
            var chargeType = ChargeType.Unknown;
            var senderProvidedChargeId = string.Empty;
            var chargeName = string.Empty;
            var description = string.Empty;
            var resolution = Resolution.Unknown;
            Instant startDateTime = default;
            Instant? endDateTime = null;
            var vatClassification = VatClassification.Unknown;
            var transparentInvoicing = false;
            var taxIndicator = false;
            var points = new List<Point>();

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.Is(CimChargeCommandConstants.ChargeOwner, CimChargeCommandConstants.Namespace))
                {
                    var content = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                    chargeOwner = content;
                }
                else if (reader.Is(CimChargeCommandConstants.ChargeType, CimChargeCommandConstants.Namespace))
                {
                    var content = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                    chargeType = ChargeTypeMapper.Map(content);
                }
                else if (reader.Is(CimChargeCommandConstants.ChargeId, CimChargeCommandConstants.Namespace))
                {
                    var content = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                    senderProvidedChargeId = content;
                }
                else if (reader.Is(CimChargeCommandConstants.ChargeName, CimChargeCommandConstants.Namespace))
                {
                    var content = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                    chargeName = content;
                }
                else if (reader.Is(CimChargeCommandConstants.ChargeDescription, CimChargeCommandConstants.Namespace))
                {
                    var content = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                    description = content;
                }
                else if (reader.Is(CimChargeCommandConstants.Resolution, CimChargeCommandConstants.Namespace))
                {
                    // Note: Resolution can be set two places in the file. If its filled here, that the one that will be used.
                    // This is done to be able to handle changes to charges without prices
                    var content = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                    resolution = ResolutionMapper.Map(content);
                }
                else if (reader.Is(CimChargeCommandConstants.StartDateTime, CimChargeCommandConstants.Namespace))
                {
                    startDateTime = Instant.FromDateTimeUtc(reader.ReadElementContentAsDateTime());
                }
                else if (reader.Is(CimChargeCommandConstants.EndDateTime, CimChargeCommandConstants.Namespace))
                {
                    endDateTime = Instant.FromDateTimeUtc(reader.ReadElementContentAsDateTime());
                }
                else if (reader.Is(CimChargeCommandConstants.VatClassification, CimChargeCommandConstants.Namespace))
                {
                    var content = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                    vatClassification = VatClassificationMapper.Map(content);
                }
                else if (reader.Is(CimChargeCommandConstants.TransparentInvoicing, CimChargeCommandConstants.Namespace))
                {
                    transparentInvoicing = reader.ReadElementContentAsBoolean();
                }
                else if (reader.Is(CimChargeCommandConstants.TaxIndicator, CimChargeCommandConstants.Namespace))
                {
                    taxIndicator = reader.ReadElementContentAsBoolean();
                }
                else if (reader.Is(CimChargeCommandConstants.SeriesPeriod, CimChargeCommandConstants.Namespace))
                {
                    var seriesPeriodIntoOperationAsync = await ParseSeriesPeriodIntoOperationAsync(reader, startDateTime, resolution).ConfigureAwait(false);
                    points.AddRange(seriesPeriodIntoOperationAsync.Points);
                    resolution = seriesPeriodIntoOperationAsync.Resolution;
                }
                else if (reader.Is(
                    CimChargeCommandConstants.ChargeTypeElement,
                    CimChargeCommandConstants.Namespace,
                    XmlNodeType.EndElement))
                {
                    break;
                }
            }

            return new ChargeOperationDto(
                operationId,
                chargeType,
                senderProvidedChargeId,
                chargeName,
                description,
                chargeOwner,
                resolution,
                taxIndicator,
                transparentInvoicing,
                vatClassification,
                startDateTime,
                endDateTime,
                points);
        }

        private async Task<(List<Point> Points, Resolution Resolution)> ParseSeriesPeriodIntoOperationAsync(XmlReader reader, Instant startDateTime, Resolution initialResolution)
        {
            var points = new List<Point>();
            var resolution = initialResolution;

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.Is(CimChargeCommandConstants.PeriodResolution, CimChargeCommandConstants.Namespace))
                {
                    // Note, this is the second place where the resolution might be identified
                    // If it was not set previous, we use this one instead
                    if (initialResolution == Resolution.Unknown)
                    {
                        var content = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                        resolution = ResolutionMapper.Map(content); // TODO
                    }
                }
                else if (reader.Is(CimChargeCommandConstants.TimeInterval, CimChargeCommandConstants.Namespace))
                {
                    startDateTime = await ParseTimeIntervalAsync(reader, startDateTime).ConfigureAwait(false);
                }
                else if (reader.Is(CimChargeCommandConstants.Point, CimChargeCommandConstants.Namespace))
                {
                    var point = await ParsePointAsync(reader, resolution, startDateTime).ConfigureAwait(false);
                    points.Add(point);
                }
                else if (reader.Is(
                    CimChargeCommandConstants.SeriesPeriod,
                    CimChargeCommandConstants.Namespace,
                    XmlNodeType.EndElement))
                {
                    break;
                }
            }

            return (points, resolution);
        }

        private async Task<Instant> ParseTimeIntervalAsync(XmlReader reader, Instant startDateTime)
        {
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.Is(CimChargeCommandConstants.TimeIntervalStart, CimChargeCommandConstants.Namespace))
                {
                    return Instant.FromDateTimeUtc(reader.ReadElementContentAsDateTime());
                }
                else if (reader.Is(
                    CimChargeCommandConstants.TimeInterval,
                    CimChargeCommandConstants.Namespace,
                    XmlNodeType.EndElement))
                {
                    break;
                }
            }

            return startDateTime;
        }

        private async Task<Point> ParsePointAsync(XmlReader reader, Resolution resolution, Instant startDateTime)
        {
            int position = 0;
            decimal price = 0m;
            Instant time = default;

            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                if (reader.Is(CimChargeCommandConstants.Position, CimChargeCommandConstants.Namespace))
                {
                    var content = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                    position = int.Parse(content, CultureInfo.InvariantCulture);
                }
                else if (reader.Is(CimChargeCommandConstants.Price, CimChargeCommandConstants.Namespace))
                {
                    var content = await reader.ReadElementContentAsStringAsync().ConfigureAwait(false);
                    price = decimal.Parse(content, CultureInfo.InvariantCulture);
                }
                else if (reader.Is(
                    CimChargeCommandConstants.Point,
                    CimChargeCommandConstants.Namespace,
                    XmlNodeType.EndElement))
                {
                    time = _iso8601Durations.GetTimeFixedToDuration(
                        startDateTime,
                        ResolutionMapper.Map(resolution),
                        position - 1);
                    break;
                }
            }

            return new Point(position, price, time);
        }
    }
}
