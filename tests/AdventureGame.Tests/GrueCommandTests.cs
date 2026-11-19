using System;
using System.IO;
using Xunit;
using AdventureGame;

namespace AdventureGame.Tests;

[Collection("Console I/O")]
public class GrueCommandTests
{
	[Fact]
	public void Lamp_ReactivationInArchive_RequiresLookForRoomRedisplay()
	{
		string noLook = Play(
				"GO EAST", "TAKE LAMP", "USE LAMP",
				"GO NORTH", "USE LAMP", "USE LAMP", "QUIT");

		string withLook = Play(
				"GO EAST", "TAKE LAMP", "USE LAMP",
				"GO NORTH", "USE LAMP", "USE LAMP",
				"LOOK", "QUIT");

		string on = "The Lamp is now on.";
		string archive =
				"Archive: An archive whose shelves hold dust-covered chronicles.";

		int firstOn = noLook.IndexOf(on, StringComparison.Ordinal);
		int firstArchive = noLook.IndexOf(archive, StringComparison.Ordinal);
		int off = noLook.IndexOf("The Lamp is now off.", StringComparison.Ordinal);
		int darkness = noLook.IndexOf("The room falls dark.", StringComparison.Ordinal);
		int secondOn = noLook.IndexOf(
				on, firstOn + on.Length, StringComparison.Ordinal);

		Assert.True(firstOn >= 0 && firstArchive > firstOn);
		Assert.True(off > firstArchive &&
								darkness > off &&
								secondOn > darkness);

		Assert.Equal(-1, noLook.IndexOf(
				archive,
				firstArchive + archive.Length,
				StringComparison.Ordinal));

		int withLookFirst = withLook.IndexOf(
				archive, StringComparison.Ordinal);

		Assert.True(withLook.IndexOf(
				archive,
				withLookFirst + archive.Length,
				StringComparison.Ordinal) >
				withLook.IndexOf(
						"The room falls dark.",
						StringComparison.Ordinal));

		Assert.DoesNotContain("A Grue devours you.", withLook);
		Assert.Contains("You leave the dungeon.", withLook);
	}

	[Fact]
	public void DroppedActiveLamp_PreservesLightAndPermitsTakeButNotUncarriedUse()
	{
		string take = Play(
				"GO EAST", "TAKE LAMP", "USE LAMP",
				"GO NORTH", "DROP LAMP", "LOOK",
				"TAKE LAMP", "QUIT");

		int drop = take.IndexOf(
				"You drop the Lamp.", StringComparison.Ordinal);

		Assert.True(drop >= 0);

		string afterDrop = take.Substring(drop);

		Assert.Contains("Items: Brass Token, Iron Key, Lamp", afterDrop);
		Assert.Contains("You take the Lamp.", afterDrop);
		Assert.DoesNotContain("The room falls dark.", afterDrop);
		Assert.DoesNotContain("A Grue devours you.", afterDrop);
		Assert.Contains("You leave the dungeon.", take);

		string use = Play(
				"GO EAST", "TAKE LAMP", "USE LAMP",
				"GO NORTH", "DROP LAMP", "USE LAMP", "QUIT");

		Assert.Contains("You are not carrying LAMP.", use);
		Assert.DoesNotContain("A Grue devours you.", use);
	}

	[Fact]
	public void Use_UncarriedOrAbsentTargetInDarknessIsFatalButMissingArgumentIsNot()
	{
		string uncarried = Play(
				"GO EAST", "GO NORTH", "USE OLD COIN", "QUIT");

		Assert.Contains("A Grue devours you.", uncarried);
		Assert.DoesNotContain(
				"You are not carrying OLD COIN.", uncarried);
		Assert.DoesNotContain(
				"You leave the dungeon.", uncarried);

		string absent = Play(
				"GO EAST", "GO NORTH", "USE MICROPHONE", "QUIT");

		Assert.Contains("A Grue devours you.", absent);
		Assert.DoesNotContain(
				"You are not carrying MICROPHONE.", absent);
		Assert.DoesNotContain(
				"You leave the dungeon.", absent);

		string malformed = Play(
				"GO EAST", "GO NORTH", "USE", "GO SOUTH", "QUIT");

		Assert.Contains(
				"Use USE followed by an item name.", malformed);
		Assert.Contains(
				"You move SOUTH to Gallery.", malformed);
		Assert.DoesNotContain(
				"A Grue devours you.", malformed);
		Assert.Contains(
				"You leave the dungeon.", malformed);
	}

	[Fact]
	public void Lamp_SafeRetreatAfterSwitchOffInArchive()
	{
		string output = Play(
				"GO EAST", "TAKE LAMP", "USE LAMP",
				"GO NORTH", "USE LAMP", "GO SOUTH", "QUIT");

		int off = output.IndexOf(
				"The Lamp is now off.", StringComparison.Ordinal);

		int darkness = output.IndexOf(
				"The room falls dark.", StringComparison.Ordinal);

		int retreat = output.IndexOf(
				"You move SOUTH to Gallery.", StringComparison.Ordinal);

		Assert.True(off >= 0);
		Assert.True(darkness > off);
		Assert.True(retreat > darkness);
		Assert.DoesNotContain("A Grue devours you.", output);
		Assert.Contains("You leave the dungeon.", output);
	}

	[Fact]
	public void Lamp_ActiveDropAndRetreatRemainSafe()
	{
		string output = Play(
				"GO EAST", "TAKE LAMP", "USE LAMP",
				"GO NORTH", "DROP LAMP", "GO SOUTH", "QUIT");

		int drop = output.IndexOf(
				"You drop the Lamp.", StringComparison.Ordinal);

		int retreat = output.IndexOf(
				"You move SOUTH to Gallery.", StringComparison.Ordinal);

		Assert.True(drop >= 0 && retreat > drop);
		Assert.DoesNotContain("The room falls dark.", output);
		Assert.DoesNotContain("A Grue devours you.", output);
		Assert.Contains("You leave the dungeon.", output);
	}

	[Fact]
	public void Door_UnlockAndRoundTrip_PreserveKeyAndDoorState()
	{
		string output = Play(
				"GO EAST", "GO EAST", "OPEN IRON DOOR",
				"TAKE LAMP", "USE LAMP",
				"GO NORTH", "TAKE IRON KEY",
				"GO SOUTH", "OPEN IRON DOOR",
				"INVENTORY", "GO EAST", "GO WEST",
				"OPEN IRON DOOR", "QUIT");

		int blocked = output.IndexOf(
				"The Iron Door is closed. You cannot pass.",
				StringComparison.Ordinal);

		int locked = output.IndexOf(
				"The Iron Door is locked. You need its key.",
				StringComparison.Ordinal);

		int unlocked = output.IndexOf(
				"You unlock and open the Iron Door.",
				StringComparison.Ordinal);

		int entered = output.IndexOf(
				"You move EAST to Storage.",
				StringComparison.Ordinal);

		int returned = output.IndexOf(
				"You move WEST to Gallery.",
				StringComparison.Ordinal);

		int alreadyOpen = output.IndexOf(
				"The Iron Door is already open.",
				StringComparison.Ordinal);

		Assert.True(blocked >= 0);
		Assert.True(locked > blocked);
		Assert.True(unlocked > locked);
		Assert.True(entered > unlocked);
		Assert.True(returned > entered);
		Assert.True(alreadyOpen > returned);

		Assert.Contains(
				"  - Iron Key: A cold iron key with a square bow.",
				output);

		Assert.DoesNotContain("A Grue devours you.", output);
		Assert.Contains("You leave the dungeon.", output);
	}

	[Fact]
	public void Open_GruePrecedesLookupButMalformedOpenIsSafe()
	{
		string fatal = Play(
				"GO EAST", "GO NORTH",
				"OPEN MICROPHONE", "QUIT");

		Assert.Contains("A Grue devours you.", fatal);
		Assert.DoesNotContain(
				"There is no MICROPHONE to open", fatal);
		Assert.DoesNotContain(
				"You leave the dungeon.", fatal);

		string namedDoor = Play(
				"GO EAST", "GO NORTH",
				"OPEN IRON DOOR", "QUIT");

		Assert.Contains("A Grue devours you.", namedDoor);
		Assert.DoesNotContain(
				"The Iron Door is locked.", namedDoor);
		Assert.DoesNotContain(
				"There is no IRON DOOR to open", namedDoor);

		string malformed = Play(
				"GO EAST", "GO NORTH",
				"OPEN", "GO SOUTH", "QUIT");

		Assert.Contains(
				"Use OPEN followed by an object name.",
				malformed);
		Assert.Contains(
				"You move SOUTH to Gallery.",
				malformed);
		Assert.DoesNotContain(
				"A Grue devours you.", malformed);
		Assert.Contains(
				"You leave the dungeon.", malformed);
	}



	// Helper: ejecutar el juego con comandos simulados.
	private static string Play(params string[] commands)
	{
		TextReader originalInput = Console.In;
		TextWriter originalOutput = Console.Out;

		using StringReader input = new StringReader(
				string.Join(Environment.NewLine, commands));

		using StringWriter output = new StringWriter();

		try
		{
			Console.SetIn(input);
			Console.SetOut(output);

			Program.Main();
		}
		finally
		{
			Console.SetIn(originalInput);
			Console.SetOut(originalOutput);
		}

		return output.ToString();
	}
}
