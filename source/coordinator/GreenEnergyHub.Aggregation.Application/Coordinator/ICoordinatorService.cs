﻿// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Threading;
using System.Threading.Tasks;
using GreenEnergyHub.Aggregation.Domain.DTOs;
using GreenEnergyHub.Aggregation.Domain.DTOs.MetaData;
using GreenEnergyHub.Aggregation.Domain.DTOs.MetaData.Enums;
using NodaTime;

namespace GreenEnergyHub.Aggregation.Application.Coordinator
{
    /// <summary>
    /// This service implements the core aggregation coordination functionality
    /// </summary>
    public interface ICoordinatorService
    {
        /// <summary>
        /// Start an aggregation job
        /// </summary>
        /// <param name="jobId">The unique id provided by calling entity</param>
        /// <param name="snapshotId">The unique snapshotId</param>
        /// <param name="jobType">Simulation/Live</param>
        /// <param name="jobOwner">Id of the entity calling the job</param>
        /// <param name="resolution">What resolution should we run this calculation with</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Task</returns>
        Task StartAggregationJobAsync(
            Guid jobId,
            Guid snapshotId,
            JobTypeEnum jobType,
            string jobOwner,
            string resolution,
            CancellationToken cancellationToken);

        /// <summary>
        /// Handles the aggregation results coming back from databricks
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="resultId"></param>
        /// <param name="processType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Async task</returns>
        Task HandleResultAsync(string inputPath, string resultId, string processType, Instant startTime, Instant endTime, CancellationToken cancellationToken);

        /// <summary>
        /// Start a wholesale job
        /// </summary>
        /// <returns>Async task</returns>
        Task StartWholesaleJobAsync(
            Guid jobId,
            Guid snapshotId,
            JobTypeEnum jobType,
            string jobOwner,
            string processVariant,
            CancellationToken cancellationToken);

        /// <summary>
        /// Start a data preparation job
        /// </summary>
        /// <returns>Async task</returns>
        Task StartDataPreparationJobAsync(
            Guid snapshotId,
            Guid jobId,
            Instant fromDate,
            Instant toDate,
            string gridAreas,
            CancellationToken cancellationToken);

        /// <summary>
        /// Update snapshot path
        /// </summary>
        /// <param name="snapshotId"></param>
        /// <param name="path"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateSnapshotPathAsync(
            Guid snapshotId,
            string path);

        /// <summary>
        /// Get JobMetaData object
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns>JobMetaData</returns>
        Task<JobMetadata> GetJob(Guid jobId);
    }
}
