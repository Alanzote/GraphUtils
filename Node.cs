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
public class Node<T1> : Node {
	
	// Holds the Value.
	public T1 V1 { get; set; }

	// Constructor.
	public Node(string Label, T1 V1) : base(Label) {
		// Set Value.
		this.V1 = V1;
	}
}

// A node, with two generic value containers.
public class Node<T1, T2> : Node<T1> {

	// Hold the 2nd Value.
	public T2 V2 { get; set; }

	// Construtor.
	public Node(string Label, T1 V1, T2 V2) : base(Label, V1) {
		// Set Value.
		this.V2 = V2;
	}
}