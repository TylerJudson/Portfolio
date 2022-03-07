namespace Splendor.Models.Implementation
{
    public class Noble : INoble
    {
        
        public string ImageName { get; }
        public Dictionary<Token, int> Criteria { get; }
        public uint PrestigePoints { get; } = 3;


        /// <summary>
        /// Initializes a Noble
        /// </summary>
        /// <param name="criteria">The criteria to acheive a noble</param>
        /// <param name="imageName">The name of the image for the nobel</param>
        public Noble(Dictionary<Token, int> criteria, string imageName)
        {
            Criteria = criteria;
            ImageName = imageName;
        }
        public string Render()
        {
            return "<img class=\"col\" src=\"Images/" + ImageName + " />";
        }
    }
}
