using CinemaSeaterLogic.Models;
using Gurobi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CinemaSeaterLogic.MIS
{
    public class ILPSettings
    {
        public bool Tune { get; set; }
        public bool Debug { get; set; }
        public string TuneOutputFile { get; set; }
        public string ParamFile { get; set; }
    }

    public class ILPMISFinder
    {
        public IEnumerable<int> Find(Graph graph, ILPSettings settings = null)
        {
            if (settings == null)
            {
                settings = new ILPSettings();
            }

            var mis = new List<int>();

            try
            {
                // Create an empty environment, set options and start
                GRBEnv env = new GRBEnv(true);
                env.Start();

                if (settings.ParamFile != null)
                {
                    env.ReadParams(settings.ParamFile);
                }

                if (!settings.Debug)
                {
                    env.Set(GRB.IntParam.OutputFlag, 0);
                }

                // Create empty model
                GRBModel model = new GRBModel(env);

                (var vertices, var weights) = AddSeatedBinaryVariables(model, graph);

                AddContraints(model, graph, vertices);

                AddObjective(model, vertices, weights);

                var optimizeTime = Utils.TimeAction(() => model.Optimize());

                for (int v = 0; v < vertices.Length; v++)
                {
                    if (vertices[v].X == 1)
                    {
                        mis.Add(v);
                    }
                }

                if (settings.Tune)
                {
                    model.Tune();
                    model.GetTuneResult(0);
                    model.Write(settings.TuneOutputFile);
                }

                // Dispose of model and env
                model.Dispose();
                env.Dispose();
            }
            catch (GRBException e)
            {
                Console.WriteLine("Error code: " + e.ErrorCode + ". " + e.Message);
                throw e;
            }

            return mis;
        }

        private static void AddObjective(GRBModel model, GRBVar[] vertices, int[] weights)
        {
            var expr = new GRBLinExpr();

            for (int v = 0; v < vertices.Length; v++)
            {
                expr.AddTerm(weights[v], vertices[v]);
            }

            model.SetObjective(expr, GRB.MAXIMIZE);
        }

        private static (GRBVar[], int[]) AddSeatedBinaryVariables(GRBModel model, Graph graph)
        {
            var vertices = new GRBVar[graph.GetNumberOfVertices()];
            var weights = new int[graph.GetNumberOfVertices()];

            for (int v = 0; v < vertices.Length; v++)
            {
                weights[v] = 1;

                if (graph.GetLabel(v) == "s")
                {
                    weights[v] = -1;
                }

                vertices[v] = model.AddVar(0, 1, 0, GRB.BINARY, "vertex" + v);
            }

            return (vertices, weights);
        }

        private static void AddContraints(GRBModel model, Graph graph, GRBVar[] vertices)
        {
            ConcurrentBag<GRBLinExpr> constraints = new ConcurrentBag<GRBLinExpr>();

            Parallel.For(0, vertices.Length, v1 =>
            {
                for (int v2 = 0; v2 < vertices.Length; v2++)
                {
                    if (v1 != v2)
                    {
                        if (graph.GetEdgeWeight(v1, v2) > 0)
                        {
                            constraints.Add(new GRBLinExpr(vertices[v1] + vertices[v2]));
                        }
                    }
                }
            });

            foreach (var constraint in constraints)
            {
                model.AddConstr(constraint, GRB.LESS_EQUAL, 1, "No edge constaint");
            }
        }
    }
}
