namespace Splendor.Models
{
    public interface INoble
    {
        /// <summary>
        /// The name of the image for the noble
        /// </summary>
        string ImageName { get; }
        /// <summary>
        /// The criteria it takes to receive a noble
        /// </summary>
        IReadOnlyDictionary<Token, int> Criteria { get; }

        /// <summary>
        /// The number of prestige points a noble contains
        /// </summary>
        uint PrestigePoints { get; } 
    }
}
