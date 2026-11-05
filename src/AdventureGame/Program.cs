using AdventureGame;

public class Program
{
	public const string NORTH = "NORTH";
	public const string SOUTH = "SOUTH";
	public const string EAST = "EAST";
	public const string WEST = "WEST";
	public const string QUIT = "QUIT";
	
	public static void Main()
	{
		Room entrance = new Room("Entrance", "A dry stone chamber with daylight spilling through the doorway.");
		Room gallery = new Room("Gallery", "A long gallery lined with faded banners.");
		Room archive = new Room("Archive", "An archive whose shelves hold dust-covered chronicles.");

		entrance.East = gallery;
		gallery.West = entrance;
		gallery.North = archive;
		archive.South = gallery;

		ShowRoom(gallery);

		while (true)
		{
			Console.Write($"Enter {NORTH}, {SOUTH}, {EAST}, {WEST}, or {QUIT} to stop: ");

			string direction = NormalizeInput(Console.ReadLine());

			if (direction == QUIT)
			{
				Console.WriteLine("Room inspector closed.");
				break;
			}

			PreviewExit(gallery, direction);
		}
	}

	public static string NormalizeInput(string? input)
	{
		return input?.Trim().ToUpperInvariant() ?? QUIT;
	}

	public static void ShowRoom(Room room)
	{
		Console.WriteLine($"{room.Name}: {room.Description}");
		Console.WriteLine($"  {NORTH}: {room.North?.Name ?? "(none)"}");
		Console.WriteLine($"  {SOUTH}: {room.South?.Name ?? "(none)"}");
		Console.WriteLine($"   {EAST}: {room.East?.Name ?? "(none)"}");
		Console.WriteLine($"   {WEST}: {room.West?.Name ?? "(none)"}");
	}

	public static void PreviewExit(Room fromRoom, string direction)
	{
		if (direction != NORTH && direction != SOUTH && direction != EAST && direction != WEST)
		{
			Console.WriteLine($"Unknown direction. Choose {NORTH}, {SOUTH}, {EAST}, or {WEST}.");
			return;
		}

		Room? neighbor = direction switch
		{
			NORTH => fromRoom.North,
			SOUTH => fromRoom.South,
			EAST => fromRoom.East,
			WEST => fromRoom.West,
			_ => null
		};

		if (neighbor is null)
		{
			Console.WriteLine($"There is no connected room {direction} of {fromRoom.Name}.");
			return;
		}

		Console.WriteLine($"Connected room {direction} of {fromRoom.Name}: {neighbor.Name}");
		Console.WriteLine($"Description: {neighbor.Description}");
	}
}