using System.Collections.Generic;
using LTCBR2.Types;

namespace LTCBR2.Searcher
{
    public class SituationSearcher
    {
        //1s - out of base, 2s - in base
        public List<Situation> SituationsInBase { get; set; }
        public double RoughValue { get; set; }


        public void SearchStart(Situation inSituation)
        {
            foreach (var situation2 in SituationsInBase)
            {
                situation2.rate = RoughCompare(inSituation.participants, situation2.participants);
            }
        }
        
        //compare connections and attributes
        //participants1 - new
        //participants2 - in base
        public double AccurateCompare(List<Participant> participants1, List<Participant> participants2)
        {
            //список уже пройденных элементов
            List<Participant> noWayList = new List<Participant>();
            
           // foreach (var VARIABLE in )
           // {
           //     
          //  }
            //найти совпадение по типу 
            //сравнить атрибуты
            //перейти по связи
            return 0.0;
        }

        


        //compare on attribute vector
        public double RoughCompare(List<Participant> participants1 , List<Participant> participants2)
        {
            var rate = 0.0;
            var attrCount = 0;
            foreach (var participant2 in participants2)
            {
                foreach (var participant1 in participants1)
                {
                    foreach (var attribute2 in participant2.attributes)
                    {
                        foreach (var attribute1 in participant1.attributes)
                        {
                            if ((attribute2.name == attribute1.name) && (attribute2.value == attribute1.value))
                                rate++;
                        }
                    }
                }
                attrCount += participant2.attributes.Count;
            }
            return rate/attrCount;
        }


    }
}