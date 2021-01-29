using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace DIV2.Format.Exporter.Utils
{
    /// <summary>
    /// Nearest Neighbour algorithm.
    /// </summary>
    /// <remarks>Algorithm description: https://en.wikipedia.org/wiki/Nearest_neighbour_algorithm </remarks>
    public static class NNAlgorithm
    {
        #region Structs
        struct NNVector
        {
            #region Public vars
            public readonly int index;
            public readonly Vector3 vector;
            public bool isVisited;
            #endregion

            #region Constructor
            public NNVector(int index, Vector3 vector)
            {
                this.index = index;
                this.vector = vector;
                this.isVisited = false;
            }
            #endregion

            #region Methods & Functions
            public static float Distance(NNVector a, NNVector b)
            {
                return Vector3.Distance(a.vector, b.vector);
            }
            #endregion
        }
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Calculates the best path from a list of <see cref="Vector3"/>.
        /// </summary>
        /// <param name="input"><see cref="Vector3"/> list that contains the vectors to calculate the best path.</param>
        /// <param name="start">Index of the start vector of the path.</param>
        /// <param name="cost">Returns the cost of the path.</param>
        /// <returns>Returns a <see cref="List{T}"/> of <see cref="int"/> with the sorted indexes.</returns>
        public static List<int> CalculatePath(IEnumerable<Vector3> input, int start, out float cost)
        {
            var vectors = input.Select((e, i) => new NNVector(i, e)).ToArray();
            int current = start;
            var path = new List<int>();
            int visited = 0;

            Action setCurrentVisited = () =>
            {
                vectors[current].isVisited = true;
                visited++;
            };

            Func<float> findNext = () =>
            {
                int local = current;
                float distance = float.NaN;

                for (int i = 0; i < vectors.Length; i++)
                    if (!vectors[i].isVisited)
                    {
                        float d = NNVector.Distance(vectors[current], vectors[i]);
                        if (float.IsNaN(distance) || d < distance)
                        {
                            local = i;
                            distance = d;
                        }
                    }

                current = local;
                path.Add(current);
                setCurrentVisited();

                return distance;
            };

            cost = 0f;
            path.Add(current);
            setCurrentVisited();

            do { cost += findNext(); } while (visited < vectors.Length);

            return path;
        }
        #endregion
    }
}
