namespace SecretHitler.Models
{
    public interface IPlayer
    {

        /// <summary>
        /// The Name of the player.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Id of the player.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The Role of the player i.e. Fascist, Hitler, or Liberal 
        /// </summary>
        Role Role { get; }

    }
}
