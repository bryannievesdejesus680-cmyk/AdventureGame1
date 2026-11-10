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
    public const string DROP = "DROP";
    public const string INVENTORY = "INVENTORY";

    public static void Main()
    {
        //Created Rooms
        Room entrance = new Room("Entrance","A dry stone chamber with daylight spilling through the doorway.");

        Room gallery = new Room("Gallery","A long gallery lined with faded banners.");

        Room archive = new Room("Archive","An archive whose shelves hold dust-covered chronicles.", false);

        // Connect rooms
        entrance.Connect(EAST, gallery);
        gallery.Connect(NORTH, archive);

        Item oldCoin = new Item("Old Coin","A tarnished coin stamped with a forgotten crest.");
        Item brassToken = new Item("Brass Token","A square brass token etched with a small tower.");
        Item copperRing = new Item("Copper Ring","A small ring with a dull copper finish.");

        gallery.PlaceItem(copperRing);
        gallery.PlaceItem(oldCoin);
        archive.PlaceItem(brassToken);

        Player player = new Player(entrance);

        Console.WriteLine("Adventure Game — Darkness and Grue Lab");

        Console.WriteLine( $"Commands: {GO} <direction>, {LOOK}, " + $"{TAKE} <item name>, {DROP} <item name>, " +
				$"{INVENTORY}, {QUIT}.");

        ShowRoom(player.CurrentRoom);

        // Main game loop
        while (true)
        {
            Console.Write("> ");

            string normalized = NormalizeInput(Console.ReadLine());

            if (normalized.Length == 0)
            {
                Console.WriteLine(
                    $"Enter {GO} <direction>, {LOOK}, " +
                    $"{TAKE} <item name>, {DROP} <item name>, " +
                    $"{INVENTORY}, or {QUIT}.");

                continue;
            }

            string[] words = normalized.Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries);

            string command = words[0];

            string target = words.Length > 1
                ? string.Join(" ", words, 1, words.Length - 1)
                : "";

            string? syntaxError = null;

            // PHASE 1: Validate command syntax

            if (command == QUIT || command == LOOK || command == INVENTORY)
	          {
		         if (words.Length != 1)
		          {
			         syntaxError = $"{command} takes no arguments.";
		          }
	          }
            
            else if (command == GO)
            {
                if (words.Length != 2)
                {
                    syntaxError = $"Use {GO} followed by one direction.";
                }
            }
            else if (command == TAKE || command == DROP)
            {
                if (words.Length < 2)
                {
                    syntaxError =
                        $"Use {command} followed by an item name.";
                }
            }
            else
            {
                syntaxError =
                    $"Unknown command. Use {GO} <direction>, " +
                    $"{LOOK}, {TAKE} <item name>, " +
                    $"{DROP} <item name>, {INVENTORY}, or {QUIT}.";
            }

            if (syntaxError is not null)
            {
                Console.WriteLine(syntaxError);
                continue;
            }

            // PHASE 2: Check the Grue hazard

            bool safeInDarkness =
                command == QUIT ||
                (command == GO &&
                 target == player.ReturnDirection);

            if (!player.CurrentRoom.IsLit && !safeInDarkness)
            {
                Console.WriteLine(DescribeGrueEncounter(command));

                Console.WriteLine(
                    "A Grue devours you. You have been defeated.");

                break;
            }

            // PHASE 3: Execute the normal command

            if (command == QUIT)
            {
                Console.WriteLine("You leave the dungeon.");
                break;
            }
            else if (command == LOOK)
            {
                ShowRoom(player.CurrentRoom);
            }
            else if (command == INVENTORY)
            {
                ShowInventory(player);
            }
            else if (command == GO)
            {
                bool moved = player.TryMove(
                    target,
                    out string message);

                Console.WriteLine(message);

                if (moved)
                {
                    ShowRoom(player.CurrentRoom);
                }
                else
                {
                    Console.WriteLine(
                        $"You remain in {player.CurrentRoom.Name}.");
                }
            }
            else if (command == TAKE)
            {
                player.TryTake(target, out string message);
                Console.WriteLine(message);
            }
            else if (command == DROP)
            {
                player.TryDrop(target, out string message);
                Console.WriteLine(message);
            }
        }
    }

    // Describe Grue encounter
    private static string DescribeGrueEncounter(string command)
    {
        if (command == GO)
        {
            return "You rush blindly ahead and stumble upon an angry woken-up beast.";
        }

        if (command == INVENTORY)
        {
            return "You rummage through your belongings looking for something, but something else finds you first.";
        }

        if (command == DROP)
        {
            return "You fumble with one of your belongings. The noise attracts something lurking in the darkness.";
        }

        if (command == TAKE)
        {
            return "You reach into the blackness. Something reaches back.";
        }

        // Default narration, including LOOK
        return "A heavy movement in the darkness interrupts your thoughts.";
    }

    // Normalize user input
    public static string NormalizeInput(string? input)
    {
        return input?.Trim().ToUpperInvariant() ?? QUIT;
    }

    // Display room information
    public static void ShowRoom(Room room)
    {
        if (!room.IsLit)
        {
            Console.WriteLine(
                "It is dark. You cannot see the room or its contents.");

            return;
        }

        Console.WriteLine($"{room.Name}: {room.Description}");

        Console.WriteLine(
            $" {NORTH}: {room.North?.Name ?? "(none)"}");

        Console.WriteLine(
            $" {SOUTH}: {room.South?.Name ?? "(none)"}");

        Console.WriteLine(
            $" {EAST}: {room.East?.Name ?? "(none)"}");

        Console.WriteLine(
            $" {WEST}: {room.West?.Name ?? "(none)"}");

        if (room.Items.Count == 0)
        {
            Console.WriteLine(" Items: (none)");
            return;
        }

        string itemList = string.Join(
            ", ",
            room.Items.Select(item => item.Name));

        Console.WriteLine($" Items: {itemList}");
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

        if (!neighbor.IsLit)
        {
            Console.WriteLine(
                $"The connected room {direction} is dark. " +
                "Its description cannot be seen.");
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
                $" - {item.Name}: {item.Description}");
        }
    }
}
