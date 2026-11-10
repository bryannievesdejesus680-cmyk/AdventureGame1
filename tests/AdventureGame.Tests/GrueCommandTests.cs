using System;
using System.IO;
using Xunit;

namespace AdventureGame.Tests;

[CollectionDefinition("Console I/O", DisableParallelization = true)]
public class ConsoleIoCollection
{
}

[Collection("Console I/O")]
public class GrueCommandTests
{
	private static string Play(params string[] commands)
	{
		TextReader originalInput = Console.In;
		TextWriter originalOutput = Console.Out;
		using StringReader input = new StringReader(string.Join(Environment.NewLine, commands));
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

	[Fact]
	public void Grue_InterceptsUnsafeAttemptsButAllowsMalformedInputAndRetreat()
	{
		string fatalMove = Play("GO EAST", "GO NORTH", "GO UP", "QUIT");
		Assert.Contains("A Grue devours you. You have been defeated.", fatalMove);
		Assert.DoesNotContain("Unknown direction UP.", fatalMove);
		Assert.DoesNotContain("You leave the dungeon.", fatalMove);

		string safeRetreat = Play("GO EAST", "GO NORTH", "LOOK EXTRA", "GO SOUTH", "QUIT");
		Assert.Contains("LOOK takes no arguments.", safeRetreat);
		Assert.Contains("You move SOUTH to Gallery.", safeRetreat);
		Assert.DoesNotContain("A Grue devours you.", safeRetreat);
		Assert.Contains("You leave the dungeon.", safeRetreat);

		string fatalTake = Play("GO EAST", "GO NORTH", "TAKE BRASS TOKEN", "QUIT");
		Assert.Contains("A Grue devours you. You have been defeated.", fatalTake);
		Assert.DoesNotContain("You take the Brass Token.", fatalTake);
		Assert.DoesNotContain("You leave the dungeon.", fatalTake);
	}

	[Fact]
	public void InterruptedCommands_KeepTheirNarrationAndSkipNormalEffects()
	{
		string move = Play("GO EAST", "GO NORTH", "GO UP", "QUIT");
		Assert.Contains("You rush blindly ahead", move);
		Assert.DoesNotContain("Unknown direction UP.", move);
		Assert.DoesNotContain("You leave the dungeon.", move);

		string inventory = Play("GO EAST", "GO NORTH", "INVENTORY", "QUIT");
		Assert.Contains("You rummage through your belongings", inventory);
		Assert.DoesNotContain("Inventory:", inventory);

		string take = Play("GO EAST", "GO NORTH", "TAKE BRASS TOKEN", "QUIT");
		Assert.Contains("You reach into the blackness.", take);
		Assert.DoesNotContain("You take the Brass Token.", take);
	}
	[Fact]
	public void UnknownVerbInDarkness_IsRejectedAndRetreatStillWorks()
	{
		string output = Play("GO EAST", "GO NORTH", "FLY NORTH", "GO SOUTH", "QUIT");
		Assert.Contains("Unknown command. Use GO <direction>", output);
		Assert.Contains("You move SOUTH to Gallery.", output);
		Assert.DoesNotContain("A Grue devours you.", output);
		Assert.Contains("You leave the dungeon.", output);
	}
}