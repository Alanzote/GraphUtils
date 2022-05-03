using SDL2;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GraphUtils;

// Helpers for SDL.
public static class SDL_Utils {

	// Set Color in SDL using the C# Default Color System.
	public static void SDL_SetColor(IntPtr Renderer, Color Col) {
		// Set Render Draw Color.
		SDL.SDL_SetRenderDrawColor(Renderer, Col.R, Col.G, Col.B, Col.A);
	}

	// Gets the Color in SDL using the C# Default Color System.
	public static Color SDL_GetColor(IntPtr Renderer) {
		// Call Get Color.
		SDL.SDL_GetRenderDrawColor(Renderer, out byte R, out byte G, out byte B, out byte A);

		// Return Color.
		return Color.FromArgb(A, R, G, B);
	}

	// Converts Color to SDL_Color.
	public static SDL.SDL_Color SDL_ToColor(Color Col) {
		// Return new SDl Color.
		return new SDL.SDL_Color {
			r = Col.R,
			g = Col.G,
			b = Col.B,
			a = Col.A
		};
	}

	// Draws a SDL Circle.
	public static void SDL_DrawCircle(IntPtr Renderer, int CenterX, int CenterY, int Radius) {
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

	// Creates the Text Texture.
	public static void SDL_CreateText(IntPtr Renderer, IntPtr Font, string text, Color Col, out IntPtr Texture, out int w, out int h) {
		// Create Surface.
		IntPtr Surface = SDL_ttf.TTF_RenderText_Solid_Wrapped(Font, text, SDL_ToColor(Col), 0);

		// Create Texture.
		Texture = SDL.SDL_CreateTextureFromSurface(Renderer, Surface);

		// Get Surface Data.
		SDL.SDL_Surface Surf_Data = Marshal.PtrToStructure<SDL.SDL_Surface>(Surface);

		// Set Width and Height.
		w = Surf_Data.w;
		h = Surf_Data.h;

		// Free Surface.
		SDL.SDL_FreeSurface(Surface);
	}

	// Creates the Text Texture.
	public static void SDL_CreateText(IntPtr Renderer, IntPtr Font, string text, Color Col, out IntPtr Texture, out SDL.SDL_Rect Rect) {
		// Create Surface.
		IntPtr Surface = SDL_ttf.TTF_RenderText_Solid_Wrapped(Font, text, SDL_ToColor(Col), 0);

		// Create Texture.
		Texture = SDL.SDL_CreateTextureFromSurface(Renderer, Surface);

		// Get Surface Data.
		SDL.SDL_Surface Surf_Data = Marshal.PtrToStructure<SDL.SDL_Surface>(Surface);

		// Create Rect.
		Rect = new SDL.SDL_Rect {
			w = Surf_Data.w,
			h = Surf_Data.h
		};

		// Free Surface.
		SDL.SDL_FreeSurface(Surface);
	}

	// Sets to the Random Color.
	public static void SDL_SetToRandomColor(IntPtr Renderer, Random ColRand, bool Apply = true) {
		// Create Color Buffer and Get next 3 Random Bytes.
		byte[] ColBuf = new byte[3];
		ColRand.NextBytes(ColBuf);

		// Set to Random Color, if we should apply.
		// Apply should be False if we are just randomizing to keep the Seed Step, but we aren't using the color.
		if (Apply)
			SDL.SDL_SetRenderDrawColor(Renderer, ColBuf[0], ColBuf[1], ColBuf[2], 0xFF);
	}
}
