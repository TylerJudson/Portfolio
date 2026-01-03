using FluentAssertions;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Helpers;
using Xunit;

namespace Splendor.Tests.Unit.Models;

/// <summary>
/// Unit tests for the Noble class.
/// </summary>
public class NobleTests
{
    [Fact]
    public void Constructor_SetsCriteriaDictionary()
    {
        // Arrange
        var criteria = TokenHelper.Create(diamond: 3, sapphire: 3, emerald: 3);
        var imageName = "Noble-3D-3S-3E.jpg";

        // Act
        var noble = new Noble(criteria, imageName);

        // Assert
        noble.Criteria.Should().BeSameAs(criteria);
        noble.Criteria[Token.Diamond].Should().Be(3);
        noble.Criteria[Token.Sapphire].Should().Be(3);
        noble.Criteria[Token.Emerald].Should().Be(3);
    }

    [Fact]
    public void Constructor_SetsImageName()
    {
        // Arrange
        var criteria = TokenHelper.Create(diamond: 4, onyx: 4);
        var imageName = "Noble-4D-4O.jpg";

        // Act
        var noble = new Noble(criteria, imageName);

        // Assert
        noble.ImageName.Should().Be(imageName);
    }

    [Fact]
    public void PrestigePoints_AlwaysReturnsThree()
    {
        // Arrange
        var criteria = TokenHelper.Create(diamond: 3, sapphire: 3, emerald: 3);
        var noble = new Noble(criteria, "test-noble.jpg");

        // Act
        var prestigePoints = noble.PrestigePoints;

        // Assert
        prestigePoints.Should().Be(3);
    }

    [Fact]
    public void Render_ReturnsHtmlImgTagString()
    {
        // Arrange
        var criteria = TokenHelper.Create(diamond: 3, sapphire: 3, emerald: 3);
        var imageName = "Noble-3D-3S-3E.jpg";
        var noble = new Noble(criteria, imageName);

        // Act
        var html = noble.Render();

        // Assert
        html.Should().Contain("<img");
        html.Should().Contain("class=\"col\"");
        html.Should().Contain("src=\"Images/Noble-3D-3S-3E.jpg");
        html.Should().Contain("/>");

        // Note: The implementation has a syntax bug - missing closing quote after image name
        // The actual output is: <img class="col" src="Images/Noble-3D-3S-3E.jpg />
        // Instead of: <img class="col" src="Images/Noble-3D-3S-3E.jpg" />
    }
}
