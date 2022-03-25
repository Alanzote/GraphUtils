namespace GraphUtils;

// A connection on our graph.
public class Connection : IComparable<Connection> {

	// Reference to Node A.
	public Node NodeA { get; set; }

	// Reference to Node B.
	public Node NodeB { get; set; }

	// The Dierection of a Connection.
	public enum EDirection {
		TwoWay,
		A_to_B,
		B_to_A
	}

	// The Direction of this Connection.
	public EDirection Direction { get; set; }

	// The Weight of this Connection.
	public int Weight { get; set; }

	// The Label of this Connection.
	public string Label { get; set; }

	// Creates a Connection.
	public Connection(string Label, EDirection Direction, int Weight) {
		// Set Data.
		this.Label = Label;
		this.Direction = Direction;
		this.Weight = Weight;
	}

	// Compares to another Connection.
	public int CompareTo(Connection? other) {
		// Return Weight Difference.
		return Weight - other!.Weight;
	}
}

// A connection that also keeps a value.
public class Connection<T> : Connection {

	// The Value we are holding.
	public T Value { get; set; }

	// Constructor.
	public Connection(string Label, EDirection Direction, int Weight, T Value) : base(Label, Direction, Weight) {
		// Set Value.
		this.Value = Value;
	}

}