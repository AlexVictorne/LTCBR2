using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using LTCBR2.Types;

namespace LTCBR2.Utils
{
    public class ImportFromConstructor
    {
        public Situation load_pps(string xml)
        {
            var output = new Situation();
            RAXY.Situation.Situation s = null;
            var ser = new XmlSerializer(typeof(RAXY.Situation.Situation));
            //var reader = new FileStream(path, FileMode.Open);
            var reader = new StringReader(xml);
            s = (RAXY.Situation.Situation)ser.Deserialize(reader);
            reader.Close();
            output = parsePps(s);
            return output;
        }

        public Situation parsePps(RAXY.Situation.Situation situation)
        {
            var counter = 0;

            var output = new Situation();
            
            output.participants = new List<Participant>();

            foreach (var s in situation.Subjects)
            {
                var lsa = new List<Attribute>();
                
                lsa.Add(new Attribute
                {
                    name = "Тип " + s.SubjectClass,
                    value = s.Type
                });
                foreach (var p in s.Properties)
                {
                    lsa.Add(new Attribute
                    {
                        name = p.Name,
                        value = p.Value
                    });
                }
                output.participants.Add(new Participant
                {
                    name = s.Name,
                    purpose = "Subject",
                    className = s.SubjectClass,
                    attributes = lsa,
                    connections = new List<int>()
                });
                output.participants[output.participants.Count - 1].id = output.participants.Count - 1;
            }

            foreach (var p in situation.Processes)
            {
                var lsa = new List<Attribute>();
                lsa.Add(new Attribute
                {
                    name = "Тип " + p.Action,
                    value = p.ActionType
                });
                lsa.Add(new Attribute
                {
                    name = p.ActionType,
                    value = "e.t.c"
                });
                output.participants.Add(new Participant
                {
                    name = p.Action + p.ActionTarget,
                    purpose = "Process",
                    className = p.ActionType,
                    attributes = lsa,
                    connections = new List<int>()
                });
                output.participants[output.participants.Count - 1].id = output.participants.Count - 1;
                if (output.participants[output.participants.Count - 1].connections == null)
                    output.participants[output.participants.Count - 1].connections =
                        new List<int>();
                foreach (var pa in output.participants)
                {
                    if ((pa.name == p.Subject) || (pa.name == p.ActionTarget))
                    {
                        if (pa.connections == null)
                            pa.connections = new List<int>();
                        pa.connections.Add(output.participants[output.participants.Count - 1].id);
                        output.participants[output.participants.Count - 1].connections.Add(pa.id);
                    }
                }
            }

            foreach (var r in situation.Relations)
            {
                var lsa = new List<Attribute>();
                lsa.Add(new Attribute
                {
                    name = r.RelationType,
                    value = r.RelationProperty
                });
                output.participants.Add(new Participant
                {
                    name = r.RelationType + r.RelationProperty,
                    purpose = "Relation",
                    className = r.RelationType,
                    attributes = lsa,
                    connections = new List<int>()
                });
                output.participants[output.participants.Count - 1].id = output.participants.Count - 1;
                if (output.participants[output.participants.Count - 1].connections == null)
                    output.participants[output.participants.Count - 1].connections =
                        new List<int>();
                foreach (var pa in output.participants)
                {
                    if ((pa.name == r.Subject1) || (pa.name == r.Subject2))
                    {
                        if (pa.connections == null)
                            pa.connections = new List<int>();
                        pa.connections.Add(output.participants[output.participants.Count - 1].id);
                        output.participants[output.participants.Count - 1].connections.Add(pa.id);
                    }
                }
            }
            output.id = output.GetHashCode();
            return output;
        }


    }
}