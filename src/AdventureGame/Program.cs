using AdventureGame;

public class Program
{
	public const string NORTH = "NORTH";
	public const string SOUTH = "SOUTH";
	public const string EAST = "EAST";
	public const string WEST = "WEST";
	public const string QUIT = "QUIT";
	public const string GO = "GO";
	public const string LOOK = "LOOK";
	public const string TAKE = "TAKE";
	
	public static void Main()
	{
		Room entrance = new Room("Entrance", "A dry stone chamber with daylight spilling through the doorway.");
		Room gallery = new Room("Gallery", "A long gallery lined with faded banners.");
		Room archive = new Room("Archive", "An archive whose shelves hold dust-covered chronicles.");

		entrance.Connect(EAST, gallery);
		gallery.Connect(NORTH, archive);

		Item oldCoin = new Item("Old Coin", "A tarnished coin stamped with a forgotten crest.");
		Item brassToken = new Item("Brass Token", "A square brass token etched with a small tower.");

		gallery.PlaceItem(oldCoin);
		archive.PlaceItem(brassToken);

		Player player = new Player(entrance);
		Console.WriteLine("Adventure Game — Item Transfer Lab");
		Console.WriteLine($"Commands: {GO} <direction>, {LOOK}, {TAKE} <item name>, {QUIT}.");
		Console.WriteLine($"Enter {GO} <direction>, {LOOK}, {TAKE} <item name>, or {QUIT}.");

		ShowRoom(player.CurrentRoom);

		while (true)
		{
			Console.Write("> ");

			string normalized = NormalizeInput(Console.ReadLine());

			if (normalized.Length == 0)
			{
				Console.WriteLine($"Enter {GO} <direction>, {LOOK}, or {QUIT}.");
				continue;
			}

			string[] words = normalized.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
			string command = words[0];

			if (command == QUIT)
			{
				if (words.Length != 1)
				{
					Console.WriteLine($"{QUIT} takes no arguments.");
					continue;
				}

				Console.WriteLine("You leave the dungeon.");
				break;
			}

			if (command == LOOK)
			{
				if (words.Length != 1)
				{
					Console.WriteLine($"{LOOK} takes no arguments.");
					continue;
				}

				ShowRoom(player.CurrentRoom);
				continue;
			}

			if (command == GO)
			{
				if (words.Length != 2)
				{
					Console.WriteLine($"Use {GO} followed by one direction: {NORTH}, {SOUTH}, {EAST}, or {WEST}.");
					continue;
				}

				string direction = words[1];
				bool moved = player.TryMove(direction, out string message);
				Console.WriteLine(message);

				if (moved)
				{
					ShowRoom(player.CurrentRoom);
				}
				else
				{
					Console.WriteLine($"You remain in {player.CurrentRoom.Name}.");
				}

				continue;
			}
			if (command == TAKE)
			{
				if (words.Length < 2)
				{
					Console.WriteLine($"Use {TAKE} followed by an item name.");
					continue;
				}

				string itemName = string.Join(" ", words, 1, words.Length - 1);
				player.TryTake(itemName, out string message);
				Console.WriteLine(message);
				continue;
			}

			Console.WriteLine($"Unknown command. Use {GO} <direction>, {LOOK}, {TAKE} <item name>, or {QUIT}.");
		}
	}

	public static string NormalizeInput(string? input)
	{
		return input?.Trim().ToUpperInvariant() ?? QUIT;
	}

	public static void ShowRoom(Room room)
	{
		Console.WriteLine($"{room.Name}: {room.Description}");
		Console.WriteLine($"  Items: {room.Item?.Name ?? "(none)"}");
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