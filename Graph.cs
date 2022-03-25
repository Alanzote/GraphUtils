using System.Text;

namespace GraphUtils;

// Our Graph.
public class Graph {

	// Our Connections.
	public Dictionary<Node, Dictionary<Node, Connection>> Connections { get; private set; } = new();
	
	// Adds a Node.
	public bool AddNode(Node NewNode) {
		// Check for Key.
		if (Connections.ContainsKey(NewNode))
			return false;

		// Add Node.
		Connections.Add(NewNode, new Dictionary<Node, Connection>());

		// Return true.
		return true;
	}

	// Removes a Node, with a reference.
	public bool RemoveNode(Node OldNode) {
		// Check for Key.
		if (!Connections.ContainsKey(OldNode))
			return false;

		// Remove from Connections.
		Connections.Remove(OldNode);

		// For Each other Entry...
		foreach (var Entry in Connections.Values) {
			// If the Key does not exist...
			if (!Entry.ContainsKey(OldNode))
				continue;

			// Remove it.
			Entry.Remove(OldNode);
		}

		// Return true.
		return true;
	}

	// Connects two Node, with a reference.
	public bool ConnectNode(Node NodeA, Node NodeB, Connection Conn) {
		// Check for Nodes.
		if (!Connections.ContainsKey(NodeA) || !Connections.ContainsKey(NodeB) || Connections[NodeA].ContainsKey(NodeB))
			return false;

		// Set Connection Properties.
		Conn.NodeA = NodeA;
		Conn.NodeB = NodeB;

		// Add Connections.
		Connections[NodeA].Add(NodeB, Conn);
		Connections[NodeB].Add(NodeA, Conn);

		// Return true.
		return true;
	}

	// Connects two Nodes, without the connection (created internally).
	public bool ConnectNode(Node NodeA, Node NodeB, int Weight) {
		// Return connect node with created Generic Connection.
		return ConnectNode(NodeA, NodeB, new Connection("Generic Connection", Connection.EDirection.TwoWay, Weight));
	}

	// Gets a connection, with a reference.
	public Connection? GetConnection(Node NodeA, Node NodeB) {
		// Check for Nodes.
		if (!Connections.ContainsKey(NodeA) || !Connections.ContainsKey(NodeB) || !Connections[NodeA].ContainsKey(NodeB))
			return null;

		// Return Connection.
		return Connections[NodeA][NodeB];
	}

	// Wrapper to get Connection weight, with a reference.
	public int GetConnectionWeight(Node NodeA, Node NodeB) {
		// Get Connection.
		Connection? Conn = GetConnection(NodeA, NodeB);

		// Check for Connection.
		if (Conn == null)
			return -1;

		// Return Weight.
		return Conn.Weight;
	}

	// Disconnects two Nodes, with references.
	public bool DisconnectNode(Node NodeA, Node NodeB) {
		// Check for Connection.
		if (!Connections.ContainsKey(NodeA) || !Connections.ContainsKey(NodeB) || !Connections[NodeA].ContainsKey(NodeB))
			return false;

		// Remove Connections.
		Connections[NodeA].Remove(NodeB);
		Connections[NodeB].Remove(NodeA);

		// Return true.
		return true;
	}

	// Gets the Connections of a Specific Node, with references.
	public int GetConnections(Node N, out List<Node> Return) {
		// Check for Connections.
		if (!Connections.ContainsKey(N)) {
			// Set Return to Empty List.
			Return = new List<Node>();

			// Return 0.
			return 0;
		}

		// Set Return.
		Return = Connections[N].Keys.ToList();

		// Return Count of Return.
		return Return.Count;
	}

	// Gets the Connections of a Specific Node, with connection references.
	public int GetConnectionsAsConn(Node N, out List<Connection> Return) {
		// Check for COnnections.
		if (!Connections.ContainsKey(N)) {
			// Set Return to Empty List.
			Return = new List<Connection>();

			// Return 0.
			return 0;
		}

		// Grab all Connections.
		Return = Connections[N].Values.ToList();

		// Return Count of Return.
		return Return.Count;
	}

	// Gets all Connections associated with this Graph.
	public List<Connection> GetAllConnections() {
		// Grabs the Connections.
		return Connections.Values.SelectMany(x => x.Values).Distinct().ToList();
	}

	// Gets all Nodes with this Graph.
	public List<Node> GetAllNodes() {
		// Return Keys to List.
		return Connections.Keys.ToList();
	}

	// Prints the Connection Matrix.
	public void PrintConnections() {
		// For Each Entry.
		foreach (var Entry in Connections) {
			// Print Label.
			Console.Write($"{Entry.Key.Label}: ");

			// For Each Connection, print Label.
			foreach (var Conn in Entry.Value.Keys)
				Console.Write($"{Conn.Label}; ");

			// go to next Line.
			Console.WriteLine();
		}
	}

	// Gets the Amount of Nodes.
	public int Size => Connections.Keys.Count;

	// Gets the Amount of Connections.
	public int ConnectionAmount => GetAllConnections().Count;

	// Finds a Node with a Specific Label.
	public Node? FindWithLabel(string Label) {
		// For each Node...
		foreach (var Node in Connections.Keys)
			if (Node.Label == Label)
				return Node;

		// Node not found.
		return null;
	}

	// Serializes the Graph to the String Format of Pajek.
	public override string ToString() {
		// Create Array of Vertices.
		List<Node> Nodes = GetAllNodes();

		// Create Result String, starting with the Vertices.
		StringBuilder result = new StringBuilder("*Vertices " + Nodes.Count + "\n");

		// For Each Vertex, add it.
		for (int v = 0; v < Nodes.Count; v++)
			result.Append($"{v + 1} \"{Nodes[v].Label}\"\n");

		// Get all Connections.
		List<Connection> Conns = GetAllConnections();

		// This Array is for All Arcs and the other one is for all Edges.
		List<Connection> Arcs = new List<Connection>();
		List<Connection> Edges = new List<Connection>();

		// Loop all Connections...
		foreach (var Conn in Conns) {
			// Switch the Connection Direction...
			switch (Conn.Direction) {
				// If it is directed, it is an arc.
				case Connection.EDirection.A_to_B:
				case Connection.EDirection.B_to_A: {
					// Add to Arcs.
					Arcs.Add(Conn);
				} break;

				// Otherwise, it is an edge.
				case Connection.EDirection.TwoWay: {
					// Add to Edges.
					Edges.Add(Conn);
				} break;
			}
		}

		// Check for Arcs.
		if (Arcs.Count > 0) {
			// Add Arcs Section.
			result.Append("*Arcs\n");

			// For Each Arc...
			foreach (var Conn in Arcs) {
				// Get Index of Nodes.
				int NodeA = Nodes.IndexOf(Conn.NodeA) + 1;
				int NodeB = Nodes.IndexOf(Conn.NodeB) + 1;

				// Add First Node to Result.
				result.Append(Conn.Direction == Connection.EDirection.A_to_B ? NodeA : NodeB);

				// Space it!
				result.Append(" ");

				// Add Second Node to Result.
				result.Append(Conn.Direction == Connection.EDirection.A_to_B ? NodeB : NodeA);

				// Space it again!
				result.Append(" ");

				// Write the Weight.
				result.Append(Conn.Weight);

				// New Line!
				result.Append("\n");
			}
		}

		// Check for Edges.
		if (Edges.Count > 0) {
			// Add Edges Section.
			result.Append("*Edges\n");

			// For Each Edge...
			foreach (var Conn in Edges) {
				// Get Index of Nodes.
				int NodeA = Nodes.IndexOf(Conn.NodeA) + 1;
				int NodeB = Nodes.IndexOf(Conn.NodeB) + 1;

				// Add First Node to Result.
				result.Append(NodeA);

				// Space it!
				result.Append(" ");

				// Add Second Node to Result.
				result.Append(NodeB);

				// Space it again!
				result.Append(" ");

				// Write the Weight.
				result.Append(Conn.Weight);

				// New Line!
				result.Append("\n");
			}
		}

		// Return Result.
		return result.ToString();
	}

	// Deserializes a Graph from a String Format. (Will use Pajek)
	public static Graph FromString(string Value) {
		// Create Result Graph.
		Graph G = new Graph();

		// Split value in lines.
		string[] Lines = Value.Split("\n").Where(x => !string.IsNullOrEmpty(x)).ToArray();

		// The Current Section.
		int CurrentSection = -1;

		// Nodes Array.
		Dictionary<int, Node> Nodes = new Dictionary<int, Node>();

        // For Each Line...
        foreach (string Line in Lines) {
            // Split Line by Spaces.
            string[] LineSplit = Line.Split(" ");

            // Check if Starts with the section...
            if (LineSplit[0].StartsWith("*")) {
                // The Current type we are parsing.
                string Type = LineSplit[0].Substring(1).ToLower();

                // Switch the Type.
                switch (Type) {
					// Vertices.
					case "vertices": {
						// Section 0.
						CurrentSection = 0;
					} break;

					// Arcs.
					case "arcs": {
						// Section 1.
						CurrentSection = 1;
					}  break;

					// Edges.
					case "edges": {
						// Section 2.
						CurrentSection = 2;
					} break;

					// Oh no, invalid Type!
					default: throw new ArgumentException($"'{Type}' section is not valid.");
				}

                // We parsed the type, continue!
                continue;
            }

            // If we aren't in a section, we are on an invalid graph.
            if (CurrentSection < 0)
				continue;

			// Switch our Current Section.
			switch (CurrentSection) {
				// Check for Vertices.
				case 0: {
					// The First Line Split is our Index.
					int Index = int.Parse(LineSplit[0]);

					// Check if Index Exists.
					if (Nodes.ContainsKey(Index))
						throw new ArgumentException($"Node of Index {Index} is already defined.");

					// Concatenate the rest as it should be our Label.
					string Label = LineSplit.Skip(1).Aggregate("", (first, second) => first + " " + second).Trim();

					// If the Label is nulled, set it to 'Node Index', otherwise, make sure it is on the correct format.
					if (string.IsNullOrEmpty(Label))
						Label = $"\"Node {Index}\"";
					else if (!Label.StartsWith("\"") || !Label.EndsWith("\""))
						throw new ArgumentException($"Label of Node {Index} isn't properly defined.");

					// Convert Label.
					Label = Label.Substring(1);

					// Create new Node.
					Node Node = new Node(Label);

					// Add to Nodes Array.
					Nodes.Add(Index, Node);

					// Add Node to Graph.
					G.AddNode(Node);
				} break;

				// Check for Arcs and Edges.
				case 1:
				case 2: {
					// Check for Nodes.
					if (Nodes.Count <= 0)
						throw new ArgumentException("Can't define Arcs and Edges with no Vertices.");

					// Make sure the we have at least two splits.
					if (LineSplit.Length < 2)
						throw new ArgumentException("Can't define an Arc/Edge without at least defining two node indexes.");

					// Get Indexes of Nodes A and B.
					int IndexA = int.Parse(LineSplit[0]);
					int IndexB = int.Parse(LineSplit[1]);

					// Make sure Index A exists.
					if (!Nodes.ContainsKey(IndexA))
						throw new ArgumentException($"Node of Index {IndexA} was not defined.");

					// Make sure Index B exists.
					if (!Nodes.ContainsKey(IndexB))
						throw new ArgumentException($"Node of Index {IndexB} was not defined.");

					// The Weight.
					int Weight = 0;

					// Check for third Index (Weight).
					if (LineSplit.Length >= 3)
						Weight = int.Parse(LineSplit[2]);

					// Connect Node.
					G.ConnectNode(Nodes[IndexA], Nodes[IndexB],
						new Connection("Generic Connection",
							CurrentSection == 1 ? Connection.EDirection.A_to_B : Connection.EDirection.TwoWay,
							Weight));
				} break;
			}
        }

        // Return Graph.
        return G;
    }

}