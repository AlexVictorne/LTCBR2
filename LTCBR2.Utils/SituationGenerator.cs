using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;
using LTCBR2.Keeper;
using LTCBR2.Types;
using LTCBR2.Types.ForOntology;

namespace LTCBR2.Utils
{
    public class SituationGenerator
    {
        List<AbstractClass> _subjects, _procesess, _relations;


        public SituationGenerator(XmlDocument xml)
        {
            _subjects = OwlWorker.LoadOntologyModel(xml).Where(x => x.Purpose == "Subject").ToList();
            _procesess = OwlWorker.LoadOntologyModel(xml).Where(x => x.Purpose == "Process").ToList();
            _relations = OwlWorker.LoadOntologyModel(xml).Where(x => x.Purpose == "Relation").ToList();
        }

        public List<Situation> GenerateSituationList(int sitCount, int subjectCount, int relationCount, int processCount, int attr)
        {
            var resultList = new List<Situation>();
            for (var i = 0; i < sitCount; i++)
            {
                resultList.Add(GenerateSituation(subjectCount,relationCount,processCount));
            }
            return resultList;
        }

        private Situation GenerateSituation(int subjectCount, int relationCount, int processCount)
        {
            var resultSituation = new Situation();
            //create properties
            //resultSituation.id = resultSituation.GetHashCode();
            resultSituation.name = resultSituation.id.ToString();
            resultSituation.description = "test situation";
            resultSituation.solution = "no solution";
            resultSituation.type = "test";
            resultSituation.participants = new List<Participant>();
            
            //create participants
            for (var i = 0; i < subjectCount; i++)
                resultSituation.participants.Add(MakeParticipant("Subject",resultSituation.participants.Count));
            for (var i = 0; i < processCount; i++)
                resultSituation.participants.Add(MakeParticipant("Process", resultSituation.participants.Count));
            for (var i = 0; i < relationCount; i++)
                resultSituation.participants.Add(MakeParticipant("Relation", resultSituation.participants.Count));

            foreach (var participant in resultSituation.participants)
            {
                if (participant.purpose == "Process")
                {
                    //получить класс процесса
                    var currentProcessClass = _procesess.First(x => x.Name == participant.className);
                    //для всех связей в классе
                    foreach (var connection in currentProcessClass.Connections)
                    {
                        //для всех участником ситуации имеющих класс текущего процесса
                        foreach (var participantToProcess in resultSituation.participants.Where(x=>x.className==connection.Name).ToList())
                        {
                            
                        }
                    }
                }
                if (participant.purpose == "Relation")
                {
                    var currentRelation = _relations.First(x => x.Name == participant.className);
                    
                }
            }



            return resultSituation;
        }

        private Participant MakeParticipant(string purpose, int id)
        {
            var r = new Random();
            var resultParticipant = new Participant();
            resultParticipant.id = id;
            resultParticipant.purpose = purpose;
            switch (purpose)
            {
                case "Subject":
                    var tmpSubjects = _subjects.Where(x => x.Parent == "Subject").ToList();
                    var selectedClass = tmpSubjects[r.Next(0, tmpSubjects.Count)];
                    Thread.Sleep(15);
                    resultParticipant.className = selectedClass.Name;
                    break;
                case "Process":
                    var tmpProcesses = _procesess.Where(x => x.Purpose == "Process").ToList();
                    var selectedProcess = tmpProcesses[r.Next(0, tmpProcesses.Count)];
                    Thread.Sleep(15);
                    resultParticipant.className = selectedProcess.Name;
                    break;
                case "Relation":
                    var tmpRelations = _relations.Where(x => x.Purpose == "Relation").ToList();
                    var selectedRelation = tmpRelations[r.Next(0, tmpRelations.Count)];
                    Thread.Sleep(15);
                    resultParticipant.className = selectedRelation.Name;
                    break;
            }
            return resultParticipant;
        }

    }
}