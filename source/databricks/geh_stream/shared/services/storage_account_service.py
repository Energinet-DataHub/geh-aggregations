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


class StorageAccountService():

    def get_storage_account_full_path(storage_base_path: str, delta_table_path: str):
        # return f"abfss://{storage_account_container_name}@{storage_account_name}.dfs.core.windows.net/{path}"
        return f"{storage_base_path}/{delta_table_path}"
