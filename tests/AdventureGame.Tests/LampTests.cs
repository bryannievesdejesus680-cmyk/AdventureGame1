using System;
using System.IO;
using Xunit;

namespace AdventureGame.Tests;

[Collection("Console I/O")]
public class LampTests
{
	[Fact]
	public void Lamp_InheritsItemAndPreservesItsIdentityAndState()
	{
		Room gallery = new Room("Gallery", "A lit gallery.");
		Lamp lamp = new Lamp("Lamp", "A sturdy oil lamp.");
		gallery.PlaceItem(lamp);
		Player player = new Player(gallery);
		UseCommand use = new UseCommand();

		Assert.IsAssignableFrom<Item>(lamp);
		Assert.Equal("Lamp", lamp.Name);
		Assert.False(lamp.IsOn);
		Assert.Null(player.FindItem("LAMP"));

		use.Execute(player, "LAMP");
		Assert.False(lamp.IsOn);

		Assert.True(player.TryTake("Lamp", out _));
		Assert.Same(lamp, player.FindItem("lAmP"));
		Assert.Same(lamp, Assert.Single(player.Inventory));

		use.Execute(player, "lamp");
		Assert.True(lamp.IsOn);

		Assert.True(player.TryDrop("Lamp", out _));
		Assert.Same(lamp, Assert.Single(gallery.Items));
		Assert.Null(player.FindItem("lamp"));

		use.Execute(player, "LAMP");
		Assert.True(lamp.IsOn);
	}

	[Fact]
	public void ActiveLamp_IlluminatesArchiveAfterDropAndCanBeRecovered()
	{
		Room gallery = new Room("Gallery", "A lit gallery.");
		Room archive = new Room("Archive", "A dark archive.", false);

		gallery.Connect(Program.NORTH, archive);
		archive.PlaceItem(new Item("Brass Token", "A heavy brass token."));

		Lamp lamp = new Lamp("Lamp", "A sturdy oil lamp.");
		gallery.PlaceItem(lamp);

		Player player = new Player(gallery);

		Assert.True(player.TryTake("Lamp", out _));
		new UseCommand().Execute(player, "Lamp");

		Assert.True(player.TryMove(Program.NORTH, out _));
		Assert.True(Program.HasLight(player));
		Assert.Contains("Archive: A dark archive.", CaptureRoomOutput(player));

		Assert.True(player.TryDrop("Lamp", out _));
		Assert.Contains(lamp, archive.Items);
		Assert.True(lamp.IsOn);
		Assert.False(archive.IsLit);
		Assert.True(Program.HasLight(player));

		string visible = CaptureRoomOutput(player);

		Assert.Contains("Archive: A dark archive.", visible);
		Assert.Contains("Brass Token", visible);
		Assert.Contains("Lamp", visible);

		Assert.True(player.TryTake("Lamp", out _));
		Assert.Same(lamp, player.FindItem("LAMP"));
		Assert.True(Program.HasLight(player));
	}

	[Fact]
	public void UseCommand_ImplementsTheInheritedInterface()
	{
		Room gallery = new Room("Gallery", "A lit gallery.");
		Lamp lamp = new Lamp("Lamp", "A sturdy oil lamp.");
		gallery.PlaceItem(lamp);

		Player player = new Player(gallery);
		Assert.True(player.TryTake("Lamp", out _));

		ICommand command = new UseCommand();

		Assert.Equal(ICommand.USE, command.Verb);

		Assert.False(command.HasValidArguments(
				new[] { ICommand.USE }, out _));

		Assert.True(command.HasValidArguments(
				new[] { ICommand.USE, "LAMP" }, out _));

		command.Execute(player, "LAMP");
		Assert.True(lamp.IsOn);
	}

	private static string CaptureRoomOutput(Player player)
	{
		TextWriter originalOutput = Console.Out;
		using StringWriter output = new StringWriter();

		try
		{
			Console.SetOut(output);
			Program.ShowRoom(player);
		}
		finally
		{
			Console.SetOut(originalOutput);
		}

		return output.ToString();
	}
}
