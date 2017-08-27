using System;
using System.Collections.Generic;
using System.Xml;
using LTCBR2.Types;
using LTCBR2.Types.ForOntology;
using Attribute = LTCBR2.Types.Attribute;

namespace LTCBR2.Keeper
{
    public static class OwlWorker
    {

        public static List<Situation> CurrentSituationBaseFromOwl = new List<Situation>();

        public static string ToNormal(string inString)
        {
            return inString.Replace("#", "");
        }

        public static List<AbstractClass> LoadOntologyModel(XmlDocument xml)
        {
            var classModel = new List<AbstractClass>();
            
            var classList = xml.SelectNodes("//*[local-name()='Declaration']//*[local-name()='Class']");
            var dataPropertyDomainList = xml.SelectNodes("//*[local-name()='DataPropertyDomain']");
            var dataPropertyRangeList = xml.SelectNodes("//*[local-name()='DataPropertyRange']");
            var objectPropertyDomainList = xml.SelectNodes("//*[local-name()='ObjectPropertyDomain']");
            var objectPropertyRangeList = xml.SelectNodes("//*[local-name()='ObjectPropertyRange']");
            var subclassList = xml.SelectNodes("//*[local-name()='SubClassOf']");



            if (classList == null) return null;
            for (var i = 0; i < classList.Count; i++)
            {
                var newClass = new AbstractClass();
                newClass.Name = ToNormal(classList[i].Attributes?["IRI"].Value);

                for (var j = 0; j < subclassList.Count; j++)
                {
                    if (ToNormal(subclassList[j].ChildNodes[0].Attributes?["IRI"].Value) == newClass.Name)
                    {
                        newClass.Parent = ToNormal(subclassList[j].ChildNodes[1].Attributes?["IRI"].Value);
                    }
                }

                for (var j = 0; j < dataPropertyDomainList.Count; j++)
                {
                    if (ToNormal(dataPropertyDomainList[j].ChildNodes[1].Attributes?["IRI"].Value) == newClass.Name)
                    {
                        var newAttribute = new DataProperty();
                        newAttribute.Name = ToNormal(dataPropertyDomainList[j].ChildNodes[0].Attributes?["IRI"].Value);
                        for (var k = 0; k < dataPropertyRangeList.Count; k++)
                        {
                            if (ToNormal(dataPropertyRangeList[k].ChildNodes[0].Attributes?["IRI"].Value) == newAttribute.Name)
                            {
                                var possibleValues = new List<string>();
                                if (dataPropertyRangeList[k].ChildNodes[1].Name == "Datatype")
                                {
                                    possibleValues.Add(
                                        ToNormal(dataPropertyRangeList[k].ChildNodes[1].Attributes?["abbreviatedIRI"].Value));
                                }
                                else
                                {
                                    for (var l = 0; l < dataPropertyRangeList[k].ChildNodes[1].ChildNodes.Count; l++)
                                    {
                                        possibleValues.Add(
                                            ToNormal(dataPropertyRangeList[k].ChildNodes[1].ChildNodes[l].InnerText));
                                    }
                                }
                                newAttribute.PossibleValues.AddRange(possibleValues);
                            }
                        }
                        newClass.Attributes.Add(newAttribute);
                    }
                }

                for (var j = 0; j < objectPropertyDomainList.Count; j++)
                {
                    if (ToNormal(objectPropertyDomainList[j].ChildNodes[1].ChildNodes[1].Attributes?["IRI"].Value) ==
                        newClass.Name)
                    {
                        var newConnection = new ObjectProperty();
                        for (var k = 0; k < objectPropertyRangeList.Count; k++)
                        {
                            if (ToNormal(objectPropertyRangeList[k].ChildNodes[0].Attributes?["IRI"].Value) ==
                                ToNormal(objectPropertyDomainList[j].ChildNodes[0].Attributes?["IRI"].Value))
                            {
                                newConnection.Name = ToNormal(objectPropertyRangeList[k].ChildNodes[1].Attributes?["IRI"].Value);
                            }
                        }
                        switch (objectPropertyDomainList[j].ChildNodes[1].Name)
                        {
                            case "ObjectExactCardinality":
                                newConnection.Count =
                                    objectPropertyDomainList[j].ChildNodes[1].Attributes?["cardinality"].Value;
                                break;
                            case "ObjectSomeValuesFrom":
                                newConnection.Count = "0";
                                break;
                        }
                        newClass.Connections.Add(newConnection);
                    }
                }


                classModel.Add(newClass);
            }
            //set up parent properties to childs
            foreach (var parentModel in classModel)
            {
                foreach (var childModel in classModel)
                {
                    if (childModel.Parent == parentModel.Name)
                    {
                        childModel.Attributes.AddRange(parentModel.Attributes);
                        childModel.Connections.AddRange(parentModel.Connections);
                    }
                }
            }
            foreach (var purpose in new List<string>{"Subject","Process","Relation"})
            {
                foreach (var klass in classModel)
                {
                    if (klass.Parent == purpose)
                    {
                        klass.Purpose = purpose;
                        foreach (var subKlass in classModel)
                        {
                            if (subKlass.Parent == klass.Name)
                            {
                                subKlass.Purpose = purpose;
                            }
                        }
                    }
                }
            }
            return classModel;
        }

        public static void LoadIndividuals(XmlDocument xml)
        {
            var individuals = new List<Situation>();
            
            var classAsIndList = xml.SelectNodes("//*[local-name()='ClassAssertion']");
            var dataPropertyAsIndList = xml.SelectNodes("//*[local-name()='DataPropertyAssertion']");
            var objectPropertyAsIndList = xml.SelectNodes("//*[local-name()='ObjectPropertyAssertion']");

            for (var i = 0; i < classAsIndList.Count; i++)
            {
                if (classAsIndList[i].ChildNodes[0].Attributes["IRI"].Value == "#Situation")
                {
                    var newSituation = new Situation();
                    newSituation.id = newSituation.GetHashCode();
                    newSituation.create_date = DateTime.Now;
                    for (var j = 0; j < dataPropertyAsIndList.Count; j++)
                    {
                        if (dataPropertyAsIndList[j].ChildNodes[1].Attributes["IRI"].Value ==
                            classAsIndList[i].ChildNodes[1].Attributes["IRI"].Value)
                            switch (dataPropertyAsIndList[j].ChildNodes[0].Attributes["IRI"].Value)
                            {
                                case "#Description":
                                    newSituation.description = dataPropertyAsIndList[j].ChildNodes[2].InnerText;
                                    break;
                                case "#Name":
                                    newSituation.name = dataPropertyAsIndList[j].ChildNodes[2].InnerText;
                                    break;
                            }
                    }
                    newSituation.participants = new List<Participant>();
                    int idCounter = 0;
                    for (var j = 0; j < objectPropertyAsIndList.Count; j++)
                    {
                        if (objectPropertyAsIndList[j].ChildNodes[1].Attributes["IRI"].Value ==
                            classAsIndList[i].ChildNodes[1].Attributes["IRI"].Value)
                        {
                            var newParticipant = new Participant();
                            newParticipant.id = idCounter;
                            idCounter++;
                            newParticipant.name = objectPropertyAsIndList[j].ChildNodes[2].Attributes["IRI"].Value;
                            //newParticipant.className;
                            for (var k = 0; k < classAsIndList.Count; k++)
                            {
                                if (objectPropertyAsIndList[j].ChildNodes[2].Attributes["IRI"].Value==
                                    classAsIndList[k].ChildNodes[1].Attributes["IRI"].Value)
                                {
                                    newParticipant.className = classAsIndList[k].ChildNodes[0].Attributes["IRI"].Value;
                                }
                            }
                            newParticipant.purpose = objectPropertyAsIndList[j].ChildNodes[0].Attributes["IRI"].Value.Remove(0,4);
                            //добавить аттрибуты
                            newParticipant.attributes = new List<Attribute>();
                            for (var k = 0; k < dataPropertyAsIndList.Count; k++)
                            {
                                if (dataPropertyAsIndList[k].ChildNodes[1].Attributes["IRI"].Value ==
                                    objectPropertyAsIndList[j].ChildNodes[2].Attributes["IRI"].Value)
                                {
                                    var newAttribute = new Attribute();
                                    newAttribute.name = dataPropertyAsIndList[k].ChildNodes[0].Attributes["IRI"].Value;
                                    newAttribute.value = dataPropertyAsIndList[k].ChildNodes[2].InnerText;
                                    newParticipant.attributes.Add(newAttribute);
                                }
                            }
                            newSituation.participants.Add(newParticipant);
                        }
                        else if (objectPropertyAsIndList[j].ChildNodes[2].Attributes["IRI"].Value ==
                            classAsIndList[i].ChildNodes[1].Attributes["IRI"].Value)
                        {
                            for (var k = 0; k < objectPropertyAsIndList.Count; k++)
                            {
                                if ((objectPropertyAsIndList[j].ChildNodes[1].Attributes["IRI"].Value ==
                                     objectPropertyAsIndList[k].ChildNodes[1].Attributes["IRI"].Value) &&
                                    (objectPropertyAsIndList[k].ChildNodes[2].Attributes["IRI"].Value !=
                                     objectPropertyAsIndList[j].ChildNodes[2].Attributes["IRI"].Value))
                                {
                                    var solution = new Participant {id = idCounter};
                                    idCounter++;
                                    solution.name = objectPropertyAsIndList[k].ChildNodes[2].Attributes["IRI"].Value;
                                    solution.connections = new List<int>();
                                    solution.purpose = objectPropertyAsIndList[k].ChildNodes[0].Attributes["IRI"].Value.Remove(0, 4);
                                    //newParticipant.className;
                                    for (var l = 0; l < classAsIndList.Count; l++)
                                    {
                                        if (objectPropertyAsIndList[k].ChildNodes[2].Attributes["IRI"].Value ==
                                            classAsIndList[l].ChildNodes[1].Attributes["IRI"].Value)
                                        {
                                            solution.className = classAsIndList[l].ChildNodes[0].Attributes["IRI"].Value;
                                        }
                                    }

                                    //добавить аттрибуты
                                    solution.attributes = new List<Attribute>();
                                    for (var l = 0; l < dataPropertyAsIndList.Count; l++)
                                    {
                                        if (dataPropertyAsIndList[l].ChildNodes[1].Attributes["IRI"].Value ==
                                            objectPropertyAsIndList[k].ChildNodes[2].Attributes["IRI"].Value)
                                        {
                                            var newAttribute = new Attribute();
                                            newAttribute.name = dataPropertyAsIndList[l].ChildNodes[0].Attributes["IRI"].Value;
                                            newAttribute.value = dataPropertyAsIndList[l].ChildNodes[2].InnerText;
                                            solution.attributes.Add(newAttribute);
                                        }
                                    }
                                    newSituation.solutionInParticipant = solution;
                                }
                            }
                        }
                    }
                    individuals.Add(newSituation);
                }
            }
            //восстановить связи между участниками ситуации

            foreach (var individ in individuals)
            {
                foreach (var participant in individ.participants)
                {
                    if (participant.connections == null)
                        participant.connections = new List<int>();
                    for (var i = 0; i < objectPropertyAsIndList.Count; i++)
                    {
                        if (participant.name == objectPropertyAsIndList[i].ChildNodes[1].Attributes["IRI"].Value)
                        {
                            foreach (var participant2 in individ.participants)
                            {
                                if (participant2.name ==
                                    objectPropertyAsIndList[i].ChildNodes[2].Attributes["IRI"].Value)
                                {
                                    if (participant2.connections == null)
                                        participant2.connections = new List<int>();
                                    participant.connections.Add(participant2.id);
                                    participant2.connections.Add(participant.id);
                                }
                            }
                        }

                    }
                }
                for (var i = 0; i < objectPropertyAsIndList.Count; i++)
                {
                    if (individ.solutionInParticipant.name ==
                        objectPropertyAsIndList[i].ChildNodes[1].Attributes["IRI"].Value)
                    {
                        foreach (var participant2 in individ.participants)
                        {
                            if (participant2.name ==
                                objectPropertyAsIndList[i].ChildNodes[2].Attributes["IRI"].Value)
                            {
                                if (participant2.connections == null)
                                    participant2.connections = new List<int>();
                                individ.solutionInParticipant.connections.Add(participant2.id);
                            }
                        }
                    }
                }
            }
            CurrentSituationBaseFromOwl.Clear();
            CurrentSituationBaseFromOwl.AddRange(individuals);
        }

        public static Situation GetSituation(int id)
        {
            return CurrentSituationBaseFromOwl.Find(x => x.id == id);
        }
    }
}