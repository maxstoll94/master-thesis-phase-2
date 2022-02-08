using CinemaSeaterLogic.MIS;
using CinemaSeaterLogic.Models;
using Gurobi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaSeaterLogic.ILPS
{
    public class StartingSeatVariable
    {
        public int Count { get; set; }
        public int Index { get; set; }
        public int Weight { get; set; }
        public int RowIndex { get; set; }
        public int GroupIndex { get; set; }
        public int GroupSize { get; set; }
        public GRBVar Variable { get; set; }
    }

    public class CinemaSeaterILP
    {
        public static (string, int) Solve(IEnumerable<int> startingSeats,
            IDictionary<int, int> seatingWeights, IEnumerable<int> groups,
            Graph graph, ILPSettings settings)
        {
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

                var variables = AddBinaryVariables(model, startingSeats, groups, seatingWeights);

                NoSeatOccupiedTwiceConstraint(model, variables, startingSeats);

                AddGroupConstraints(model, variables, groups.Count());

                AddDistanceConstraints(model, graph, variables);

                AddObjective(model, variables);

                var optimizeTime = Utils.TimeAction(() => model.Optimize());

                foreach (var seatVariables in variables)
                {
                    var variable = seatVariables.Variable;

                    if (variable.X == 1)
                    {
                        for (int i = 0; i < seatVariables.GroupSize; i++)
                        {
                            graph.SetLabel(seatVariables.Index + i, seatVariables.GroupSize.ToString());
                        }
                    }
                }

                //// Dispose of model and env
                model.Dispose();
                env.Dispose();

                return (optimizeTime, variables.Count());
            }
            catch (GRBException e)
            {
                if (e.ErrorCode == 10001)
                {
                    throw new OutOfMemoryException();
                }

                return (null, 0);
            }
        }

        private static void AddObjective(GRBModel model, IEnumerable<StartingSeatVariable> seatingVariables)
        {
            var expr = new GRBLinExpr();

            foreach (var seatingVariable in seatingVariables)
            {
                var variable = seatingVariable.Variable;
                var groupSize = seatingVariable.GroupSize;
                expr.AddTerm(groupSize, variable);
            }

            model.SetObjective(expr, GRB.MAXIMIZE);
        }

        private static IEnumerable<StartingSeatVariable> AddBinaryVariables(GRBModel model,
            IEnumerable<int> startingSeats, IEnumerable<int> groups,
            IDictionary<int, int> seatingWeights)
        {
            var variables = new List<StartingSeatVariable>();
            var numberOfRows = startingSeats.Count();
            var numberOfGroups = groups.Count();

            var numberOfStartingSeats = startingSeats.Count();

            for (int s = 0; s < numberOfStartingSeats; s++)
            {
                var startingSeat = startingSeats.ElementAt(s);
                var seatWeight = seatingWeights[startingSeat];
                var selectedGroups = groups.Where(g => g <= seatWeight);

                for (int g = 0; g < selectedGroups.Count(); g++)
                {     
                    var binaryVariable = model.AddVar(0.0, 1.0, 0.0, GRB.BINARY, $"seated_{startingSeat}_{g}");
                    var startingSeatVariable = new StartingSeatVariable
                    {
                        GroupSize = groups.ElementAt(g),
                        Index = startingSeat,
                        Variable = binaryVariable,
                        Weight = seatWeight,
                        GroupIndex = g
                    };

                    variables.Add(startingSeatVariable);
                }
            }

            return variables;
        }

        private static void NoSeatOccupiedTwiceConstraint(GRBModel model, IEnumerable<StartingSeatVariable> seatingVariables, IEnumerable<int> startingSeats)
        {
            foreach (var startingSeat in startingSeats)
            {
                var selectedVariables = seatingVariables.Where(s => s.Index == startingSeat);

                var expr = new GRBLinExpr();

                foreach (var selectedVariable in selectedVariables)
                {
                    expr.Add(selectedVariable.Variable);
                }

                model.AddConstr(expr, GRB.LESS_EQUAL, 1, $"No_seat_occupied_twice_{startingSeat}");
            }
        }

        private static void AddGroupConstraints(GRBModel model, IEnumerable<StartingSeatVariable> seatingVariables, int numberOfGroups)
        {
            for (int g = 0; g < numberOfGroups; g++)
            {
                var selectedVariables = seatingVariables.Where(s => s.GroupIndex == g);

                var expr = new GRBLinExpr();

                foreach (var selectedVariable in selectedVariables)
                {
                    expr.Add(selectedVariable.Variable);
                }

                model.AddConstr(expr, GRB.LESS_EQUAL, 1, $"GroupConstraint");
            }
        }

        private static void AddDistanceConstraints(GRBModel model, Graph graph, IEnumerable<StartingSeatVariable> seatingVariables)
        {
            ConcurrentBag<GRBLinExpr> constraints = new ConcurrentBag<GRBLinExpr>();

            Parallel.ForEach(seatingVariables, seatingVariable =>
            {
                {
                    var selectedVertices = Enumerable.Range(seatingVariable.Index, seatingVariable.GroupSize);

                    var invalidVertices = graph.GetAdjacentVertices(selectedVertices);

                    foreach (var invalidVertex in invalidVertices)
                    {
                        var invalidVariables = seatingVariables.Where(v => v.Index == invalidVertex);

                        foreach (var invalidVariable in invalidVariables)
                        {
                            if (invalidVariable.Index != seatingVariable.Index)
                            {
                                constraints.Add(new GRBLinExpr(seatingVariable.Variable + invalidVariable.Variable));
                            }
                        }
                    }
                }
            });

            foreach (var constraint in constraints)
            {
                model.AddConstr(constraint, GRB.LESS_EQUAL, 1, $"distance_constraint");
            }
        }      
    }
}
