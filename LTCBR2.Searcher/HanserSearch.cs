using LTCBR2.Types;

namespace LTCBR2.Searcher
{
    public class HanserSearch
    {
        //сравнить атрибуты двух ситуаций
        public void HanserCompare(Situation situation1, Situation situation2)
        {
            foreach (var participant1 in situation1.participants)
            {
                foreach (var participant2 in situation2.participants)
                {
                    //тут же можно сравнивать атрибуты
                    if ((participant1.className == participant2.className) &&
                        (participant1.purpose == participant2.purpose))
                    {

                    }
                }
            }
        }

        //build associative graph
        public void CreateAssociativeGraph(Situation situation1, Situation situation2)
        {

        }

        //search max clique in associative graph
        public void CliqueSearch()
        {

        }

    }
}