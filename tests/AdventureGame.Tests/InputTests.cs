namespace AdventureGame.Tests;

public class InputTests
{
	[Theory]
	[InlineData(" north ", "NORTH")]
	[InlineData(" go   EaSt ", "GO   EAST")]
	[InlineData("look", "LOOK")]
	[InlineData("   ", "")]
	public void NormalizeInput_PreservesLesson01Behavior(string input, string expected)
	{
		Assert.Equal(expected, Program.NormalizeInput(input));
	}

	[Fact]
	public void NormalizeInput_NullStillMapsToQuit()
	{
		Assert.Equal(Program.QUIT, Program.NormalizeInput(null));
	}
}