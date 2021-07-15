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


# step 11
def adjust_production(hourly_production_result_df: DataFrame, added_grid_loss_result_df: DataFrame, sys_cor_df: DataFrame):

    # select columns from dataframe that contains information about metering points registered as SystemCorrection to use in join.
    sc_df = sys_cor_df.selectExpr(
        Names.from_date.value,
        Names.to_date.value,
        "{0} as SysCor_EnergySupplier".format(Names.energy_supplier_id.value),
        "{0} as SysCor_GridArea".format(Names.grid_area.value),
        Names.is_system_correction.value
    )

    # join result dataframes from previous steps on time window and grid area.
    df = hourly_production_result_df.join(
        added_grid_loss_result_df, [Names.time_window.value, Names.grid_area.value], "inner")

    # join information from system correction dataframe on to joined result dataframe with information about which energy supplier,
    # that is responsible for system correction in the given time window from the joined result dataframe.
    df = df.join(
        sc_df,
        when(col(Names.to_date.value).isNotNull(), col("{0}.start".format(Names.time_window.value)) <= col(Names.to_date.value)).otherwise(True)
        & (col("{0}.start".format(Names.time_window.value)) >= col(Names.from_date.value))
        & (col(Names.to_date.value).isNull() | (col("{0}.end".format(Names.time_window.value)) <= col(Names.to_date.value)))
        & (col(Names.grid_area.value) == col("SysCor_GridArea"))
        & (col(Names.is_system_correction.value)),
        "left")

    # update function that selects the sum of two columns if condition is met, or selects data from a single column if condition is not met.
    update_func = (when(col(Names.energy_supplier_id.value) == col("SysCor_EnergySupplier"),
                        col(Names.sum_quantity.value) + col(Names.added_system_correction.value))
                   .otherwise(col(Names.sum_quantity.value)))

    result_df = df.withColumn("adjusted_sum_quantity", update_func) \
        .drop(Names.sum_quantity.value) \
        .withColumnRenamed("adjusted_sum_quantity", Names.sum_quantity.value)

    return result_df.select(
        Names.grid_area.value,
        Names.balance_responsible_id.value,
        Names.energy_supplier_id.value,
        Names.time_window.value,
        Names.sum_quantity.value,
        Names.aggregated_quality.value) \
        .orderBy(
            Names.grid_area.value,
            Names.balance_responsible_id.value,
            Names.energy_supplier_id.value,
            Names.time_window.value)
