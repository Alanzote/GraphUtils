using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using SDL2;

namespace GraphUtils;

// Functions related to viewing Graphs with SDL2.
public static class GraphView {

	// Graph View Refresh Rate.
	private static readonly uint RefreshRate = 16;

	// The Color Seed of Graph View.
	private static readonly int ColorSeed = 1701;

	// Displays a Graph.
	public static void Display(this Graph G) {
		// Init SDL.
		SDL.SDL_Init(SDL.SDL_INIT_VIDEO);

		// Init TTF.
		SDL_ttf.TTF_Init();

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

		// Current Selection.
		Node? Current = null;
		int? CurrentIdx = null;

		// The Current Zoom.
		int Zoom = 1;

		// Open Arial .tff.
		IntPtr Font = SDL_ttf.TTF_OpenFont("./Assets/arial.ttf", 12);

		// Dictionary of Node reference and text.
		Dictionary<Node, Tuple<IntPtr, IntPtr, int, int>> NodeText = new Dictionary<Node, Tuple<IntPtr, IntPtr, int, int>>();

		// Create Help Text.
		SDL_Utils.SDL_CreateText(Renderer, Font, "Use Arrow Keys to Move.\nZ, X to Increase/Decrease Zoom\nC to Cycle Selected Node (Shows as White)", Color.White, out IntPtr HelpTexture, out int HelpW, out int HelpH);

		// Create Help Rect.
		SDL.SDL_Rect HelpRect = new SDL.SDL_Rect {
			w = HelpW,
			h = HelpH,
			x = 0,
			y = 768 - HelpH
		};

		// Allocate Rect Ptr.
		IntPtr HelpRectPtr = Marshal.AllocHGlobal(Marshal.SizeOf<SDL.SDL_Rect>());

		// Copy Rect.
		Marshal.StructureToPtr(HelpRect, HelpRectPtr, false);

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
								OffsetX -= MovementSpeed;
							} break;

							// Key C for Cycle.
							case SDL.SDL_Keycode.SDLK_c: {
								// Check for Null..
								if (Current == null) {
									// Get First Node, set first index.
									Current = G.GetAllNodes().First();
									CurrentIdx = 0;
								} else {
									// Get all Nodes.
									var AllNodes = G.GetAllNodes();

									// Increase Current Idx.
									CurrentIdx = (CurrentIdx + 1) % AllNodes.Count;

									// Set Current.
									Current = AllNodes[CurrentIdx!.Value];
								}
							} break;

							// Plus for Zoom!
							case SDL.SDL_Keycode.SDLK_z: {
								// Add to Zoom.
								Zoom += 1;
							} break;

							// Minus for Less Zoom!
							case SDL.SDL_Keycode.SDLK_x: {
								// Remove from Zoom.
								Zoom = Math.Max(1, Zoom - 1);
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
				var x1 = NodeA.V1.Item1;
				var y1 = NodeA.V1.Item2;
				var x2 = NodeB.V1.Item1;
				var y2 = NodeB.V1.Item2;

				// Set to Random Color.
				SDL_Utils.SDL_SetToRandomColor(Renderer, ColRand);

				// Draw the Line.
				SDL.SDL_RenderDrawLine(Renderer, (x1 + OffsetX) * Zoom, (y1 + OffsetY) * Zoom, (x2 + OffsetX) * Zoom, (y2 + OffsetY) * Zoom);
			}

			// For Each Node...
			foreach (var Node in Nodes) {
				// Get the Node Casted.
				var NodeC = (Node<Tuple<int, int>>) Node;

				// Check if this Node is Selected.
				bool IsSelected = Current != null && Node == Current;

				// Set to Random Color.
				SDL_Utils.SDL_SetToRandomColor(Renderer, ColRand, !IsSelected);

				// Set Color to White if Selected.
				if (IsSelected)
					SDL_Utils.SDL_SetColor(Renderer, Color.White);

				// Draw Each Node.
				SDL_Utils.SDL_DrawCircle(Renderer, (NodeC.V1.Item1 + OffsetX) * Zoom, (NodeC.V1.Item2 + OffsetY) * Zoom, 5 * Zoom);

				// Check for Node Text.
				if (!NodeText.ContainsKey(Node)) {
					// Create it.
					SDL_Utils.SDL_CreateText(Renderer, Font, Node.Label, Color.White, out IntPtr Texture, out int W, out int H);

					// Allocate Rect Memory.
					IntPtr RectPtr = Marshal.AllocHGlobal(Marshal.SizeOf<SDL.SDL_Rect>());

					// Add it.
					NodeText.Add(Node, Tuple.Create(Texture, RectPtr, W, H));
				}

				// Create Rectagle.
				SDL.SDL_Rect Rect = new SDL.SDL_Rect {
					w = NodeText[Node].Item3 * Zoom,
					h = NodeText[Node].Item4 * Zoom,
					x = (NodeC.V1.Item1 + OffsetX - NodeText[Node].Item3 / 2) * Zoom,
					y = (NodeC.V1.Item2 + OffsetY + 10) * Zoom
				};

				// Copy Struct to Memory.
				Marshal.StructureToPtr(Rect, NodeText[Node].Item2, false);

				// Copy Text to Screen.
				SDL.SDL_RenderCopy(Renderer, NodeText[Node].Item1, IntPtr.Zero, NodeText[Node].Item2);
			}

			// Copy Help Text to Screen.
			SDL.SDL_RenderCopy(Renderer, HelpTexture, IntPtr.Zero, HelpRectPtr);

			// Set Color to What we Had Before.
			SDL_Utils.SDL_SetColor(Renderer, Color.Black);

			// Render.
			SDL.SDL_RenderPresent(Renderer);

			// Delay Each Loop.
			SDL.SDL_Delay(RefreshRate);
		}

		// Quit TTF.
		SDL_ttf.TTF_Quit();

		// Destroy Renderer.
		SDL.SDL_DestroyRenderer(Renderer);

		// Destroy SDL Window.
		SDL.SDL_DestroyWindow(Window);

		// Quit SDL.
		SDL.SDL_Quit();
	}
}

