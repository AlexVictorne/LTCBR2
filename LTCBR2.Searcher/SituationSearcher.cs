using System;
using System.Collections.Generic;
using System.Linq;
using LTCBR2.Types;
using LTCBR2.Utils;
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
                Tools.ValidateSituation(inSituation);
                Tools.ValidateSituation(situation2);
                var situationInGpu1 = Tools.SituationToSituationGpu(inSituation);
                var situationInGpu2 = Tools.SituationToSituationGpu(situation2);
                if ((situationInGpu2.Attributes.GetLength(1) == 0) || (situationInGpu1.Attributes.GetLength(1) == 0))
                    continue;
                int[,] compared = new int[inSituation.participants.Count,situation2.participants.Count];
                situation2.rate = 0;
                //HIDE Math.Round((double)Main.RoughSearch(situationInGpu1, situationInGpu2)/situation2.participants.Count,2);
                //situation2.rate = Main.AccurateSearch(situationInGpu1, situationInGpu2, out compared);
                //situation2.rate = RoughCompare(inSituation.participants, situation2.participants);
            }
            SituationsInBase = SituationsInBase.Where(x => x.rate > 0).ToList();
        }
        
        public void RoughSearch(int[,] inParticipants, int[,,] inAttributes, int[,] dbParticipants,
            int[,,] dbAttributes, double roughLimit, out double roughMark, out int[,] compared)
        {
            roughMark = 0.0;
            compared = new int[inParticipants.GetLength(0), dbParticipants.GetLength(0)];
            var countOfCompared = 0;
            for (int i = 0; i < inParticipants.GetLength(0); i++)
            {
                for (int j = 0; j < dbParticipants.GetLength(0); j++)
                {
                    bool result = inParticipants[i, 1] == dbParticipants[j, 1] &&
                                  inParticipants[i, 2] == dbParticipants[j, 2];
                    if (result)
                    {
                        for (int k = 0; k < inAttributes.GetLength(1); k++)
                        {
                            var inPartAttributeName = inAttributes[i, k, 0];
                            var inPartAttributeValue = inAttributes[i, k, 1];

                            if (inPartAttributeName == 0 || inPartAttributeValue == 0)
                            {
                                break;
                            }
                            for (int l = 0; l < dbAttributes.GetLength(1); l++)
                            {
                                var dbPartAttributeName = dbAttributes[j, l, 0];
                                var dbPartAttributeValue = dbAttributes[j, l, 1];

                                if (dbPartAttributeName == 0 || dbPartAttributeValue == 0)
                                {
                                    break;
                                }

                                result = inPartAttributeName == dbPartAttributeName &&
                                         inPartAttributeValue == dbPartAttributeValue;
                                if (result)
                                {
                                    break;
                                }
                            }
                        }
                        if (!result)
                        {
                            break;
                        }
                    }
                    compared[i, j] = result ? 1 : 0;
                    countOfCompared = result ? countOfCompared + 1 : countOfCompared;
                }
            }
            if (countOfCompared > 0)
            {
                roughMark = Math.Round((double) countOfCompared/inParticipants.GetLength(0), 2);
            }

        }
        
        public void AccurateSearch(int[,] inConnections, int[,] dbConnections, int[] purposes, int[,] compared,
            out int[,] fullCompared)
        {
            //accurateCompared = new int[inConnections.GetLength(0)];
            fullCompared = new int[compared.GetLength(0),compared.GetLength(1)];
            int[] activity1 = new int[inConnections.GetLength(0)];
            int[] activity2 = new int[dbConnections.GetLength(0)];
            //выбираем базовую точку (совпавшая и субъект)
            for (int i = 0; i < compared.GetLength(0); i++)
            {
                var finded = false;
                for (int j = 0; j < compared.GetLength(1); j++)
                {
                    if ((compared[i, j] == 1)&&(purposes[i] == 1))
                    {
                        activity1[i] = 1;
                        activity2[j] = 1;
                        finded = true;
                        break;
                    }
                }
                if (finded)
                    break;
            }
            //выполнять пока есть активные элементы
            while (CountOf(activity1, 1) > 0)
            {
                //для всех активных из массива1
                for (int k = 0; k < activity1.GetLength(0); k++)
                {
                    if (activity1[k] == 1)
                    {
                        //для всех совпавших 
                        var founded = false;
                        for (int l = 0; l < compared.GetLength(1); l++)
                        {
                            //если есть совпадение и элемент во втором массиве активен
                            if ((compared[k, l] == 1) && (activity2[l] == 1))
                            {
                                activity1[k] = 5; //на проверке
                                activity2[l] = 5; //на проверке
                                //активируем вторые субъекты в отношениях
                                for (int m = 0; m < inConnections.GetLength(0); m++)
                                {
                                    if ((inConnections[k, m] == 1) && (activity1[m] != 2) && (activity1[m] != 3))
                                    {
                                        activity1[m] = 6;
                                    }
                                }
                                for (int m = 0; m < dbConnections.GetLength(0); m++)
                                {
                                    if ((dbConnections[l, m] == 1) && (activity2[m] != 2) && (activity2[m] != 3))
                                    {
                                        activity2[m] = 6;
                                    }
                                }
                                //подтверждаем совпадение вторых субъектов
                                int idIn = -1, idDb = -1;
                                for (int i = 0; i < activity1.Length; i++)
                                {
                                    if (activity1[i] == 6)
                                    {
                                        for (int j = 0; j < compared.GetLength(1); j++)
                                        {
                                            if ((compared[i, j] == 1) && (activity2[j] == 6))
                                            {
                                                //подтверждаем совпадение связанного
                                                idIn = i;
                                                idDb = j;
                                                //переходим по связям связанного
                                                for (int m = 0; m < inConnections.GetLength(0); m++)
                                                {
                                                    if ((inConnections[i, m] == 1) && (activity1[m] != 2) && (activity1[m] != 3))
                                                    {
                                                        activity1[m] = 1;
                                                    }
                                                }
                                                for (int m = 0; m < dbConnections.GetLength(0); m++)
                                                {
                                                    if ((dbConnections[j, m] == 1) && (activity2[m] != 2) && (activity2[m] != 3))
                                                    {
                                                        activity2[m] = 1;
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                }
                                if ((idIn != -1) && (idDb != -1))
                                {
                                    activity1[k] = 2;
                                    activity2[l] = 2;
                                    activity1[idIn] = 2;
                                    activity2[idDb] = 2;
                                    fullCompared[k, l] = 2;
                                    fullCompared[idIn, idDb] = 2;
                                }
                            }
                        }
                        //если не найдено
                        if (!founded)
                            activity1[k] = 3;
                    }
                }
            }
            
            //чистить активные элементы если не найдено
        }

        private int CountOf(int[] inArray, int checkValue)
        {
            int countOf = 0;
            for (int i = 0; i < inArray.Length; i++)
            {
                if (inArray[i] == checkValue)
                    countOf++;
            }
            return countOf;
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