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
variable "databricks_id" {
    type = string
}

variable "wheel_file" {
    type = string
}

variable "aggregation_evh_listening_key" {
    type = string
}

variable "aggregation_storage_account_key" {
    type = string
}

variable "aggregation_storage_account_name" {
    type = string
}

variable "delta_lake_container_name" {
    type = string
}

variable "events_data_blob_name" {
    type = string
}

variable "master_data_blob_name" {
    type = string
}

