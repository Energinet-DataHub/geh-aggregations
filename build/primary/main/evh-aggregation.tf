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

module "evhnm_aggregations" {
  source                          = "git::https://github.com/Energinet-DataHub/geh-terraform-modules.git//azure/eventhub-namespace?ref=6.0.0"

  name                            = "aggregations"
  project_name                    = var.domain_name_short
  environment_short               = var.environment_short
  environment_instance            = var.environment_instance
  resource_group_name             = azurerm_resource_group.this.name
  location                        = azurerm_resource_group.this.location
  sku                             = "Standard"
  capacity                        = 1
  private_endpoint_subnet_id      = data.azurerm_key_vault_secret.snet_private_endoints_id.value
  private_dns_resource_group_name = data.azurerm_key_vault_secret.pdns_resource_group_name.value

  tags                            = azurerm_resource_group.this.tags
}

module "evh_aggregations" {
  source                    = "git::https://github.com/Energinet-DataHub/geh-terraform-modules.git//azure/eventhub?ref=5.1.0"

  name                      = "aggregations"
  namespace_name            = module.evhnm_aggregations.name
  resource_group_name       = azurerm_resource_group.this.name
  partition_count           = 4
  message_retention         = 1
  auth_rules            = [
    {
      name    = "listen",
      listen  = true
    },
    {
      name    = "send",
      send    = true
    },
  ]
}

module "kvs_evh_aggregations_listen_key" {
  source        = "git::https://github.com/Energinet-DataHub/geh-terraform-modules.git//azure/key-vault-secret?ref=5.1.0"

  name          = "evh-aggregations-listen-connection-string"
  value         = module.evh_aggregations.primary_connection_strings["listen"]
  key_vault_id  = module.kv_aggregations.id

  tags          = azurerm_resource_group.this.tags
}