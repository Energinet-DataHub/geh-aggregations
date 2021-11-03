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

import pytest
from geh_stream.event_dispatch.dispatcher_base import period_mutations as method_to_test
from datetime import datetime
from pyspark.sql.types import StructType, StringType, StructField, TimestampType
from pyspark.sql.functions import col, lit, when
from geh_stream.schemas import metering_point_schema

settlement_method_updated_schema = StructType([
    StructField("metering_point_id", StringType(), False),
    StructField("settlement_method", StringType(), False),
    StructField("effective_date", TimestampType(), False),
])

#   metering_point_id
#   metering_point_type
#   settlement_method
#   grid_area
#   connection_state
#   resolution
#   in_grid_area
#   out_grid_area
#   metering_method
#   net_settlement_group
#   parent_metering_point_id
#   unit
#   product
#   from_date
#   to_date
consumption_mps = [
        ("1", "E17", "D01", "ga", "constate", "res", "in", "out", "mm", "netset", "parent", "unit", "prod", datetime(2021, 1, 1, 0, 0), datetime(2021, 1, 7, 0, 0)),
        ("1", "E17", "D02", "ga", "constate", "res", "in", "out", "mm", "netset", "parent", "unit", "prod", datetime(2021, 1, 7, 0, 0), datetime(2021, 1, 9, 0, 0)),
        ("1", "E17", "D03", "ga", "constate", "res", "in", "out", "mm", "netset", "parent", "unit", "prod", datetime(2021, 1, 9, 0, 0), datetime(2021, 1, 12, 0, 0)),
        ("1", "E17", "D04", "ga", "constate", "res", "in", "out", "mm", "netset", "parent", "unit", "prod", datetime(2021, 1, 12, 0, 0), datetime(2021, 1, 17, 0, 0)),
        ("1", "E17", "D05", "ga", "constate", "res", "in", "out", "mm", "netset", "parent", "unit", "prod", datetime(2021, 1, 17, 0, 0), datetime(9999, 1, 1, 0, 0))]

def test_changed_period_after_update(spark):
   
    consumption_mps_df = spark.createDataFrame(consumption_mps, schema=metering_point_schema)

    settlement_method_updated_event = [("1", "D06", datetime(2021, 1, 7, 0, 0))]
    settlement_method_updated_df = spark.createDataFrame(settlement_method_updated_event, schema=settlement_method_updated_schema)

    result_df = method_to_test(spark, consumption_mps_df, settlement_method_updated_df, ["settlement_method"]).orderBy("to_date")

    assert(consumption_mps_df.count() == 5)
    assert(result_df.count() == 5)

    assert(result_df.collect()[0]["from_date"] == datetime(2021, 1, 1, 0, 0))
    assert(result_df.collect()[0]["to_date"] == datetime(2021, 1, 7, 0, 0))

    assert(result_df.collect()[1]["from_date"] == datetime(2021, 1, 7, 0, 0))
    assert(result_df.collect()[1]["to_date"] == datetime(2021, 1, 9, 0, 0))

    assert(result_df.collect()[2]["from_date"] == datetime(2021, 1, 9, 0, 0))
    assert(result_df.collect()[2]["to_date"] == datetime(2021, 1, 12, 0, 0))

    assert(result_df.collect()[3]["from_date"] == datetime(2021, 1, 12, 0, 0))
    assert(result_df.collect()[3]["to_date"] == datetime(2021, 1, 17, 0, 0))

    assert(result_df.collect()[4]["from_date"] == datetime(2021, 1, 17, 0, 0))
    assert(result_df.collect()[4]["to_date"] == datetime(9999, 1, 1, 0, 0))

    assert(result_df.collect()[0]["settlement_method"] == "D01") # 1/1
    assert(result_df.collect()[1]["settlement_method"] == "D06") # 7/1
    assert(result_df.collect()[2]["settlement_method"] == "D03") # 9/1
    assert(result_df.collect()[3]["settlement_method"] == "D04") # 12/1
    assert(result_df.collect()[4]["settlement_method"] == "D05") # 17/1


def test_add_new_period_after_update(spark):
  
    consumption_mps_df = spark.createDataFrame(consumption_mps, schema=metering_point_schema)

    settlement_method_updated_event = [("1", "D06", datetime(2021, 1, 8, 0, 0))]
    settlement_method_updated_df = spark.createDataFrame(settlement_method_updated_event, schema=settlement_method_updated_schema)

    result_df = method_to_test(spark, consumption_mps_df, settlement_method_updated_df, ["settlement_method"]).orderBy("to_date")

    assert(consumption_mps_df.count() == 5)
    assert(result_df.count() == 6)

    assert(result_df.collect()[0]["from_date"] == datetime(2021, 1, 1, 0, 0))
    assert(result_df.collect()[0]["to_date"] == datetime(2021, 1, 7, 0, 0))

    assert(result_df.collect()[1]["from_date"] == datetime(2021, 1, 7, 0, 0))
    assert(result_df.collect()[1]["to_date"] == datetime(2021, 1, 8, 0, 0))

    assert(result_df.collect()[2]["from_date"] == datetime(2021, 1, 8, 0, 0))
    assert(result_df.collect()[2]["to_date"] == datetime(2021, 1, 9, 0, 0))

    assert(result_df.collect()[3]["from_date"] == datetime(2021, 1, 9, 0, 0))
    assert(result_df.collect()[3]["to_date"] == datetime(2021, 1, 12, 0, 0))

    assert(result_df.collect()[0]["settlement_method"] == "D01") # 1/1
    assert(result_df.collect()[1]["settlement_method"] == "D02") # 7/1
    assert(result_df.collect()[2]["settlement_method"] == "D06") # 8/1
    assert(result_df.collect()[3]["settlement_method"] == "D03") # 9/1
    assert(result_df.collect()[4]["settlement_method"] == "D04") # 12/1
    assert(result_df.collect()[5]["settlement_method"] == "D05") # 17/1

def test_add_new_future_period_after_update(spark):
  
    consumption_mps_df = spark.createDataFrame(consumption_mps, schema=metering_point_schema)

    settlement_method_updated_event = [("1", "D06", datetime(2021, 1, 18, 0, 0))]
    settlement_method_updated_df = spark.createDataFrame(settlement_method_updated_event, schema=settlement_method_updated_schema)

    result_df = method_to_test(spark, consumption_mps_df, settlement_method_updated_df, ["settlement_method"]).orderBy("to_date")

    assert(consumption_mps_df.count() == 5)
    assert(result_df.count() == 6)

    assert(result_df.collect()[0]["from_date"] == datetime(2021, 1, 1, 0, 0))
    assert(result_df.collect()[0]["to_date"] == datetime(2021, 1, 7, 0, 0))

    assert(result_df.collect()[1]["from_date"] == datetime(2021, 1, 7, 0, 0))
    assert(result_df.collect()[1]["to_date"] == datetime(2021, 1, 9, 0, 0))

    assert(result_df.collect()[2]["from_date"] == datetime(2021, 1, 9, 0, 0))
    assert(result_df.collect()[2]["to_date"] == datetime(2021, 1, 12, 0, 0))

    assert(result_df.collect()[3]["from_date"] == datetime(2021, 1, 12, 0, 0))
    assert(result_df.collect()[3]["to_date"] == datetime(2021, 1, 17, 0, 0))

    assert(result_df.collect()[4]["from_date"] == datetime(2021, 1, 17, 0, 0))
    assert(result_df.collect()[4]["to_date"] == datetime(2021, 1, 18, 0, 0))

    assert(result_df.collect()[5]["from_date"] == datetime(2021, 1, 18, 0, 0))
    assert(result_df.collect()[5]["to_date"] == datetime(9999, 1, 1, 0, 0))

    assert(result_df.collect()[0]["settlement_method"] == "D01") # 1/1
    assert(result_df.collect()[1]["settlement_method"] == "D02") # 7/1
    assert(result_df.collect()[2]["settlement_method"] == "D03") # 9/1
    assert(result_df.collect()[3]["settlement_method"] == "D04") # 12/1
    assert(result_df.collect()[4]["settlement_method"] == "D05") # 17/1
    assert(result_df.collect()[5]["settlement_method"] == "D06") # 18/1

settlement_method_and_connect_updated_schema = StructType([
    StructField("metering_point_id", StringType(), False),
    StructField("settlement_method", StringType(), False),
    StructField("connection_state", StringType(), False),
    StructField("effective_date", TimestampType(), False),
])

#Lets test that we also are able to set multple properties in an update
def test_multiple_properties_updated_after_update(spark):

    consumption_mps_df = spark.createDataFrame(consumption_mps, schema=metering_point_schema)

    settlement_method_and_connected_updated_event = [("1", "D06", "True", datetime(2021, 1, 8, 0, 0))]
    event_df = spark.createDataFrame(settlement_method_and_connected_updated_event, schema=settlement_method_and_connect_updated_schema)

    result_df = method_to_test(spark, consumption_mps_df, event_df, ["settlement_method","connection_state"]).orderBy("to_date")

    assert(consumption_mps_df.count() == 5)
    assert(result_df.count() == 6)

    assert(result_df.collect()[0]["from_date"] == datetime(2021, 1, 1, 0, 0))
    assert(result_df.collect()[0]["to_date"] == datetime(2021, 1, 7, 0, 0))

    assert(result_df.collect()[1]["from_date"] == datetime(2021, 1, 7, 0, 0))
    assert(result_df.collect()[1]["to_date"] == datetime(2021, 1, 8, 0, 0))

    assert(result_df.collect()[2]["from_date"] == datetime(2021, 1, 8, 0, 0))
    assert(result_df.collect()[2]["to_date"] == datetime(2021, 1, 9, 0, 0))

    assert(result_df.collect()[3]["from_date"] == datetime(2021, 1, 9, 0, 0))
    assert(result_df.collect()[3]["to_date"] == datetime(2021, 1, 12, 0, 0))

    assert(result_df.collect()[0]["settlement_method"] == "D01") # 1/1
    assert(result_df.collect()[1]["settlement_method"] == "D02") # 7/1
    assert(result_df.collect()[2]["settlement_method"] == "D06") # 8/1
    assert(result_df.collect()[3]["settlement_method"] == "D03") # 9/1
    assert(result_df.collect()[4]["settlement_method"] == "D04") # 12/1
    assert(result_df.collect()[5]["settlement_method"] == "D05") # 17/1

    assert(result_df.collect()[0]["connection_state"] == "constate") # 1/1
    assert(result_df.collect()[1]["connection_state"] == "constate") # 7/1
    assert(result_df.collect()[2]["connection_state"] == "True") # 8/1
    assert(result_df.collect()[3]["connection_state"] == "constate") # 9/1
    assert(result_df.collect()[4]["connection_state"] == "constate") # 12/1
    assert(result_df.collect()[5]["connection_state"] == "constate") # 17/1

def test_multiple_properties_updated_in_exsisting_period(spark):

    consumption_mps_df = spark.createDataFrame(consumption_mps, schema=metering_point_schema)

    settlement_method_and_connected_updated_event = [("1", "D06", "True", datetime(2021, 1, 7, 0, 0))]
    event_df = spark.createDataFrame(settlement_method_and_connected_updated_event, schema=settlement_method_and_connect_updated_schema)

    result_df = method_to_test(spark, consumption_mps_df, event_df, ["settlement_method","connection_state"]).orderBy("to_date")

    assert(consumption_mps_df.count() == 5)
    assert(result_df.count() == 5)

    assert(result_df.collect()[0]["from_date"] == datetime(2021, 1, 1, 0, 0))
    assert(result_df.collect()[0]["to_date"] == datetime(2021, 1, 7, 0, 0))

    assert(result_df.collect()[1]["from_date"] == datetime(2021, 1, 7, 0, 0))
    assert(result_df.collect()[1]["to_date"] == datetime(2021, 1, 9, 0, 0))

    assert(result_df.collect()[2]["from_date"] == datetime(2021, 1, 9, 0, 0))
    assert(result_df.collect()[2]["to_date"] == datetime(2021, 1, 12, 0, 0))

    assert(result_df.collect()[3]["from_date"] == datetime(2021, 1, 12, 0, 0))
    assert(result_df.collect()[3]["to_date"] == datetime(2021, 1, 17, 0, 0))

    assert(result_df.collect()[4]["from_date"] == datetime(2021, 1, 17, 0, 0))
    assert(result_df.collect()[4]["to_date"] == datetime(9999, 1, 1, 0, 0))

    assert(result_df.collect()[0]["settlement_method"] == "D01") # 1/1
    assert(result_df.collect()[1]["settlement_method"] == "D06") # 7/1
    assert(result_df.collect()[2]["settlement_method"] == "D03") # 9/1
    assert(result_df.collect()[3]["settlement_method"] == "D04") # 12/1
    assert(result_df.collect()[4]["settlement_method"] == "D05") # 17/1

    assert(result_df.collect()[0]["connection_state"] == "constate") # 1/1
    assert(result_df.collect()[1]["connection_state"] == "True") # 7/1
    assert(result_df.collect()[2]["connection_state"] == "constate") # 9/1
    assert(result_df.collect()[3]["connection_state"] == "constate") # 12/1
    assert(result_df.collect()[4]["connection_state"] == "constate") # 17/1
