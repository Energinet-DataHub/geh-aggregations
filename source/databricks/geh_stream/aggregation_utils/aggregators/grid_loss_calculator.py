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

from geh_stream.codelists import Names
from pyspark.sql import DataFrame
from pyspark.sql.functions import col, when
from .aggregate_quality import aggregate_total_consumption_quality


production_sum_quantity = "production_sum_quantity"
exchange_sum_quantity = "exchange_sum_quantity"
aggregated_production_quality = "aggregated_production_quality"
aggregated_net_exchange_quality = "aggregated_net_exchange_quality"
hourly_result = "hourly_result"
flex_result = "flex_result"
prod_result = "prod_result"


# Function used to calculate grid loss (step 6)
def calculate_grid_loss(agg_net_exchange: DataFrame, agg_hourly_consumption: DataFrame, agg_flex_consumption: DataFrame, agg_production: DataFrame):
    agg_net_exchange_result = agg_net_exchange.selectExpr(Names.grid_area.value, "{0} as net_exchange_result".format(Names.sum_quantity.value), Names.time_window.value)
    agg_hourly_consumption_result = agg_hourly_consumption \
        .selectExpr(Names.grid_area.value, "{0} as {1}".format(Names.sum_quantity.value, hourly_result), Names.time_window.value) \
        .groupBy(Names.grid_area.value, Names.time_window.value) \
        .sum(hourly_result) \
        .withColumnRenamed("sum({0})".format(hourly_result), hourly_result)
    agg_flex_consumption_result = agg_flex_consumption \
        .selectExpr(Names.grid_area.value, "{0} as {1}".format(Names.sum_quantity.value, flex_result), Names.time_window.value) \
        .groupBy(Names.grid_area.value, Names.time_window.value) \
        .sum(flex_result) \
        .withColumnRenamed("sum({})".format(flex_result), flex_result)
    agg_production_result = agg_production \
        .selectExpr(Names.grid_area.value, "{0} as {1}".format(Names.sum_quantity.value, prod_result), Names.time_window.value) \
        .groupBy(Names.grid_area.value, Names.time_window.value) \
        .sum(prod_result) \
        .withColumnRenamed("sum({0})".format(prod_result), prod_result)

    result = agg_net_exchange_result \
        .join(agg_production_result, [Names.grid_area.value, Names.time_window.value]) \
        .join(agg_hourly_consumption_result.join(agg_flex_consumption_result, [Names.grid_area.value, Names.time_window.value]), [Names.grid_area.value, Names.time_window.value]) \
        .orderBy(Names.grid_area.value, Names.time_window.value)
    result = result\
        .withColumn(Names.grid_loss.value, result.net_exchange_result + result.prod_result - (result.hourly_result + result.flex_result))
    # Quality is always calculated for grid loss entries
    return result.select(Names.grid_area.value, Names.time_window.value, Names.grid_loss.value)


# Function to calculate system correction to be added (step 8)
def calculate_added_system_correction(df: DataFrame):
    result = df.withColumn(Names.added_system_correction.value, when(col(Names.grid_loss.value) < 0, (col(Names.grid_loss.value)) * (-1)).otherwise(0))
    return result.select(Names.grid_area.value, Names.time_window.value, Names.added_system_correction.value)


# Function to calculate grid loss to be added (step 9)
def calculate_added_grid_loss(df: DataFrame):
    result = df.withColumn(Names.added_grid_loss.value, when(col(Names.grid_loss.value) > 0, col(Names.grid_loss.value)).otherwise(0))
    return result.select(Names.grid_area.value, Names.time_window.value, Names.added_grid_loss.value)


# Function to calculate total consumption (step 21)
def calculate_total_consumption(agg_net_exchange: DataFrame, agg_production: DataFrame):

    result_production = agg_production.selectExpr(Names.grid_area.value, Names.time_window.value, Names.sum_quantity.value, Names.aggregated_quality.value) \
        .groupBy(Names.grid_area.value, Names.time_window.value, Names.aggregated_quality.value).sum(Names.sum_quantity.value) \
        .withColumnRenamed("sum({0})".format(Names.sum_quantity.value), production_sum_quantity) \
        .withColumnRenamed(Names.aggregated_quality.value, aggregated_production_quality)

    result_net_exchange = agg_net_exchange.selectExpr(Names.grid_area.value, Names.time_window.value, Names.sum_quantity.value, Names.aggregated_quality.value) \
        .groupBy(Names.grid_area.value, Names.time_window.value, Names.aggregated_quality.value).sum(Names.sum_quantity.value) \
        .withColumnRenamed("sum({0})".format(Names.sum_quantity.value), exchange_sum_quantity) \
        .withColumnRenamed(Names.aggregated_quality.value, aggregated_net_exchange_quality)

    result = result_production.join(result_net_exchange, [Names.grid_area.value, Names.time_window.value]) \
        .withColumn(Names.sum_quantity.value, col(production_sum_quantity) + col(exchange_sum_quantity))

    result = aggregate_total_consumption_quality(result).orderBy(Names.grid_area.value, Names.time_window.value)

    result = result.select(Names.grid_area.value, Names.time_window.value, Names.aggregated_quality.value, Names.sum_quantity.value)
    return result
