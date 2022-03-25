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

using System.Collections.Generic;
using GreenEnergyHub.Charges.Domain.Charges;
using NodaTime;

namespace GreenEnergyHub.Charges.Domain.Dtos.ChargeCommands
{
    // Non-nullable member is uninitialized is ignored
    // Only properties which is allowed to be null is nullable
    // ChargeEvent integrity is null checked by ChargeCommandNullChecker

    /// <summary>
    /// The ChargeOperationDto class contains the intend of the charge command, e.g. updating an existing charge.
    /// </summary>
    public class ChargeOperationDto
    {
        public ChargeOperationDto(
                string id,
                ChargeType type,
                string chargeId,
                string chargeName,
                string chargeDescription,
                string chargeOwner,
                Resolution resolution,
                bool taxIndicator,
                bool transparentInvoicing,
                VatClassification vatClassification,
                Instant startDateTime,
                int receivedOrder,
                OperationType operationType,
                List<Point> points)
        {
            Points = new List<Point>();
            Id = id;
            Type = type;
            ChargeId = chargeId;
            ChargeName = chargeName;
            ChargeDescription = chargeDescription;
            ChargeOwner = chargeOwner;
            Resolution = resolution;
            TaxIndicator = taxIndicator;
            TransparentInvoicing = transparentInvoicing;
            VatClassification = vatClassification;
            StartDateTime = startDateTime;
            ReceivedOrder = receivedOrder;
            OperationType = operationType;
            Points = points;
        }

        /// <summary>
        /// Contains a unique ID for the specific Charge Operation, provided by the sender.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Unique ID of a charge (Note, unique per market participants).
        /// Example: EA-001
        /// </summary>
        public string ChargeId { get; }

        public ChargeType Type { get; }

        /// <summary>
        /// The charge name
        /// </summary>
        public string ChargeName { get; }

        public string ChargeDescription { get; }

        /// <summary>
        /// Valid from, of a charge price list. Also known as Effective Date.
        /// </summary>
        public Instant StartDateTime { get; }

        public int ReceivedOrder { get; }

        /// <summary>
        /// Type of operation
        /// </summary>
        public OperationType OperationType { get; }

        public VatClassification VatClassification { get; }

        /// <summary>
        /// In Denmark the Energy Supplier invoices the customer, including the charges from the Grid Access Provider and the System Operator.
        /// This boolean can be use to indicate that a charge must be visible on the invoice sent to the customer.
        /// </summary>
        public bool TransparentInvoicing { get; }

        /// <summary>
        /// Indicates whether the Charge is tax or not.
        /// </summary>
        public bool TaxIndicator { get; }

        /// <summary>
        ///  Charge Owner, e.g. the GLN or EIC identification number.
        /// </summary>
        public string ChargeOwner { get; }

        public Resolution Resolution { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227", Justification = "JSON deserialization")]
        public List<Point> Points { get; }
    }
}
