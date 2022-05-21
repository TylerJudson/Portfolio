namespace SecretHitler.Models
{
    public interface IGameBoard
    {

        /// <summary>
        /// The players that are playing the game
        /// </summary>
        List <IPlayer> Players { get; }

        /// <summary>
        /// The Track that is liberal
        /// </summary>
        ITrack LiberalTrack { get; }

        /// <summary>
        /// The Track that is fascist
        /// </summary>
        ITrack FascistTrack { get; }

        /// <summary>
        /// The deck to draw policies out of
        /// </summary>
        List <IPolicy> PolicyDeck { get; }

        /// <summary>
        /// The deck to discard policies
        /// </summary>
        List <IPolicy> PolicyDiscard { get; }

    }
}
