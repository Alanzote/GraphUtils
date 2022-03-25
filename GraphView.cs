using SDL2;

namespace GraphUtils;

// Functions related to viewing Graphs with SDL2.
public static class GraphView {

	// Graph View Refresh Rate.
	private static readonly uint RefreshRate = 16;

	// The Color Seed of Graph View.
	private static readonly int ColorSeed = 1701;

	// Draws a SDL Circle.
	private static void SDL_DrawCircle(IntPtr Renderer, int CenterX, int CenterY, int Radius) {
		// Calculate the Diameter.
		int Diameter = Radius * 2;

		// Calculate initial variables.
		int x = Radius - 1;
		int y = 0;
		int tx = 1;
		int ty = 1;
		int error = tx - Diameter;

		// Loop until x >= y.
		while (x >= y) {
			// Draw the Points.
			SDL.SDL_RenderDrawPoint(Renderer, CenterX + x, CenterY - y);
			SDL.SDL_RenderDrawPoint(Renderer, CenterX + x, CenterY + y);
			SDL.SDL_RenderDrawPoint(Renderer, CenterX - x, CenterY - y);
			SDL.SDL_RenderDrawPoint(Renderer, CenterX - x, CenterY + y);
			SDL.SDL_RenderDrawPoint(Renderer, CenterX + y, CenterY - x);
			SDL.SDL_RenderDrawPoint(Renderer, CenterX + y, CenterY + x);
			SDL.SDL_RenderDrawPoint(Renderer, CenterX - y, CenterY - x);
			SDL.SDL_RenderDrawPoint(Renderer, CenterX - y, CenterY + x);

			// Check for Error <= 0.
			if (error <= 0) {
				// Do some stuff.
				++y;
				error += ty;
				ty += 2;
			}

			// Check for Error > 0.
			if (error > 0) {
				// Do some stuff.
				--x;
				tx += 2;
				error += tx - Diameter;
			}
		}
	}

	// Sets to the Random Color.
	private static void SDL_SetToRandomColor(IntPtr Renderer, Random ColRand) {
		// Create Color Buffer and Get next 3 Random Bytes.
		byte[] ColBuf = new byte[3];
		ColRand.NextBytes(ColBuf);

		// Set to Random Color.
		SDL.SDL_SetRenderDrawColor(Renderer, ColBuf[0], ColBuf[1], ColBuf[2], 0xF0);
	}

	// Displays a Graph.
	public static void Display(this Graph G) {
		// Init SDL.
		SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

		// Create SDL Window.
		var Window = SDL.SDL_CreateWindow("GraphUtils",
			SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
			1024, 768,
			SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

		// Create Renderer.
		var Renderer = SDL.SDL_CreateRenderer(Window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

		// Current Offset in X and Y.
		int OffsetX = 0;
		int OffsetY = 0;

		// The Movement Coeficient.
		int MovementSpeed = 10;

		Node? Current = null;
		int? CurrentIdx = null;

		// Loop until...
		while (true) {
			// If we should quit this Loop.
			bool Quit = false;

			// Poll all SDL Events until there are None.
			while (SDL.SDL_PollEvent(out SDL.SDL_Event evt) != 0) {
				// Switch evt.
				switch (evt.type) {
					// If it is SDL_quit...
					case SDL.SDL_EventType.SDL_QUIT: {
						// We Quit.
						Quit = true;
					} break;

					// Check for Keydown.
					case SDL.SDL_EventType.SDL_KEYDOWN: {
						// Switch Key.
						switch (evt.key.keysym.sym) {
							// Key up!
							case SDL.SDL_Keycode.SDLK_UP: {
								// Increase Offset.
								OffsetY += MovementSpeed;
							} break;

							// Key Down!
							case SDL.SDL_Keycode.SDLK_DOWN: {
								// Decrease Offset.
								OffsetY -= MovementSpeed;
							} break;

							// Key Left!
							case SDL.SDL_Keycode.SDLK_LEFT: {
								// Increase Offset.
								OffsetX += MovementSpeed;
							} break;

							// Key Right!
							case SDL.SDL_Keycode.SDLK_RIGHT: {
								// Decrease Offset.
								OffsetX += MovementSpeed;
							} break;

							// Key C for Cycle.
							case SDL.SDL_Keycode.SDLK_c: {
								// Cycle Selection.
								if (Current == null) {
									Current = G.GetAllNodes().First();
									CurrentIdx = 0;
								} else {
									var AllNodes = G.GetAllNodes();

									CurrentIdx += 1;

									if (CurrentIdx >= AllNodes.Count)
										CurrentIdx = 0;

									Current = AllNodes[CurrentIdx!.Value];
								}

								Console.WriteLine(Current!.Label);
							} break;
						}
					} break;
				}
			}

			// Check for Quit.
			if (Quit)
				break;

			// Create new Color Random.
			Random ColRand = new Random(ColorSeed);

			// Clear Video.
			SDL.SDL_RenderClear(Renderer);

			// List of Rendered Connections and Nodes.
			var Conns = G.GetAllConnections();
			var Nodes = G.GetAllNodes();

			// For Each Connection...
			foreach (var Conn in Conns) {
				// Get Nodes Casted.
				var NodeA = (Node<Tuple<int, int>>) Conn.NodeA;
				var NodeB = (Node<Tuple<int, int>>) Conn.NodeB;

				// Calculate Positions.
				var x1 = NodeA.Value.Item1;
				var y1 = NodeA.Value.Item2;
				var x2 = NodeB.Value.Item1;
				var y2 = NodeB.Value.Item2;

				// Set to Random Color.
				SDL_SetToRandomColor(Renderer, ColRand);

				// Draw the Line.
				SDL.SDL_RenderDrawLine(Renderer, x1 + OffsetX, y1 + OffsetY, x2 + OffsetX, y2 + OffsetY);
			}

			// Set Draw Color to White.
			SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);

			// For Each Node...
			foreach (var Node in Nodes) {
				// Get the Node Casted.
				var NodeC = (Node<Tuple<int, int>>) Node;

				if (Current != null && Node == Current)
					SDL.SDL_SetRenderDrawColor(Renderer, 0xFF, 0xFF, 0xFF, 0xFF);
				else
					SDL_SetToRandomColor(Renderer, ColRand);

				// Draw Each Node.
				SDL_DrawCircle(Renderer, NodeC.Value.Item1 + OffsetX, NodeC.Value.Item2 + OffsetY, 5);
				//SDL.SDL_RenderDrawPoint(Renderer, NodeC.Value.Item1 + OffsetX, NodeC.Value.Item2 + OffsetY);
			}

			// Set Color to What we Had Before.
			SDL.SDL_SetRenderDrawColor(Renderer, 0x00, 0x00, 0x00, 0xFF);

			// Render.
			SDL.SDL_RenderPresent(Renderer);

			// Delay Each Loop.
			SDL.SDL_Delay(RefreshRate);
		}

		// Destroy Renderer.
		SDL.SDL_DestroyRenderer(Renderer);

		// Destroy SDL Window.
		SDL.SDL_DestroyWindow(Window);

		// Quit SDL.
		SDL.SDL_Quit();
	}
}

