
using System;
using System.IO;
using Xunit;

namespace AdventureGame.Tests;

[Collection("Console I/O")]
public class DoorTests
{
	[Fact]
	public void TryOpen_RequiresExactCarriedKeyAndDoesNotConsumeIt()
	{
		Room gallery = new Room("Gallery", "A lit gallery.");
		Item key = new Item("Iron Key", "The compatible key.");
		Item lookalike = new Item("Iron Key", "A different key.");
		Door door = new Door("Iron Door", key);
		Player player = new Player(gallery);

		Assert.True(door.IsLocked);
		Assert.False(door.IsOpen);
		Assert.False(door.TryOpen(player, out _));

		gallery.PlaceItem(lookalike);
		Assert.True(player.TryTake("Iron Key", out _));
		Assert.False(door.TryOpen(player, out _));
		Assert.True(door.IsLocked);
		Assert.False(door.IsOpen);

		gallery.PlaceItem(key);
		Assert.True(player.TryTake("Iron Key", out _));
		Assert.True(door.TryOpen(player, out string message));
		Assert.Equal("You unlock and open the Iron Door.", message);
		Assert.False(door.IsLocked);
		Assert.True(door.IsOpen);
		Assert.Contains(key, player.Inventory);
		Assert.Equal(2, player.Inventory.Count);
		Assert.False(door.TryOpen(player, out string again));
		Assert.Equal("The Iron Door is already open.", again);
	}

	[Fact]
	public void UnlockedDoor_OpensWithoutAKey()
	{
		Door door = new Door("Wooden Door");
		Player player = new Player(new Room("Hall", "A lit hall."));

		Assert.False(door.IsLocked);
		Assert.False(door.IsOpen);
		Assert.True(door.TryOpen(player, out string message));
		Assert.Equal("You open the Wooden Door.", message);
		Assert.True(door.IsOpen);
	}

	[Fact]
	public void AttachDoor_UsesOneReferenceAndRejectsConflicts()
	{
		Room gallery = new Room("Gallery", "A lit gallery.");
		Room storage = new Room("Storage", "A lit storeroom.");
		gallery.Connect(Program.EAST, storage);
		Door door = new Door("Iron Door");

		gallery.AttachDoor(Program.EAST, door);
		Assert.Same(door, gallery.GetDoor(Program.EAST));
		Assert.Same(door, storage.GetDoor(Program.WEST));
		Assert.Same(door, storage.FindDoor("IRON DOOR"));

		Assert.Throws<InvalidOperationException>(() =>
				gallery.AttachDoor(Program.EAST, new Door("Other Door")));
		Assert.Same(door, gallery.GetDoor(Program.EAST));
		Assert.Same(door, storage.GetDoor(Program.WEST));
	}

	[Fact]
	public void AttachDoor_RejectsMissingPassageWithoutMutation()
	{
		Room first = new Room("First", "A lit room.");
		Room second = new Room("Second", "A lit room.");
		Door door = new Door("Iron Door");

		Assert.Throws<InvalidOperationException>(() =>
				first.AttachDoor(Program.EAST, door));
		Assert.Null(first.GetDoor(Program.EAST));
		Assert.Null(second.GetDoor(Program.WEST));
	}

	[Fact]
	public void TryMove_RejectsClosedDoorWithoutChangingReturnDirection()
	{
		Room entrance = new Room("Entrance", "A lit entrance.");
		Room gallery = new Room("Gallery", "A lit gallery.");
		Room storage = new Room("Storage", "A lit storeroom.");
		entrance.Connect(Program.EAST, gallery);
		gallery.Connect(Program.EAST, storage);
		Item key = new Item("Iron Key", "The compatible key.");
		Door door = new Door("Iron Door", key);
		gallery.AttachDoor(Program.EAST, door);
		gallery.PlaceItem(key);
		Player player = new Player(entrance);

		Assert.True(player.TryMove(Program.EAST, out _));
		Assert.Equal(Program.WEST, player.ReturnDirection);
		Assert.False(player.TryMove(Program.EAST, out string blocked));
		Assert.Equal("The Iron Door is closed. You cannot pass.", blocked);
		Assert.Same(gallery, player.CurrentRoom);
		Assert.Equal(Program.WEST, player.ReturnDirection);

		Assert.True(player.TryTake("Iron Key", out _));
		Assert.True(door.TryOpen(player, out _));
		Assert.True(player.TryMove(Program.EAST, out _));
		Assert.Same(storage, player.CurrentRoom);
		Assert.Equal(Program.WEST, player.ReturnDirection);
		Assert.True(player.TryMove(Program.WEST, out _));
		Assert.Same(gallery, player.CurrentRoom);
		Assert.Equal(Program.EAST, player.ReturnDirection);
	}

	[Fact]
	public void OpenCommand_UsesInheritedInterfaceAndArgumentRules()
	{
		ICommand command = new OpenCommand();

		Assert.Equal(Program.OPEN, command.Verb);
		Assert.False(command.HasValidArguments(new[] { Program.OPEN }, out _));
		Assert.True(command.HasValidArguments(
				new[] { Program.OPEN, "IRON", "DOOR" }, out _));
	}

	[Fact]
	public void RoomDisplay_ShowsDoorButConcealsUnlitArchive()
	{
		Room gallery = new Room("Gallery", "A lit gallery.");
		Room storage = new Room("Storage", "A lit storeroom.");
		Room archive = new Room("Archive", "A dark archive.", false);
		gallery.Connect(Program.EAST, storage);
		gallery.Connect(Program.NORTH, archive);
		Item key = new Item("Iron Key", "A cold iron key.");
		Door door = new Door("Iron Door", key);
		gallery.AttachDoor(Program.EAST, door);
		archive.PlaceItem(key);

		Player galleryPlayer = new Player(gallery);
		string visible = Capture(() => Program.ShowRoom(galleryPlayer));
		Assert.Contains("EAST door: Iron Door (locked)", visible);

		Player archivePlayer = new Player(archive);
		string hidden = Capture(() => Program.ShowRoom(archivePlayer));
		Assert.Contains(
				"It is dark. You cannot see the room or its contents.", hidden);
		Assert.DoesNotContain("A dark archive.", hidden);
		Assert.DoesNotContain("Iron Key", hidden);

		string preview = Capture(() =>
				Program.PreviewExit(gallery, Program.EAST));
		Assert.Contains(
				"The Iron Door blocks the passage EAST from Gallery.", preview);
		Assert.DoesNotContain("A lit storeroom.", preview);
	}

	private static string Capture(Action action)
	{
		TextWriter original = Console.Out;
		using StringWriter output = new StringWriter();

		try
		{
			Console.SetOut(output);
			action();
		}
		finally
		{
			Console.SetOut(original);
		}

		return output.ToString();
	}
}
