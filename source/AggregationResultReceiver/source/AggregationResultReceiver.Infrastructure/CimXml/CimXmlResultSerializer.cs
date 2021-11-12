﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using AggregationResultReceiver.Application.Serialization;
using Energinet.DataHub.ResultReceiver.Domain;
using NodaTime;

namespace AggregationResultReceiver.Infrastructure.CimXml
{
    public class CimXmlResultSerializer : ICimXmlResultSerializer
    {
        public Task SerializeToStreamAsync(IEnumerable<ResultData> results, Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public List<XDocument> MapToCimXml(IEnumerable<ResultData> results) // include message from coordinator
        {
            var resultsGroupedOnGridAreaAndResultName = results
                .GroupBy(x => x.GridArea)
                .Select(g => g
                    .GroupBy(y => y.ResultName)
                    .Select(h => h
                        .ToList())
                    .ToList())
                .ToList();

            List<XDocument> cimXmlFiles = new List<XDocument>();

            XNamespace cimNamespace = "urn:ediel.org:measure:notifyaggregatedtimeseries:0:1";
            XNamespace xmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace xmlSchemaLocation = "urn:ediel.org:measure:notifyaggregatedtimeseries:0:1 urn-ediel-org-measure-notifyaggregatedtimeseries-0-1.xsd";

            foreach (var item in resultsGroupedOnGridAreaAndResultName)
            {
                XDocument document = new XDocument(
                    new XElement(
                        cimNamespace + "NotifyAggregatedTimeSeries_MarketDocument",
                        new XAttribute(
                            XNamespace.Xmlns + "xsi",
                            xmlSchemaNamespace),
                        new XAttribute(
                            XNamespace.Xmlns + "cim",
                            cimNamespace),
                        new XAttribute(
                            xmlSchemaNamespace + "schemaLocation",
                            xmlSchemaLocation),
                        new XElement(
                            cimNamespace + "mRID",
                            Guid.NewGuid()),
                        new XElement(
                            cimNamespace + "type",
                            "E31"), // const
                        new XElement(
                            cimNamespace + "process.processType",
                            "D04"), // get from coordinator message
                        new XElement(
                            cimNamespace + "businessSector.type",
                            "23"), // always 23 for electricity
                        new XElement(
                            cimNamespace + "sender_MarketParticipant.mRID",
                            new XAttribute(
                                "codingScheme",
                                "A10"), // const: A10 is datahub
                            "5790001330552"), // const: datahub gln number
                        new XElement(
                            cimNamespace + "sender_MarketParticipant.marketRole.type",
                            "DGL"), // const: role of datahub
                        new XElement(
                            cimNamespace + "receiver_MarketParticipant.mRID",
                            new XAttribute(
                                "codingScheme",
                                "A10"), // get from some where
                            "5799999933318"), // gln
                        new XElement(
                            cimNamespace + "receiver_MarketParticipant.marketRole.type",
                            "MDR"), // get from coordinator message
                        new XElement(
                            cimNamespace + "createdDateTime",
                            Instant.FromDateTimeUtc(DateTime.Now.ToUniversalTime())),
                        GetSeries(item, cimNamespace)));
                cimXmlFiles.Add(document);
            }

            return cimXmlFiles;
        }

        public IEnumerable<XElement> GetSeries(List<List<ResultData>> item, XNamespace cimNamespace)
        {
            List<XElement> series = new List<XElement>();
            foreach (var s in item)
            {
                series.Add(new XElement(
                    cimNamespace + "Series",
                    new XElement(
                        cimNamespace + "mRID",
                        Guid.NewGuid()),
                    new XElement(
                        cimNamespace + "version",
                        "1"), // get from coordinator message
                    new XElement(
                        cimNamespace + "marketEvaluationPoint.type",
                        s.First().MeteringPointType),
                    new XElement(
                        cimNamespace + "marketEvaluationPoint.settlementMethod",
                        s.First().SettlementMethod),
                    new XElement(
                        cimNamespace + "meteringGridArea_Domain.mRID",
                        new XAttribute(
                            "codingScheme",
                            "NDK"), // const: NDK is grid areas of denmark
                        s.First().GridArea),
                    new XElement(
                        cimNamespace + "product",
                        "8716867000030"), // const: product type
                    new XElement(
                        cimNamespace + "quantity_Measure_Unit.name",
                        "KWH"),
                    GetPeriod(s, cimNamespace)));
            }

            return series;
        }

        public XElement GetPeriod(List<ResultData> s, XNamespace cimNamespace)
        {
            return new XElement(
                cimNamespace + "Period",
                new XElement(
                    cimNamespace + "resolution",
                    s.First().Resolution),
                new XElement(
                    cimNamespace + "timeInterval",
                    new XElement(
                        cimNamespace + "start",
                        "2021-09-05T22:00Z"),
                    new XElement(
                        cimNamespace + "end",
                        "2221-09-06T22:00Z")),
                GetPoints(s, cimNamespace));
        }

        public IEnumerable<XElement> GetPoints(List<ResultData> s, XNamespace cimNamespace)
        {
            List<XElement> points = new List<XElement>();
            var pointIndex = 1;
            foreach (var point in s.OrderBy(t => t.StartDateTime))
            {
                points.Add(new XElement(
                    cimNamespace + "Point",
                    new XElement(
                        cimNamespace + "position",
                        pointIndex),
                    new XElement(
                        cimNamespace + "quantity",
                        point.SumQuantity),
                    new XElement(
                        cimNamespace + "quality",
                        point.Quality)));
                pointIndex++;
            }

            return points;
        }
    }
}
