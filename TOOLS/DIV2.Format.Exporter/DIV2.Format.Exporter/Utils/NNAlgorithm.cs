using System;
using System.Collections.Generic;

namespace DIV2.Format.Exporter.Utils
{
    /// <summary>
    /// Nearest Neighbour algorithm.
    /// </summary>
    /// <remarks>Description: https://en.wikipedia.org/wiki/Nearest_neighbour_algorithm </remarks>
    public static class NNAlgorithm
    {
        #region Structs
        struct NNVector
        {
            #region Public vars
            public readonly int index;
            public readonly float x;
            public readonly float y;
            public readonly float z;
            public bool isVisited;
            #endregion

            #region Constructor
            public NNVector(int index, Tuple<float, float, float> xyz)
            {
                this.index = index;

                this.x = xyz.Item1;
                this.y = xyz.Item2;
                this.z = xyz.Item3;

                this.isVisited = false;
            }
            #endregion

            #region Methods & Functions
            public static float Distance(NNVector a, NNVector b)
            {
                float diff_x = a.x - b.x;
                float diff_y = a.y - b.y;
                float diff_z = a.z - b.z;

                return MathF.Sqrt(diff_x * diff_x +
                                  diff_y * diff_y +
                                  diff_z * diff_z);
            }
            #endregion
        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Calculates the best path from a list of vectors (x, y, z).
        /// </summary>
        /// <param name="input"><see cref="Tuple"/> of 3 <see cref="float"/> values that contains the vectors to calculate the best path.</param>
        /// <param name="start">Index of the start vector of the path.</param>
        /// <param name="cost">Returns the cost of the path.</param>
        /// <returns>Returns a <see cref="List{T}"/> of <see cref="int"/> with the sorted indexes.</returns>
        public static List<int> CalculatePath(List<Tuple<float, float, float>> input, int start, out float cost)
        {
            var nnVectors = new NNVector[input.Count];
            int current = start;
            var path = new List<int>();
            int visited = 0;

            Action<int> setVisited = (i) =>
            {
                nnVectors[i].isVisited = true;
                visited++;
            };

            Func<int, Tuple<int, float>> getNext = (s) =>
            {
                var ret = new Tuple<int, float>(s, float.NaN);

                for (int i = 0; i < nnVectors.Length; i++)
                {
                    if (!nnVectors[i].isVisited)
                    {
                        float d = NNVector.Distance(nnVectors[s], nnVectors[i]);
                        if (float.IsNaN(ret.Item2) || d < ret.Item2)
                            ret = new Tuple<int, float>(i, d);
                    }
                }

                setVisited(ret.Item1);

                return ret;
            };

            for (int i = 0; i < input.Count; i++)
                nnVectors[i] = new NNVector(i, input[i]);

            cost = 0;
            setVisited(current);
            path.Add(current);

            do
            {
                Tuple<int, float> next = getNext(current);
                current = next.Item1;
                cost += next.Item2;
                path.Add(current);

            } while (visited < nnVectors.Length);

            return path;
        }
        #endregion
    }
}
