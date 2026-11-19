using AdventureGame;
using static AdventureGame.ICommand;
using static AdventureGame.Room;
using System.Collections.Generic;

public class Program
{

	public const string NORTH = "NORTH";
	public const string SOUTH = "SOUTH";
	public const string EAST = "EAST";
	public const string WEST = "WEST";

	public const string QUIT = "QUIT";
	public const string GO = "GO";
	public const string OPEN = "OPEN";
	public const string LOOK = "LOOK";
	public const string TAKE = "TAKE";
	public const string USE = "USE";
	public const string DROP = "DROP";
	public const string INVENTORY = "INVENTORY";

	public static bool HasLight(Player player)
	{
		Room room = player.CurrentRoom;
		return room.IsLit ||
			player.Inventory.OfType<Lamp>().Any(lamp => lamp.IsOn) ||
			room.Items.OfType<Lamp>().Any(lamp => lamp.IsOn);
	}

	public static void Main()
	{
		Room entrance = new Room("Entrance", "A dry stone chamber with daylight spilling through the doorway.");
		Room gallery = new Room("Gallery", "A long gallery lined with faded banners.");
		Room archive = new Room("Archive", "An archive whose shelves hold dust-covered chronicles.", false);
		Room storage = new Room("Storage", "A small storeroom with stacked wooden crates.");

		// Connect rooms
		entrance.Connect(EAST, gallery);
		gallery.Connect(NORTH, archive);
		gallery.Connect(EAST, storage);

		Item oldCoin = new Item(
				"Old Coin",
				"A tarnished coin stamped with a forgotten crest.");

		Item brassToken = new Item(
				"Brass Token",
				"A square brass token etched with a small tower.");

		Item copperRing = new Item(
				"Copper Ring",
				"A small ring with a dull copper finish.");

		Lamp lamp = new Lamp(
				"Lamp",
				"A sturdy oil lamp with a metal switch.");

		Item ironKey = new Item(
				"Iron Key",
				"A cold iron key with a square bow.");

		Door ironDoor = new Door("Iron Door", ironKey);


		gallery.PlaceItem(oldCoin);
		gallery.PlaceItem(copperRing);
		gallery.PlaceItem(lamp);
		gallery.AttachDoor(EAST, ironDoor);
		archive.PlaceItem(brassToken);
		archive.PlaceItem(ironKey);

		Player player = new Player(entrance);
		IReadOnlyList<ICommand> commands = new ICommand[]
		{
		 new GoCommand(),
		 new LookCommand(),
		 new TakeCommand(),
		 new DropCommand(),
		 new InventoryCommand(),
		 new UseCommand(),
		 new OpenCommand()
		};

		Console.WriteLine("Adventure Game — Doors and Keys Lab");
		Console.WriteLine($"Commands: {GO} <direction>, {LOOK}, {TAKE} <item name>, " +
		$"{DROP} <item name>, {USE} <item name>, {OPEN} <object name>, {INVENTORY}, {QUIT}.");

		ShowRoom(player);

		// Main game loop
		while (true)
		{
			Console.Write("> ");
			string normalized = NormalizeInput(Console.ReadLine());

			if (normalized.Length == 0)
			{
				Console.WriteLine($"Enter {GO} <direction>, {LOOK}, {TAKE} <item name>, " +
				$"{DROP} <item name>, {USE} <item name>, {OPEN} <object name>, {INVENTORY}, or {QUIT}.");
				continue;
			}

			string[] words = normalized.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
			string verb = words[0];
			string target = words.Length > 1 ? string.Join(" ", words, 1, words.Length - 1) : "";

			if (verb == QUIT)
			{
				if (words.Length != 1)
				{
					Console.WriteLine($"{QUIT} takes no arguments.");
					continue;
				}

				Console.WriteLine("You leave the dungeon.");
				break;
			}

			ICommand? selected = FindCommand(verb, commands);

			if (selected is null)
			{
				Console.WriteLine($"Unknown command. Use {GO} <direction>, {LOOK}, {TAKE} <item name>, " +
				$"{DROP} <item name>, {USE} <item name>, {OPEN} <object name>, {INVENTORY}, or {QUIT}.");
				continue;
			}

			if (!selected.HasValidArguments(words, out string syntaxMessage))
			{
				Console.WriteLine(syntaxMessage);
				continue;
			}


			if (!HasLight(player))
			{
				bool safeRetreat =
						verb == GO && target == player.ReturnDirection;

				bool safeLampReactivation =
						verb == USE &&
						player.FindItem(target) is Lamp carriedLamp &&
						!carriedLamp.IsOn;

				if (!safeRetreat && !safeLampReactivation)
				{
					Console.WriteLine(selected.DescribeInterruptedAttempt());
					Console.WriteLine("A Grue devours you. You have been defeated.");
					break;
				}
			}

			selected.Execute(player, target);

		}

	}
	private static ICommand? FindCommand(
			string verb,
			IReadOnlyList<ICommand> commands)
	{
		foreach (ICommand candidate in commands)
		{
			if (candidate.Verb == verb)
			{
				return candidate;
			}
		}

		return null;
	}
	public static string NormalizeInput(string? input)
	{
		return input?.Trim().ToUpperInvariant() ?? QUIT;
	}

	// Display room information
	public static void ShowRoom(Player player)
	{
		if (!HasLight(player))
		{
			Console.WriteLine("It is dark. You cannot see the room or its contents.");
			return;
		}

		Room room = player.CurrentRoom;
		Console.WriteLine($"{room.Name}: {room.Description}");
		Console.WriteLine($"  {NORTH}: {room.North?.Name ?? "(none)"}");
		Console.WriteLine($"  {SOUTH}: {room.South?.Name ?? "(none)"}");
		Console.WriteLine($"   {EAST}: {room.East?.Name ?? "(none)"}");
		Console.WriteLine($"   {WEST}: {room.West?.Name ?? "(none)"}");

		if (room.Items.Count == 0)
		{
			Console.WriteLine("  Items: (none)");
		}
		else
		{
			string itemList = string.Join(", ", room.Items.Select(item => item.Name));
			Console.WriteLine($"  Items: {itemList}");
		}
		foreach (string direction in new[] { NORTH, SOUTH, EAST, WEST })
		{
			Door? door = room.GetDoor(direction);

			if (door is not null)
			{
				string state = door.IsOpen
						? "open"
						: door.IsLocked ? "locked" : "closed";

				Console.WriteLine(
						$"  {direction} door: {door.Name} ({state})");
			}
		}
	}

	// Preview connected room
	public static void PreviewExit(Room fromRoom, string direction)
	{
		if (direction != NORTH &&
				direction != SOUTH &&
				direction != EAST &&
				direction != WEST)
		{
			Console.WriteLine(
					$"Unknown direction. Choose {NORTH}, {SOUTH}, " +
					$"{EAST}, or {WEST}.");

			return;
		}

		if (!fromRoom.IsLit)
		{
			Console.WriteLine(
					"It is dark. You cannot see the room or its contents.");
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
			Console.WriteLine(
					$"There is no connected room {direction} of {fromRoom.Name}.");

			return;
		}
		Door? door = fromRoom.GetDoor(direction);

		if (door is not null && !door.IsOpen)
		{
			Console.WriteLine(
					$"The {door.Name} blocks the passage {direction} from {fromRoom.Name}.");

			return;
		}

		if (!neighbor.IsLit)
		{
			Console.WriteLine(
					$"The passage {direction} from {fromRoom.Name} leads into darkness.");
			return;
		}

		Console.WriteLine(
				$"Connected room {direction} of {fromRoom.Name}: {neighbor.Name}");

		Console.WriteLine($"Description: {neighbor.Description}");
	}

	// Display player inventory
	public static void ShowInventory(Player player)
	{
		if (player.Inventory.Count == 0)
		{
			Console.WriteLine("Inventory: (empty)");
			return;
		}

		Console.WriteLine("Inventory:");

		foreach (Item item in player.Inventory)
		{
			Console.WriteLine(
					$"  - {item.Name}: {item.Description}");
		}
	}
}

