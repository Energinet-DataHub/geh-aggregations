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

import os
import pyodbc
import pytest
import subprocess


@pytest.fixture(scope="session")
def databricks_path() -> str:
    """
    Returns the source/databricks folder path.
    Please note that this only works if current folder haven't been changed prior using us.chdir().
    The correctness also relies on the prerequisite that this function is actually located in a
    file located directly in the integration tests folder.
    """
    return os.path.dirname(os.path.realpath(__file__)) + "/../.."


@pytest.fixture(scope="session")
def source_path() -> str:
    """
    Returns the source/databricks folder path.
    Please note that this only works if current folder haven't been changed prior using us.chdir().
    The correctness also relies on the prerequisite that this function is actually located in a
    file located directly in the integration tests folder.
    """
    return os.path.dirname(os.path.realpath(__file__)) + "/../../.."


sql_server = "sql-server"
sa_user_id = "sa"
sa_user_password = "P@ssword123"
master_data_database_name = "master-data"
master_data_connection_string = f"Server={sql_server};Database={master_data_database_name};User Id={sa_user_id};Password={sa_user_password};Trusted_Connection=False;TrustServerCertificate=True;Encrypt=False;"


@pytest.fixture(scope="session")
def master_data_db_info():
    return {
        "server_name": sql_server,
        "database_name": master_data_database_name,
        "sa_user_id": sa_user_id,
        "sa_user_password": sa_user_password
    }


@pytest.fixture(scope="session")
def master_data_database(source_path):
    # Create database if not exists
    # conn = pyodbc.connect('DRIVER={/opt/microsoft/msodbcsql18/lib64/libmsodbcsql-18.0.so.1.1};'
    conn = pyodbc.connect('DRIVER={ODBC Driver 18 for SQL Server};'
                          f'Server={sql_server};'
                          'Database=master;'
                          'TrustServerCertificate=yes;'
                          f'UID={sa_user_id};'
                          f'PWD={sa_user_password}',
                          autocommit=True)
    cursor = conn.cursor()
    cursor.execute(f"""
--IF EXISTS(SELECT name FROM sys.databases WHERE name = '{master_data_database_name}')
DROP DATABASE [{master_data_database_name}]
""")
    import time
    # time.sleep(5)
    cursor.execute(f"""CREATE DATABASE [{master_data_database_name}]""")
    # time.sleep(30)

    # Build db migration program
    subprocess.check_call([
        "dotnet",
        "build",
        # f"{source_path}/IntegrationEventListener/Energinet.DataHub.Aggregations.DatabaseMigration/Energinet.DataHub.Aggregations.DatabaseMigration.csproj"
        "/workspaces/geh-aggregations/source/databricks/tests/integration/../../../IntegrationEventListener/Energinet.DataHub.Aggregations.DatabaseMigration/Energinet.DataHub.Aggregations.DatabaseMigration.csproj"
    ])

    # Run db migrations
    subprocess.run([
        "/bin/dotnet",
        # f"{source_path}/IntegrationEventListener/Energinet.DataHub.Aggregations.DatabaseMigration/bin/Debug/Energinet.DataHub.Aggregations.DatabaseMigration.dll",
        "/workspaces/geh-aggregations/source/IntegrationEventListener/Energinet.DataHub.Aggregations.DatabaseMigration/bin/Debug/net5.0/Energinet.DataHub.Aggregations.DatabaseMigration.dll",
        master_data_connection_string
    ])
