namespace GraphUtils;

// Constains Graph Extensions.
public static class GraphExtensions {

    // Runs the Warshall Algorithim.
    public static int?[,] Warshall(this Graph ThisGraph) {
        // Create Matrix of Sizes.
        int?[,] Matrix = new int?[ThisGraph.Size, ThisGraph.Size];

        // X and Y.
        int x = 0, y = 0;

        // Get all Nodes.
        List<Node> AllNodes = ThisGraph.GetAllNodes();

        // Loop all Connections...
        foreach (var Conns in ThisGraph.Connections.Values) {
            // Loop all Connections
            foreach (var Conn in Conns) {
                // Calculate X.
                for (int i = 0; i < AllNodes.Count; i++) {
                    // If it is different, ignore.
                    if (Conn.Key != AllNodes[i])
                        continue;

                    // Set X.
                    x = i;

                    // Break.
                    break;
                }

                // Set X and Y in Matrix.
                Matrix[x,y] = ((Connection<int>)Conn.Value).Value;
            }

            // Increment Y.
            y++;
        }

        // Actually Implement Marshall.
        for (int k = 0; k < AllNodes.Count; k++) {
            // Second Loop...
            for (int i = 0; i < AllNodes.Count; i++) {
                // Third Loop...
                for (int j = 0; j < AllNodes.Count; j++) {
                    // Get Values.
                    int? Val1 = Matrix[i,j] != null ? Matrix[i,j] : int.MaxValue;
                    int? Val2 = Matrix[i,k] != null ? Matrix[i,k] : int.MaxValue;
                    int? Val3 = Matrix[k,j] != null ? Matrix[k,j] : int.MaxValue;

                    // Set Value.
                    Matrix[i,j] = Math.Min(Val1!.Value, Val2!.Value + Val3!.Value);
                }
            }
        }

        // Return Matrix.
        return Matrix;
    }

    // THe Dijkstra Algorithim.
    public static int Dijkstra(this Graph ThisGraph, Node StartPoint, Node EndPoint, out List<Node> Path, bool Inversed, List<Node>? Ignore) {
        // Our Nodes to Visit.
        HashSet<Node> ToVisit = new HashSet<Node>();

        // Our Distances and Predecessors Hash Maps.
        Dictionary<Node, int> Distances = new Dictionary<Node, int>();
        Dictionary<Node, Node?> Predecessors = new Dictionary<Node, Node?>();

        // For Each Node in the Graph...
        foreach (var Node in ThisGraph.Connections.Keys) {
            // Ignore node if it is on list.
            if (Ignore != null && Ignore.Contains(Node))
                continue;

            // Add on Distances.
            Distances.Add(Node, int.MaxValue);

            // Add to Predecessors.
            Predecessors.Add(Node, null);

            // Add To Visit.
            ToVisit.Add(Node);
        }

        // Our start point has 0 distance.
        Distances[StartPoint] = 0;

        // While our set is not empty....
        while (ToVisit.Count > 0) {
            // This will be the Current Node.
            Node? Cur = null;

            // The Length of the Current Node.
            int Length = int.MaxValue;

            // For Each Node in Set...
            foreach (Node Node in ToVisit) {
                // Check for Current Node...
                if (Cur == null) {
                    // It is null, set this as Current Node.
                    Cur = Node;
                    Length = Distances[Cur];

                    // Go to next iteration.
                    continue;
                }

                // Check for distance.
                if (Distances[Node] >= Length)
                    continue;

                // Distance is less, set current node.
                Cur = Node;
                Length = Distances[Node];
            }

            // Remove Current from Visit.
            ToVisit.Remove(Cur);

            // Check for Destination.
            if (Cur == EndPoint)
                break;

            // Loop all Connected Nodes...
            foreach (var Conn in ThisGraph.Connections[Cur]) {
                // Ignore nodes we already visited.
                if (!ToVisit.Contains(Conn.Key))
                    continue;

                // Calculate Distance.
                int dst = Distances[Cur] + (Inversed ? (1 / Conn.Value.Weight) : Conn.Value.Weight);

                // If the Distance is greater or equals to the target distance, ignore...
                if (dst >= Distances[Conn.Key])
                    continue;

                // Set Distances and Predecessors.
                Distances[Conn.Key] = dst;
                Predecessors[Conn.Key] = Cur;
            }
        }

        // The Total Distance.
        int Distance = 0;

        // The Current Graph we are Iterating is the End Point.
        Node? Previous = null;
        Node? Current = EndPoint;

        // Create Path.
        Path = new List<Node>();

        // While we have a current node...
        while (Current != null) {
            // Insert at the start of path.
            if (Path != null)
                Path.Insert(0, Current);

            // Get the Connection and add the Distance.
            if (Previous != null)
                Distance += ThisGraph.GetConnectionWeight(Current, Previous);

            // Go to predecessor.
            Previous = Current;
            Current = Predecessors[Current];
        }

        // Return Distance.
        return Distance;
    }

    // Finds Vertices in Distance.
    private static void InDistance(List<Node> Results, Graph G, Node Current, int Distance) {
        // Check for End Distance.
        if (Distance == 0) {
            // Add Result if it isn't there.
            if (!Results.Contains(Current))
                Results.Add(Current);

            // Stop Running.
            return;
        }

        // Get all connections...
        G.GetConnections(Current, out List<Node> Connections);

        // For Each Connection, recursive.
        foreach (var Conn in Connections)
            InDistance(Results, G, Conn, Distance - 1);
    }

    // In Distance Function.
    public static List<Node> InDistance(this Graph G, Node Node, int Distance) {
        // Create Results Array.
        List<Node> Results = new List<Node>();

        // Call In Distance.
        InDistance(Results, G, Node, Distance);

        // Return Results.
        return Results;
    }

    // Searches in Depth.
    private static bool DepthSearch(List<Node> Visited, List<Node> Path, Graph G, Node Current, Node Target, int? Limit = null) {
        // Check for Visited or Limit.
        if (Visited.Contains(Current) || (Limit.HasValue && Limit <= 0))
            return false;

        // Add to Visisted.
        Visited.Add(Current);

        // Add Current to Path.
        Path.Add(Current);

        // Check for Target.
        if (Current != Target) {
            // Get all connetions...
            G.GetConnections(Current, out List<Node> Connections);

            // Decrease Limit.
            if (Limit.HasValue)
                Limit--;

            // For Each connection, recursive.
            foreach (var Conn in Connections) {
                // Call Depth Search.
                if (DepthSearch(Visited, Path, G, Conn, Target, Limit))
                    return true;
            }

            // Remove from Path.
            Path.Remove(Current);

            // Return Failure.
            return false;
        }

        // Return true.
        return true;
    }

    // Searches in Depth.
    public static bool DepthSearch(this Graph G, Node From, Node Target, out List<Node> Path, int? Limit = null) {
        // Create Path.
        Path = new List<Node>();

        // Return the Depth Search.
        return DepthSearch(new List<Node>(), Path, G, From, Target, Limit);
    }

    // Searches with Iterative Deepening. Since this algorithim can run idefinately, it is required to give a Limit in Height to consider a failure to find a path.
    public static bool IterativeDeepening(this Graph G, Node From, Node Target, out List<Node> Path, int Limit = 5) {
        // The Current Iteration.
        int Current = 0;

        // Loop until...
        while (true) {
            // Increase Current.
            Current++;

            // If we hit our limit, we stop the algorithim.
            if (Current > Limit) {
                // Return Empty Path.
                Path = new List<Node>();

                // Return false.
                return false;
            }

            // Otherwise, make a Depth Search based on Current Limit.
            if (G.DepthSearch(From, Target, out Path, Current))
                return true;

            // If the Depth Search failed, we continue to next level.
        }
    }

    // Searches using Heuristic Values.
    private static bool HeuristicSearch(List<Node> Visited, List<Node> Path, Graph G, Node Current, Node Target, Dictionary<Node, int> HeuristicValues, bool UseWeights = false, int? Limit = null) {
        // Check for Visited or Limit.
        if (Visited.Contains(Current) || (Limit.HasValue && Limit <= 0))
            return false;

        // Add to Visisted.
        Visited.Add(Current);

        // Add Current to Path.
        Path.Add(Current);

        // Check for Target.
        if (Current != Target) {
            // Get all connetions...
            G.GetConnections(Current, out List<Node> Connections);

            // Decrease Limit.
            if (Limit.HasValue)
                Limit--;

            // To Grab our Target Node...
            Node GoTo = Connections
                .Select(x => Tuple.Create(x, HeuristicValues[x] + (UseWeights ? G.GetConnection(Current, x)!.Weight : 0))) // We first Create Tuples based on the Node itself and their Heuristic Values.
                                                                                                                     // If we have Use A, we will increment the connection weight to it.
                .OrderBy(x => x.Item2) // We order by the Heuristic Values (ascending).
                .First().Item1; // We take the first item on our list 

            // Call Heuristic Search.
            if (HeuristicSearch(Visited, Path, G, GoTo, Target, HeuristicValues, UseWeights, Limit))
                return true;

            // Remove from Path. (We backtrack, but in theory this shouldn't happen if we setup our heuristic values correctly)
            Path.Remove(Current);

            // Return Failure.
            return false;
        }

        // Return true.
        return true;
    }

    // Searches using Heuristic Values.
    public static bool HeuristicSearch(this Graph G, Node From, Node Target, Dictionary<Node, int> HeuristicValues, out List<Node> Path, bool UseWeights = false, int? Limit = null) {
        // Create Path.
        Path = new List<Node>();

        // Return the Depth Search.
        return HeuristicSearch(new List<Node>(), Path, G, From, Target, HeuristicValues, UseWeights, Limit);
    }

    // Searches with Iterative Deepening with Heuristic Values. Since this algorithim can run idefinately, it is required to give a Limit in Height to consider a failure to find a path.
    public static bool HeuristicIterativeDeepening(this Graph G, Node From, Node Target, Dictionary<Node, int> HeuristicValues, out List<Node> Path, int Limit = 5, bool UseWeights = false) {
        // The Current Iteration.
        int Current = 0;

        // Loop until...
        while (true) {
            // Increase Current.
            Current++;

            // If we hit our limit, we stop the algorithim.
            if (Current > Limit) {
                // Return Empty Path.
                Path = new List<Node>();

                // Return false.
                return false;
            }

            // Otherwise, make a Heuristic Search based on Current Limit.
            if (G.HeuristicSearch(From, Target, HeuristicValues, out Path, UseWeights, Current))
                return true;

            // If the Heuristic Search failed, we continue to next level.
        }
    }

    // Breadth Search.
    public static void BreadthSearch(this Graph G, Node From, Node To, out List<Node> Result) {
        // Create Result.
        Result = new List<Node>();

        // Create Visited and Queue.
        HashSet<Node> Visisted = new HashSet<Node>();
        Queue<Node> Queue = new Queue<Node>();

        // Add From to Visisted and Queue.
        Visisted.Add(From);
        Queue.Enqueue(From);

        // While we have a queue.
        while (Queue.Count > 0) {
            // Get Current Node.
            Node Current = Queue.Dequeue();

            // Add it to Results.
            Result.Add(Current);

            // If it is identical, stop search.
            if (Current == To)
                return;

            // Get Adjacency List.
            G.GetConnections(Current, out List<Node> AdjLst);

            // For Each Connected Node...
            foreach (var Next in AdjLst) {
                // If we have been visited, ignore...
                if (Visisted.Contains(Next))
                    continue;

                // Otherwise, mark as visited and add to queue.
                Visisted.Add(Next);
                Queue.Enqueue(Next);
            }
        }
    }

    // Gets the Lowest Connection from all Connections.
    private static Connection? GetLowestConnection(List<Connection> All, List<Connection> Visited, Node Current, List<Node> Ignore) {
        // Initialize Connections.
        int Weight = int.MaxValue;
        Connection? Result = null;

        // For Each Connection...
        foreach (var Conn in All) {
            // Check if it was visited.
            if (Visited != null && Visited.Contains(Conn))
                continue;

            // Check its weight.
            if (Conn.Weight >= Weight)
                continue;

            // Get the Other Graph Node.
            Node? Other = Conn.NodeA == Current ? Conn.NodeB : Conn.NodeA;

            // If Target Node Contains, continue.
            if (Ignore.Contains(Other))
                continue;

            // Set Data.
            Weight = Conn.Weight;
            Result = Conn;
        }

        // Return Result.
        return Result;
    }

    // PRIM Algorithim.
    public static int PRIM(this Graph G, Node From, List<Node> Result) {
        // Make Sure Result is Cleared.
        Result.Clear();

        // Add From to Results Array.
        Result.Add(From);

        // The List of Current Connections and the Visited Ones.
        G.GetConnectionsAsConn(From, out List<Connection> CurrentConnections);
        List<Connection> VisistedConnections = new List<Connection>();

        // Get the Current Connection.
        Connection? Current = GetLowestConnection(CurrentConnections, VisistedConnections, From, Result);

        // All Weight.
        int AllWeight = 0;

        // While we've got a current.
        while (Result.Count < G.Size) {
            // Get Current Node.
            Node CurrentNode = Result.Contains(Current!.NodeA) ? Current.NodeB : Current.NodeA;

            // Make sure our Node is not on the Result array. (if it is, our iteration has finished)
            if (Result.Contains(CurrentNode))
                return AllWeight;

            // Add to Result.
            Result.Add(CurrentNode);

            // Add Weight.
            AllWeight += Current.Weight;

            // Mark Visited.
            VisistedConnections.Add(Current);

            // Create Next Connections Array.
            G.GetConnectionsAsConn(CurrentNode, out List<Connection> NextConnections);

            // Add the Connections to our Find Array.
            foreach (var Conn in NextConnections)
                if (!CurrentConnections.Contains(Conn))
                    CurrentConnections.Add(Conn);

            // Get the Lowest Connection.
            Current = GetLowestConnection(CurrentConnections, VisistedConnections, CurrentNode, Result);
        }

        // Return All Weight.
        return AllWeight;
    }

    // Finds a Cycle on the Graph.
    private static bool FindCycle(Node Current, Node Target, List<Connection> Visited, List<Connection> Path) {
        // If we've reached our target, we have a CYCLE!
        if (Current == Target)
            return true;

        // For each of our Visited connections...
        foreach (var Conn in Visited) {
            // If we made a path, make sure we don't search it again.
            if (Path.Contains(Conn))
                continue;

            // If we aren't the current node we are searching, ignore...
            if (Conn.NodeA != Current && Conn.NodeB != Current)
                continue;

            // Add this Connection to the Path.
            Path.Add(Conn);

            // Get the New Current Node.
            Node NewCurrent = Conn.NodeA == Current ? Conn.NodeB : Conn.NodeA;

            // Recursive Call.
            if (FindCycle(NewCurrent, Target, Visited, Path))
                return true;

            // We didn't find any cycles on this iteration, backtrack.
            Path.Remove(Conn);
        }

        // No Cycles detected!
        return false;
    }

    // Finds a Cycle on the Graph.
    public static bool FindCycle(Node Current, Node Target, List<Connection> Visited) {
        return FindCycle(Current, Target, Visited, new List<Connection>());
    }

    // The Kruskal Algorithim.
    public static int Kruskal(Graph G, List<Connection> Result) {
        // First, clear our results array.
        Result.Clear();

        // Create our Connections and Visisted Array.
        List<Connection> AllConnections = G.GetAllConnections();

        // Sort our Connections Array.
        AllConnections.Sort();

        // Initialize Weight Result.
        int AllWeight = 0;

        // For Each Connection...
        foreach (var Current in AllConnections) {
            // If we found a cycle with all the result connections, continue...
            if (FindCycle(Current.NodeA, Current.NodeB, Result))
                continue;

            // Add Connection to result.
            Result.Add(Current);

            // Add its weight.
            AllWeight += Current.Weight;
        }

        // Return Weight.
        return AllWeight;
    }

    // Prints an Array of Nodes.
    public static void PrintArrayOfNodes(List<Node> Nodes, string Prefix) {
        // Give it a Separator.
        Console.WriteLine();

        // Let's print the Nodes.
        Console.Write($"{Prefix}: ");

        // For Each Node in Path.
        foreach (var Node in Nodes)
            Console.Write($"{Node.Label}, ");

        // Stop the Nodes.
        Console.WriteLine();
    }

    // Prints an Array of Connections.
    public static void PrintArrayOfConnections(List<Connection> Connections, string Prefix) {
        // Give it a Separator.
        Console.WriteLine();

        // Let's print the Connections.
        Console.Write($"{Prefix}: ");

        // For Each Connection in Path.
        foreach (var Conn in Connections)
            Console.Write($"[{Conn.NodeA.Label} - {Conn.NodeB.Label}] ({Conn.Weight}), ");

        // Stop the Connections.
        Console.WriteLine();
    }

    // Saves the Graph to a File.
    public static void SaveToFile(this Graph G, string Path) {
        // Remove File if exists.
        if (File.Exists(Path))
            File.Delete(Path);

        // Write File.
        File.WriteAllText(Path, G.ToString());
    }

    // Creates a Graph from a File.
    public static Graph FromSavedFile(string Path) {
        // Return new Graph From the File String.
        return Graph.FromString(File.ReadAllText(Path));
    }

    // If a Graph is Strongly Connected (Conexo).
    public static bool IsStronglyConnected(this Graph G) {
        // For Each Node...
        foreach (var Node in G.Connections.Keys) {
            // Loop all other nodes...
            foreach (var Other in G.Connections.Keys) {
                // Ignore identical nodes.
                if (Node == Other)
                    continue;

                // Apply Dijkstra.
                Dijkstra(G, Node, Other, out List<Node> Path, false, null);

                // Check for Result Path.
                if (Path.Count <= 0)
                    return false;
            }
        }

        // Return Result.
        return true;
    }

    // If a Graph is Eurelian.
    public static bool IsEurelian(this Graph G) {
        // First, we get the array of all nodes.
        List<Node> Nodes = G.GetAllNodes();

        // We make find a Cycle that contains all nodes, so...
        foreach (var Node in Nodes) {
            // Get all Connections...
            G.GetConnectionsAsConn(Node, out List<Connection> Connections);

            // For Each Connection...
            foreach (var Conn in Connections) {
                // Create Visited Array.
                List<Connection> Visited = new List<Connection>();

                // Add This Connection to Visited.
                Visited.Add(Conn);

                // Create Path Array.
                List<Connection> Path = new List<Connection>();

                // Find the Cycle, if we don't then, continue...
                if (!FindCycle(Node, Conn.NodeA == Node ? Conn.NodeB : Conn.NodeA, Connections, Path))
                    continue;

                // Convert Path to Set.
                HashSet<Node> PathNodes = new HashSet<Node>();

                // For Each Connection...
                foreach (var Connec in Path) {
                    // Add Nodes A and B.
                    PathNodes.Add(Connec.NodeA);
                    PathNodes.Add(Connec.NodeB);
                }

                // If it contains all.
                bool ContainsAll = true;

                // Let's check if all nodes are present on our Cycle.
                foreach (var Other in Nodes) {
                    // If contains, continue...
                    if (PathNodes.Contains(Other))
                        continue;

                    // If it doesn't, set to false...
                    ContainsAll = false;

                    // And break.
                    break;
                }

                // If Contains All is True, return it.
                if (ContainsAll)
                    return true;

                // Otherwise, try to find another cycle.
            }
        }

        // Return Result.
        return false;
    }

    // If a Graph is Cyclic.
    public static bool IsCyclic(this Graph G) {
        // Loop all Nodes...
        foreach (var Node in G.GetAllNodes()) {
            // Get all Connections.
            G.GetConnectionsAsConn(Node, out List<Connection> Connections);

            // For Each Connection...
            foreach (var Conn in Connections) {
                // Create Visited Array.
                List<Connection> Visited = new List<Connection>();

                // Add this Connection to Visited.
                Visited.Add(Conn);

                // Find Cycle, if we do find a cycle, tis finished!
                if (FindCycle(Node, Conn.NodeA == Node ? Conn.NodeB : Conn.NodeA, Visited))
                    return true;
            }
        }

        // Return false.
        return false;
    }

    // Calculates the Closeness Centrality of a Node on a Graph, with an Nodes Filter.
    public static float ClosenessCentrality(this Graph G, Node Node) {
        // The Distances, sum.
        int Distances = 0;

        // For Each Nodes in Graph...
        foreach (var Other in G.GetAllNodes()) {
            // If same node, ignore...
            if (Node == Other)
                continue;

            // Distances summed with a Dijkstra.
            Distances += Dijkstra(G, Node, Other, out List<Node> Path, false, null);
        }

        // Check for Distances.
        if (Distances <= 0)
            return 0f;

        // Return the 1 / Distances.
        return 1f / Distances;
    }

    // Calculates the Betweenness Centrality of a Node in a Graph.
    public static float BetweennessCentrality(this Graph G, Node Node) {
        // Total Paths and Paths that Succeed.
        int TotalPaths = 0, SucceedPaths = 0;

        // For Each Vertices...
        foreach (var From in G.GetAllNodes()) {
            // Ignore if From is our Node.
            if (From == Node)
                continue;

            // Aaaand again...
            foreach (var To in G.GetAllNodes()) {
                // Ignore if To is our Node.
                if (To == From)
                    continue;

                // Dijkstra.
                Dijkstra(G, From, To, out List<Node> Path, false, null);

                // If no Path, ignore.
                if (Path.Count <= 0)
                    continue;

                // Add to Total Paths.
                TotalPaths++;

                // If the Path Contains our Node, add to SucceedPaths.
                if (Path.Contains(Node))
                    SucceedPaths++;
            }
        }

        // Check for Total Paths.
        if (TotalPaths == 0)
            return 0f;

        // Return Coefficient.
        return SucceedPaths / (float)TotalPaths;
    }

    // Creates a Random Graph.
    public static Graph CreateRandom(int Vertices, int Connections, bool StronglyConnected, int Seed) {
        // The Max Random Weight of a Connection.
        int MaxWeight = 32;

        // Create Random.
        Random rand = new Random(Seed);

        // Create Graph.
        Graph G = new Graph();

        // Array of Nodes.
        List<Node> Nodes = new List<Node>();

        // For Each Vertex...
        for (int v = 0; v < Vertices; v++) {
            // Create new Node.
            Node NewNode = new Node($"Node {v + 1}");

            // Add to Graph.
            G.AddNode(NewNode);

            // Add to List.
            Nodes.Add(NewNode);
        }

        // Check for Strongly Connected.
        if (StronglyConnected) {
            // Loop all Nodes...
            foreach (var Node in Nodes) {
                // And again...
                foreach (var Other in Nodes) {
                    // Make sure they aren't identical.
                    if (Node == Other)
                        continue;

                    // Create Connection.
                    G.ConnectNode(Node, Other, new Connection("Generic Connection", Connection.EDirection.TwoWay, rand.Next(MaxWeight)));
                }
            }

            // All Used Connections.
            List<Connection> UsedConnections = new List<Connection>();

            // Apply the Kruskal Algorithm.
            Kruskal(G, UsedConnections);

            // All Graph Connections.
            List<Connection> AllConnections = G.GetAllConnections();

            // For Each Connection...
            foreach (var Conn in AllConnections) {
                // If we used this connection, ignore it.
                if (UsedConnections.Contains(Conn))
                    continue;

                // Remove it.
                G.DisconnectNode(Conn.NodeA, Conn.NodeB);
            }
        }

        // While we don't have enough connections...
        while (Connections > G.ConnectionAmount) {
            // Our Current Nodes.
            Node? NodeA = null, NodeB = null;

            // Forver, and ever!
            while (true) {
                // Get Random Node A.
                NodeA = Nodes[rand.Next(Nodes.Count)];
                NodeB = Nodes[rand.Next(Nodes.Count)];

                // Ignore if Nodes are Identical.
                if (NodeA == NodeB)
                    continue;

                // Find if they have a Connection.
                if (G.GetConnection(NodeA, NodeB) != null)
                    continue;

                // Stop the Loop, we found our Nodes.
                break;
            }

            // Connect Nodes.
            G.ConnectNode(NodeA, NodeB, new Connection("Generic Connection", Connection.EDirection.TwoWay, rand.Next(MaxWeight)));
        }

        // Check for Parameter issue.
        if (StronglyConnected && G.ConnectionAmount > Connections)
            throw new ArgumentException("Can't generate a strongly connected graph with the provided parameters as there would be more connections than the provided value.");

        // Return Graph.
        return G;
    }

    // Executes the Alpha Beta algorithim.
    private static int AlphaBeta(Graph G, Node<int> Current, int Depth, ref int Alpha, ref int Beta, bool Max) {
        // Check for Depth.
        if (Depth <= 0)
            return Current.V1;

        // Grab Connections that continue towards the end of thr graph.
        var Connections = G.GetConnections(Current, Connection.EDirection.A_to_B).ToList();

        // If no connections, return current heuristic value.
        if (Connections.Count <= 0)
            return Current.V1;

        // Start Val at specific position.
        int val = Max ? int.MinValue : int.MaxValue;

        // For Each connection...
        foreach (var conn in Connections) {
            // Cast connection.
            var conn_cst = conn as Node<int>;

            // If cast invalid, ignore node.
            if (conn_cst == null)
                continue;

            // Calculate alphabeta value.
            var alphabeta = AlphaBeta(G, conn_cst, Depth - 1, ref Alpha, ref Beta, !Max);

            // Calculate Value.
            val = Max ? Math.Max(val, alphabeta) : Math.Min(val, alphabeta);

            // Check for Max/Min.
            if (Max) {
                // Check for Beta Cutoff.
                if (val >= Beta)
                    break;

                // Update Alpha Value.
                Alpha = Math.Max(Alpha, val);
			} else {
                // Check for Alpha Cutoff.
                if (val <= Alpha)
                    break;

                // Update Beta Value.
                Beta = Math.Min(Beta, val);
			}
		}

        // Return Value.
        return val;
	}

    // Runs an Alpha/Beta algorithim searching for the best possible path to victory.
    public static Node<int>? AlphaBeta(Graph G, Node<int> StartNode, int Depth) {
        // Result Node.
        Node<int>? MaxNode = null;

        // The Current Result Max Value we are tracking.
        int MaxValue = int.MinValue;

        // Alpha/Beta Values for Cutoff.
        int Alpha = int.MinValue;
        int Beta = int.MaxValue;

        // Grab all connections... one way.
        var Conns = G.GetConnections(StartNode, Connection.EDirection.A_to_B).ToList();

        // For each connection...
        foreach (var conn in Conns) {
            // Cast to node type.
            var cnn_cst = conn as Node<int>;

            // If wrong node type, ignore.
            if (cnn_cst == null)
                continue;

            // Runs alpha beta, searching for all Min Values.
            int value = AlphaBeta(G, cnn_cst, Depth - 1, ref Alpha, ref Beta, false);

            // From the min value, we build the max value.
            if (MaxNode == null || MaxValue < value) {
                // Sets.
                MaxNode = cnn_cst;
                MaxValue = value;
			}
        }

        // Return the Node to run.
        return MaxNode;
    }
}