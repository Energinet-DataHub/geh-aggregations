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

locals {
  metadataDatabaseAdminName = "metadataadmin"
}

data "azurerm_sql_server" "sqlsrv" {
  name                = "sqlsrv-sharedres-${var.organisation}-${var.environment}"
  resource_group_name = var.sharedresources_resource_group_name
}

module "sqldb_metadata" {
  source              = "Azure/database/azurerm"
  name                = "sqldb-aggregation-metadata"
  resource_group_name = var.sharedresources_resource_group_name
  server_name         = data.azurerm_sql_server.sqlsrv.name
  db_name             = "sqldb-aggregation-metadata"
  sql_admin_username  = local.metadataDatabaseAdminName
  sql_password        = var.database_password
}

module "kvs_db_admin_name" {
  source        = "git::https://github.com/Energinet-DataHub/geh-terraform-modules.git//key-vault-secret?ref=1.2.0"
  name          = "AGGREGATION-METADATA-DB-ADMIN-NAME"
  value         = local.metadataDatabaseAdminName
  key_vault_id  = data.azurerm_key_vault.kv_sharedresources.id
}

module "kvs_db_admin_password" {
  source        = "git::https://github.com/Energinet-DataHub/geh-terraform-modules.git//key-vault-secret?ref=1.2.0"
  name          = "AGGREGATION-DB-ADMIN-PASSWORD"
  value         = var.database_password
  key_vault_id  = data.azurerm_key_vault.kv_sharedresources.id
}