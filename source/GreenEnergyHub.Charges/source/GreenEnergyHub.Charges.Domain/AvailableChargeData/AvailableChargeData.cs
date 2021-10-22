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
using GreenEnergyHub.Charges.Domain.Charges;
using NodaTime;

namespace GreenEnergyHub.Charges.Domain.AvailableChargeData
{
    public class AvailableChargeData
    {
        public AvailableChargeData(
            string chargeOwner,
            ChargeType chargeType,
            Instant startDateTime,
            Instant endDateTime,
            VatClassification vatClassification,
            bool taxIndicator,
            bool transparentInvoicing,
            Resolution resolution,
            List<AvailableChargeDataPoint> points,
            Instant requestTime,
            Guid availableDataReferenceId)
        {
            Id = Guid.NewGuid();
            ChargeOwner = chargeOwner;
            ChargeType = chargeType;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            VatClassification = vatClassification;
            TaxIndicator = taxIndicator;
            TransparentInvoicing = transparentInvoicing;
            Resolution = resolution;
            _points = points;
            RequestTime = requestTime;
            AvailableDataReferenceId = availableDataReferenceId;
        }

        /// <summary>
        /// Used implicitly by persistence.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private AvailableChargeData(string chargeOwner)
        {
            _points = new List<AvailableChargeDataPoint>();
            ChargeOwner = chargeOwner;
        }

        public Guid Id { get; }

        public string ChargeOwner { get; }

        public ChargeType ChargeType { get; }

        public Instant StartDateTime { get; }

        public Instant EndDateTime { get; }

        public VatClassification VatClassification { get; }

        public bool TaxIndicator { get; }

        public bool TransparentInvoicing { get; }

        public Resolution Resolution { get; }

        private readonly List<AvailableChargeDataPoint> _points;

        public IReadOnlyCollection<AvailableChargeDataPoint> Points => _points.AsReadOnly();

        public Instant RequestTime { get; }

        public Guid AvailableDataReferenceId { get; }
    }
}
