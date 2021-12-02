namespace Splendor.Models
{
    public interface IGameObject
    {
        /// <summary>
        /// Contains the html code that renders an object
        /// </summary>
        /// <returns>html code that renders an object</returns>
        string Render();
    }
}
