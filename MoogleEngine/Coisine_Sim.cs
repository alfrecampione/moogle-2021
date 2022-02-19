using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoogleEngine
{
    public static class Coisine_Sim
    {
        private static float Magnitude(Vector v1)
        {
            return (float)Math.Sqrt(v1 * v1);
        }
        private static float CoisineSimilarity(Vector general, Vector query)
        {
            if ((Magnitude(general) * Magnitude(query)) == 0)
                return 0;
            return (general * query) / (Magnitude(general) * Magnitude(query));
        }
        public static Dictionary<int, float> GetCoisineSim(float[] query_array, float[][] documents_matrix)
        {
            List<Vector> document_vectors = new List<Vector>();
            foreach (var array in documents_matrix)
            {
                document_vectors.Add(new Vector(array));
            }
            Vector query_vector = new(query_array);
            Dictionary<int, float> coisine_sim = new();
            for (int i = 0; i < document_vectors.Count; i++)
            {
                coisine_sim.Add(i, CoisineSimilarity(document_vectors[i], query_vector));
            }
            return coisine_sim;
        }
    }
}
