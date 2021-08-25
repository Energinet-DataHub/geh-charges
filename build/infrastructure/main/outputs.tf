# Copyright 2020 Energinet DataHub A/S
#
# Licensed under the Apache License, Version 2.0 (the "License2");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
output "sql_connection_string_keyvaultsecretname" {
  description = "Name of the secret in the keyvault containing the username for the charges sql database"
  value = local.CHARGE_DB_CONNECTION_STRING
  sensitive = true
}

output "kv_charges_name" {
  description = "Name of the keyvault"
  value = module.kv_charges.name
  sensitive = true
}