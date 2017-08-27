using System;
using System.Linq;
using System.Security.Cryptography;
using LTCBR2.Types;
using LTCBR2.Types.TypesForGpu;

namespace LTCBR2.Utils
{
    public class Tools
    {
        public static int ConvertToInt(string inString)
        {
            if (inString == null)
                return 0;
            var result = inString.ToUpper().GetHashCode();
            return result;
        }

        public static SituationGpu SituationToSituationGpu(Situation inSituation)
        {
            var result = new SituationGpu();
            result.Participants = new int[inSituation.participants.Count, 3];
            var maxCountOfAttributes = inSituation.participants.Select(participant => participant.attributes.Count).Concat(new[] {0}).Max();
            result.Attributes = new int[inSituation.participants.Count,maxCountOfAttributes,2];
            result.Connections = new int[inSituation.participants.Count,inSituation.participants.Count];
            result.Purposes = new int[inSituation.participants.Count];
            foreach (var participant in inSituation.participants)
            {
                result.Participants[participant.id, 1] = ConvertToInt(participant.className);
                result.Participants[participant.id, 2] = ConvertToInt(participant.purpose);
                switch (participant.purpose)
                {
                    case "Subject":
                        result.Purposes[participant.id] = 1;
                        break;
                    case "Process":
                        result.Purposes[participant.id] = 2;
                        break;
                    case "Relation":
                        result.Purposes[participant.id] = 3;
                        break;
                }
                var attributeCount = 0;
                foreach (var attribute in participant.attributes)
                {
                    if ((attribute.name == null) || (attribute.value == null)) continue;
                    result.Attributes[participant.id, attributeCount, 0] = ConvertToInt(attribute.name);
                    result.Attributes[participant.id, attributeCount, 1] = ConvertToInt(attribute.value);
                    attributeCount++;
                }
                foreach (var connection in participant.connections)
                {
                    result.Connections[participant.id, connection] = 1;
                }
            }
            

            
            //result.Attributes = new int[inSituation.participants.Count,,2];
            return result;
        }


        public static Situation ValidateSituation(Situation inSituation)
        {
            for (int i = 0; i < inSituation.participants.Count; i++)
            {
                if (inSituation.participants[i].id != i)
                {
                    foreach (var participant in inSituation.participants)
                    {
                        if (participant.connections.Any(x => x==inSituation.participants[i].id))
                            for (int j = 0; j < participant.connections.Count; j++)
                            {
                                if (participant.connections[j] == inSituation.participants[i].id)
                                {
                                    participant.connections[j] = i;
                                }
                            }
                    }
                    if (inSituation.coordinates!=null)
                    foreach (var coordinate in inSituation.coordinates)
                    {
                        if (coordinate.id == inSituation.participants[i].id)
                            coordinate.id = i;
                    }
                    inSituation.participants[i].id = i;
                }
            }
            return inSituation;
        }
    }
}