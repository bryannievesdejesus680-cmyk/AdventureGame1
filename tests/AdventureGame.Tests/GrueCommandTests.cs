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
}