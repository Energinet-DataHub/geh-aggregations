﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Energinet.DataHub.Aggregations.Infrastructure.Persistence
{
    public partial class GridLossSysCorr
    {
        public Guid RowId { get; set; }
        public string MeteringPointId { get; set; }
        public string GridArea { get; set; }
        public string EnergySupplierId { get; set; }
        public bool IsGridLoss { get; set; }
        public bool IsSystemCorrection { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}