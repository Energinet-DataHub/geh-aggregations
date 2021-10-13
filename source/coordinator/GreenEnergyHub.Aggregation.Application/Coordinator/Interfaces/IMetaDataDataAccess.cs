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
using System.Threading.Tasks;
using GreenEnergyHub.Aggregation.Domain.DTOs;
using GreenEnergyHub.Aggregation.Domain.DTOs.MetaData;

namespace GreenEnergyHub.Aggregation.Application.Coordinator.Interfaces
{
    /// <summary>
    /// Provides CRUD access to metadata
    /// </summary>
    public interface IMetaDataDataAccess
    {
        /// <summary>
        /// Insert Snapshot
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task CreateSnapshotAsync(Snapshot snapshot);

        /// <summary>
        /// Insert JobMetadata
        /// </summary>
        /// <param name="job"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task CreateJobAsync(Job job);

        /// <summary>
        /// Update JobMetadata
        /// </summary>
        /// <param name="job"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateJobAsync(Job job);

        /// <summary>
        /// Insert jobMetadata
        /// </summary>
        /// <param name="jobResult"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task CreateJobResultAsync(JobResult jobResult);

        /// <summary>
        /// Update jobMetadata.
        /// </summary>
        /// <param name="jobResult"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateJobResultAsync(JobResult jobResult);

        /// <summary>
        /// Update snapshot path
        /// </summary>
        /// <param name="snapshotId"></param>
        /// <param name="path"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateSnapshotPathAsync(Guid snapshotId, string path);

        /// <summary>
        /// Get JobMetaData by job id
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns>JobMetaData</returns>
        Task<Job> GetJobAsync(Guid jobId);
    }
}
