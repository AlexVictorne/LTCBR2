namespace LTCBR2.Types.TypesForGpu
{
    public class SituationGpu
    {
        public int Id { get; set; }
        /// <summary>
        /// N - number of parties and 3 as party ID/type/purpose
        /// </summary>
        public int[,] Participants { get; set; }
        /// <summary>
        /// N1 - number of new parties, N2 - max allowed number of attributes and 2 as attribute name/value
        /// </summary>
        public int[,,] Attributes { get; set; }
        /// <summary>
        /// N1*N2 - N1,N2 - count of participants
        /// </summary>
        public int[,] Connections { get; set; }

        public int[] Purposes { get; set; }
    }
}