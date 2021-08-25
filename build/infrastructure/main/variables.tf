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
variable "resource_group_name" {
  type = string
}

variable "environment" {
  type          = string
  description   = "Short for environment name. Helps ensuring global uniqueness of resource names"
}

variable "project" {
  type          = string
  description   = "Project that is running the infrastructure code"
}

variable "organisation" {
  type          = string
  description   = "Organisation that is running the infrastructure code"
}

variable "tenant_id" {
  type          = string
  description   = "tenant id"
}

variable "spn_object_id" {
  type          = string
  description   = "spn_object_id"
}

variable "sharedresources_keyvault_name" {
  type          = string
  description   = "Name of key vault for shared secrets"
}

variable "sharedresources_resource_group_name" {
  type          = string
  description   = "Resource group containing shared resources"
}

variable "sharedresources_sql_server_name" {
  type          = string
  description   = "Sql server shared resources"
}

variable "notification_email" {
  type          = string
  description   = "Email address to send notifications to"
}
