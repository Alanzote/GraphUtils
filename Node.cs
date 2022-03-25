namespace GraphUtils;

// A node of our Graph.
public class Node {

	// The Label of this Node.
	public string Label { get; set; }

	// Constructor.
	public Node(string Label) {
		// Set Data.
		this.Label = Label;
	}
}

// A node, with a generic value container.
public class Node<T> : Node {
	
	// Holds the Value.
	public T Value { get; set; }

	// Constructor.
	public Node(string Label, T Value) : base(Label) {
		// Set Value.
		this.Value = Value;
	}
}