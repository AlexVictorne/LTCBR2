using System.Collections.Generic;
using LTCBR2.Types;

namespace LTCBR2.Utils
{
    public class SituationGenerator
    {
        public List<Situation> GenerateSituationList(int sitCount, int nodeCount, int conCount, int attCount)
        {
            var resultList = new List<Situation>();
            for (var i = 0; i < sitCount; i++)
            {
                resultList.Add(GenerateSituation(nodeCount,conCount,attCount));
            }
            return resultList;
        }

        private Situation GenerateSituation(int nodeCount, int conCount, int attCount)
        {
            var resultSituation = new Situation();
            //create properties
            resultSituation.id = resultSituation.GetHashCode();
            resultSituation.name = resultSituation.id.ToString();
            resultSituation.description = "test situation";
            resultSituation.solution = "no solution";
            resultSituation.type = "test";
            //create participants


            return resultSituation;
        }

        private Participant MakeParticipant(int attCount)
        {
            var resultParticipant = new Participant();

            return resultParticipant;
        }

    }
}