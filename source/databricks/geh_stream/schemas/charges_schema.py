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

from pyspark.sql.types import DecimalType, StructType, StructField, StringType, TimestampType

charges_schema = StructType([
      StructField("charge_key", StringType(), False),
      StructField("charge_type", StringType(), False),
      StructField("charge_owner", StringType(), False),
      StructField("resolution", StringType(), False),
      StructField("charge_tax", StringType(), False),
      StructField("currency", StringType(), False),
      StructField("from_date", TimestampType(), False),
      StructField("to_date", TimestampType(), False),
])


charge_links_schema = StructType([
      StructField("charge_key", StringType(), False),
      StructField("metering_point_id", StringType(), False),
      StructField("from_date", TimestampType(), False),
      StructField("to_date", TimestampType(), False),
])


charge_prices_schema = StructType([
      StructField("charge_key", StringType(), False),
      StructField("charge_price", DecimalType(18, 6), False),
      StructField("time", TimestampType(), False),
])
