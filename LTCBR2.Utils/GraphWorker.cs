using System;
using System.Collections.Generic;
using System.Linq;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;
using LTCBR2.Types;

namespace LTCBR2.Utils
{
    public class GraphWorker
    {
        public string MakeGraph(string graphInString)
        {
            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);
            var wrapper = new GraphGeneration(getStartProcessQuery,
                getProcessStartInfoQuery,
                registerLayoutPluginCommand) {GraphvizPath = @"G:\Projects\GraphViz\bin"};
            wrapper.RenderingEngine = Enums.RenderingEngine.Dot;
            
            var output = wrapper.GenerateGraph(graphInString, Enums.GraphReturnType.PlainExt);
            var result = System.Text.Encoding.UTF8.GetString(output);
            return result;
        }

        public string SituationToGraph(List<Participant> listOfParticipants)
        {
            var listOfPastParticipant = new List<int>();
            var graphInString = "graph{ splines=line; graph [pad=\"5000\"; ranksep=\"100\", nodesep=\"100\"]; ";
            foreach (var participant in listOfParticipants)
            {
                graphInString = participant.connections.Where(connection => listOfPastParticipant.IndexOf(connection) < 0)
                    .Aggregate(graphInString, (current, connection) => current + (participant.id.ToString() + " -- " + connection.ToString() + "; "));
                listOfPastParticipant.Add(participant.id);
            }
            graphInString += "}";
            return graphInString;
        }

        public Situation FillCoordinates(Situation situation, string graphInString)
        {
            var lc = new List<Coordinate>();
            graphInString = graphInString.Replace(".", ",");
            while (graphInString.Contains("node"))
            {
                graphInString = graphInString.Remove(0, graphInString.IndexOf("node", StringComparison.Ordinal)+4);
                graphInString = graphInString.Remove(0, graphInString.IndexOf(" ", StringComparison.Ordinal)+1);
                var id = Convert.ToInt16(graphInString.Substring(0, graphInString.IndexOf(" ", StringComparison.Ordinal) + 1));
                graphInString = graphInString.Remove(0, graphInString.IndexOf(" ", StringComparison.Ordinal)+1);
                var i = Convert.ToDouble(graphInString.Substring(0, graphInString.IndexOf(" ", StringComparison.Ordinal)));
                graphInString = graphInString.Remove(0, graphInString.IndexOf(" ", StringComparison.Ordinal)+1);
                var c = Convert.ToDouble(graphInString.Substring(0, graphInString.IndexOf(" ", StringComparison.Ordinal)));
                var coordinate = new Coordinate
                {
                    id = id,
                    x = i,
                    y = c
                };
                lc.Add(coordinate);
            }
            situation.coordinates.Clear();
            situation.coordinates = lc;
            return situation;
        }
    }
}