using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Resolvers;
using LTCBR2.Types;
using Attribute = LTCBR2.Types.Attribute;

namespace LTCBR2.Keeper
{
    public class OwlWorker
    {

        public class AbstractClass
        {
            public string name;
            public string parent;
            public List<DataProperty> attributes = new List<DataProperty>();
            public List<ObjectProperty> connections = new List<ObjectProperty>();
        }


        public class ObjectProperty
        {
            public string name;
            public string count; //0-some
        }

        public class DataProperty
        {
            public string name;
            public List<string> possibleValues = new List<string>();
        }


        public List<AbstractClass> LoadOntologyModel(string filename)
        {
            var classModel = new List<AbstractClass>();

            XmlDocument xml = new XmlDocument();
            xml.Load(filename);

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
                newClass.name = classList[i].Attributes?["IRI"].Value;

                for (var j = 0; j < subclassList.Count; j++)
                {
                    if (subclassList[j].ChildNodes[0].Attributes?["IRI"].Value == newClass.name)
                    {
                        newClass.parent = subclassList[j].ChildNodes[1].Attributes?["IRI"].Value;
                    }
                }

                for (var j = 0; j < dataPropertyDomainList.Count; j++)
                {
                    if (dataPropertyDomainList[j].ChildNodes[1].Attributes?["IRI"].Value == newClass.name)
                    {
                        var newAttribute = new DataProperty();
                        newAttribute.name = dataPropertyDomainList[j].ChildNodes[0].Attributes?["IRI"].Value;
                        for (var k = 0; k < dataPropertyRangeList.Count; k++)
                        {
                            if (dataPropertyRangeList[k].ChildNodes[0].Attributes?["IRI"].Value == newAttribute.name)
                            {
                                var possibleValues = new List<string>();
                                if (dataPropertyRangeList[k].ChildNodes[1].Name == "Datatype")
                                {
                                    possibleValues.Add(
                                        dataPropertyRangeList[k].ChildNodes[1].Attributes?["abbreviatedIRI"].Value);
                                }
                                else
                                {
                                    for (var l = 0; l < dataPropertyRangeList[k].ChildNodes[1].ChildNodes.Count; l++)
                                    {
                                        possibleValues.Add(
                                            dataPropertyRangeList[k].ChildNodes[1].ChildNodes[l].InnerText);
                                    }
                                }
                                newAttribute.possibleValues.AddRange(possibleValues);
                            }
                        }
                        newClass.attributes.Add(newAttribute);
                    }
                }

                for (var j = 0; j < objectPropertyDomainList.Count; j++)
                {
                    if (objectPropertyDomainList[j].ChildNodes[1].ChildNodes[1].Attributes?["IRI"].Value ==
                        newClass.name)
                    {
                        var newConnection = new ObjectProperty();
                        for (var k = 0; k < objectPropertyRangeList.Count; k++)
                        {
                            if (objectPropertyRangeList[k].ChildNodes[0].Attributes?["IRI"].Value ==
                                objectPropertyDomainList[j].ChildNodes[0].Attributes?["IRI"].Value)
                            {
                                newConnection.name = objectPropertyRangeList[k].ChildNodes[1].Attributes?["IRI"].Value;
                            }
                        }
                        switch (objectPropertyDomainList[j].ChildNodes[1].Name)
                        {
                            case "ObjectExactCardinality":
                                newConnection.count =
                                    objectPropertyDomainList[j].ChildNodes[1].Attributes?["cardinality"].Value;
                                break;
                            case "ObjectSomeValuesFrom":
                                newConnection.count = "0";
                                break;
                        }
                        newClass.connections.Add(newConnection);
                    }
                }


                classModel.Add(newClass);
            }
            //set up parent properties to childs
            foreach (var parentModel in classModel)
            {
                foreach (var childModel in classModel)
                {
                    if (childModel.parent == parentModel.name)
                    {
                        childModel.attributes.AddRange(parentModel.attributes);
                        childModel.connections.AddRange(parentModel.connections);
                    }
                }
            }

            return classModel;
        }

        public List<Situation> LoadIndividuals(string filename)
        {
            var individuals = new List<Situation>();

            XmlDocument xml = new XmlDocument();
            xml.Load(filename);

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
                    newSituation.solution
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
            }
            return individuals;
        }


    }
}