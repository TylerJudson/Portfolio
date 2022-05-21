namespace SecretHitler.Models
{
    public interface IPolicy
    {
        /// <summary>
        /// The Ideology of the policy i.e. fascist or liberal
        /// </summary>
        Ideology Ideology { get; }
    }
}
