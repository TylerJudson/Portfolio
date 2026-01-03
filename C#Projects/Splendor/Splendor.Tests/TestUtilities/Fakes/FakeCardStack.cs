using Splendor.Models;

namespace Splendor.Tests.TestUtilities.Fakes;

/// <summary>
/// A controllable ICardStack implementation for predictable testing.
/// Unlike the real CardStack, this draws cards in a deterministic FIFO order.
/// </summary>
public class FakeCardStack : ICardStack
{
    private readonly Queue<ICard> _cards;

    public uint Level { get; }

    public List<ICard> Cards => _cards.ToList();

    public FakeCardStack(uint level)
    {
        Level = level;
        _cards = new Queue<ICard>();
    }

    public FakeCardStack(uint level, params ICard[] cards)
    {
        Level = level;
        _cards = new Queue<ICard>(cards);
    }

    public FakeCardStack(uint level, IEnumerable<ICard> cards)
    {
        Level = level;
        _cards = new Queue<ICard>(cards);
    }

    /// <summary>
    /// Draws the next card from the queue (FIFO order).
    /// Returns null if no cards remain.
    /// </summary>
    public ICard? Draw()
    {
        return _cards.Count > 0 ? _cards.Dequeue() : null;
    }

    /// <summary>
    /// Adds a card to the end of the queue.
    /// </summary>
    public void AddCard(ICard card)
    {
        _cards.Enqueue(card);
    }

    /// <summary>
    /// Adds multiple cards to the end of the queue.
    /// </summary>
    public void AddCards(params ICard[] cards)
    {
        foreach (var card in cards)
        {
            _cards.Enqueue(card);
        }
    }

    /// <summary>
    /// Clears all cards from the stack.
    /// </summary>
    public void SetEmpty()
    {
        _cards.Clear();
    }

    /// <summary>
    /// Returns the number of cards remaining in the stack.
    /// </summary>
    public int Count => _cards.Count;

    /// <summary>
    /// Peeks at the next card without removing it.
    /// </summary>
    public ICard? Peek()
    {
        return _cards.Count > 0 ? _cards.Peek() : null;
    }
}
