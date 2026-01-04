using FluentAssertions;
using Splendor.Models;
using Splendor.Models.Implementation;
using Splendor.Tests.TestUtilities.Builders;
using Splendor.Tests.TestUtilities.Helpers;
using Xunit;

namespace Splendor.Tests.Unit.Models
{
    /// <summary>
    /// Comprehensive tests for the reservation + gold + token return flow
    /// Testing the ENTIRE flow to find logic errors
    /// </summary>
    public class GameBoardTests_ReservationGoldFlow
    {
        [Fact]
        public void ReservationGoldFlow_Step1_InitialState()
        {
            // Arrange - Player has exactly 10 tokens
            var player = new PlayerBuilder()
                .WithName("Player1")
                .WithId(0)
                .WithTokens(diamond: 2, sapphire: 2, emerald: 2, ruby: 2, onyx: 2, gold: 0)
                .Build();

            var dummyPlayer = new Player("Dummy", 1);
            var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

            // Assert initial state
            player.NumberOfTokens().Should().Be(10, "player starts with exactly 10 tokens");
            board.TokenStacks[Token.Gold].Should().Be(5, "board should have 5 gold tokens for 2 players");
            board.CurrentPlayer.Should().Be(0, "should be player 1's turn");
            player.ReservedCards.Count.Should().Be(0, "player should have no reserved cards");
        }

        [Fact]
        public void ReservationGoldFlow_Step2_ReserveCard()
        {
            // Arrange - Player has exactly 10 tokens
            var player = new PlayerBuilder()
                .WithName("Player1")
                .WithId(0)
                .WithTokens(diamond: 2, sapphire: 2, emerald: 2, ruby: 2, onyx: 2, gold: 0)
                .Build();

            var dummyPlayer = new Player("Dummy", 1);
            var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

            var cardToReserve = board.Level1Cards[0]!;
            int initialGoldOnBoard = board.TokenStacks[Token.Gold];

            // Act - Reserve the card
            var turn = new Turn(cardToReserve, isReserve: true);
            var result = board.ExecuteTurn(turn);

            // Assert - Should return continue action
            result.ContinueAction.Should().NotBeNull("reservation should trigger continue action when player has 10 tokens");
            result.ContinueAction!.ActionCode.Should().Be(0, "action code 0 means return tokens");
            result.ContinueAction.Message.Should().Contain("return", "message should tell player to return tokens");
            result.ContinueAction.Message.Should().Contain("1 token", "player needs to return 1 token");
            result.Error.Should().BeNull("should not have an error");

            // Assert - Player state after reservation
            player.ReservedCards.Should().Contain(cardToReserve, "card should be in reserved cards");
            player.ReservedCards.Count.Should().Be(1, "player should have 1 reserved card");
            player.Tokens[Token.Gold].Should().Be(1, "player should have received the gold token");
            player.NumberOfTokens().Should().Be(11, "player should have 11 tokens total (10 + 1 gold)");

            // Assert - Board state after reservation
            board.TokenStacks[Token.Gold].Should().Be(initialGoldOnBoard - 1, "gold should be removed from board");
            board.CurrentPlayer.Should().Be(0, "turn should NOT advance when continue action is needed");
            board.Version.Should().Be(1, "version should increment to signal state change");
            board.LastTurn.Should().NotBeNull("last turn should be recorded");
            board.LastTurn!.ContinueAction.Should().NotBeNull("last turn should have continue action");
        }

        [Fact]
        public void ReservationGoldFlow_Step3_ReturnOneToken_Success()
        {
            // Arrange - Player has 11 tokens after reservation
            var reservedCard = new CardBuilder()
                .WithLevel(1)
                .WithType(Token.Diamond)
                .WithPrestigePoints(0)
                .WithImageName("test.png")
                .Build();

            var player = new PlayerBuilder()
                .WithName("Player1")
                .WithId(0)
                .WithTokens(diamond: 2, sapphire: 2, emerald: 2, ruby: 2, onyx: 2, gold: 1)
                .WithReservedCards(reservedCard)
                .Build();

            var dummyPlayer = new Player("Dummy", 1);
            var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

            // Simulate that we just did a reservation (set up the state)
            int initialEmeraldTokensOnBoard = board.TokenStacks[Token.Emerald];

            // Act - Return 1 Emerald token
            var returnTurn = new Turn(new Dictionary<Token, int> { { Token.Emerald, -1 } });
            var result = board.ExecuteTurn(returnTurn);

            // Assert - Should complete successfully
            result.Error.Should().BeNull("returning 1 token should succeed");
            result.ContinueAction.Should().BeNull("should not need another continue action after returning 1 token");
            result.GameOver.Should().BeFalse("game should not be over");

            // Assert - Player state after return
            player.Tokens[Token.Emerald].Should().Be(1, "player should have 1 emerald left (had 2, returned 1)");
            player.Tokens[Token.Gold].Should().Be(1, "player should still have the gold token");
            player.NumberOfTokens().Should().Be(10, "player should have exactly 10 tokens now");

            // Assert - Board state after return
            board.TokenStacks[Token.Emerald].Should().Be(initialEmeraldTokensOnBoard + 1, "returned token should be added back to board");
            board.CurrentPlayer.Should().Be(1, "turn should advance to next player");
            board.Version.Should().Be(1, "version should increment");
        }

        [Fact]
        public void ReservationGoldFlow_Step3_ReturnGoldToken_Success()
        {
            // Arrange - Player has 11 tokens after reservation, tries to return the gold
            var reservedCard = new CardBuilder()
                .WithLevel(1)
                .WithType(Token.Diamond)
                .WithPrestigePoints(0)
                .WithImageName("test.png")
                .Build();

            var player = new PlayerBuilder()
                .WithName("Player1")
                .WithId(0)
                .WithTokens(diamond: 2, sapphire: 2, emerald: 2, ruby: 2, onyx: 2, gold: 1)
                .WithReservedCards(reservedCard)
                .Build();

            var dummyPlayer = new Player("Dummy", 1);
            var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

            int initialGoldTokensOnBoard = board.TokenStacks[Token.Gold];

            // Act - Return the gold token that was just received
            var returnTurn = new Turn(new Dictionary<Token, int> { { Token.Gold, -1 } });
            var result = board.ExecuteTurn(returnTurn);

            // Assert - Should complete successfully
            result.Error.Should().BeNull("returning the gold token should succeed");
            result.ContinueAction.Should().BeNull("should not need another continue action");

            // Assert - Player state
            player.Tokens[Token.Gold].Should().Be(0, "player should have 0 gold after returning it");
            player.NumberOfTokens().Should().Be(10, "player should have exactly 10 tokens");

            // Assert - Board state
            board.TokenStacks[Token.Gold].Should().Be(initialGoldTokensOnBoard + 1, "gold should be returned to board");
            board.CurrentPlayer.Should().Be(1, "turn should advance");
        }

        [Fact]
        public void ReservationGoldFlow_Step3_ReturnTooFewTokens_StillOver10()
        {
            // Arrange - Player has 12 tokens somehow, tries to return only 1
            var player = new PlayerBuilder()
                .WithName("Player1")
                .WithId(0)
                .WithTokens(diamond: 2, sapphire: 2, emerald: 2, ruby: 2, onyx: 3, gold: 1)
                .Build();

            var dummyPlayer = new Player("Dummy", 1);
            var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

            player.NumberOfTokens().Should().Be(12, "player starts with 12 tokens");

            // Act - Try to return only 1 token (should need to return 2)
            var returnTurn = new Turn(new Dictionary<Token, int> { { Token.Emerald, -1 } });
            var result = board.ExecuteTurn(returnTurn);

            // Assert - Should get error or continue action
            if (result.Error != null)
            {
                result.Error.Message.Should().Contain("10", "error should mention the 10 token limit");
            }
            else if (result.ContinueAction != null)
            {
                result.ContinueAction.Message.Should().Contain("return", "should ask to return more tokens");
                player.NumberOfTokens().Should().Be(11, "player should have 11 tokens after returning 1");
            }
            else
            {
                Assert.Fail("Should either have error or continue action when still over 10 tokens");
            }
        }

        [Fact]
        public void ReservationGoldFlow_Step3_ReturnTooManyTokens_Under10()
        {
            // Arrange - Player has 11 tokens, tries to return 2
            var player = new PlayerBuilder()
                .WithName("Player1")
                .WithId(0)
                .WithTokens(diamond: 2, sapphire: 2, emerald: 2, ruby: 2, onyx: 2, gold: 1)
                .Build();

            var dummyPlayer = new Player("Dummy", 1);
            var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

            // Act - Try to return 2 tokens (would leave player with 9)
            var returnTurn = new Turn(new Dictionary<Token, int> { { Token.Emerald, -2 } });
            var result = board.ExecuteTurn(returnTurn);

            // Assert - Should succeed (players CAN have fewer than 10 tokens)
            result.Error.Should().BeNull("returning tokens to go under 10 should be allowed");
            player.NumberOfTokens().Should().Be(9, "player should have 9 tokens");
        }

        [Fact]
        public void ReservationGoldFlow_Step3_ReturnTokenPlayerDoesntHave()
        {
            // Arrange - Player has 11 tokens but no sapphire
            var player = new PlayerBuilder()
                .WithName("Player1")
                .WithId(0)
                .WithTokens(diamond: 3, sapphire: 0, emerald: 2, ruby: 2, onyx: 3, gold: 1)
                .Build();

            var dummyPlayer = new Player("Dummy", 1);
            var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

            player.Tokens[Token.Sapphire].Should().Be(0, "player has no sapphire");

            // Act - Try to return a sapphire token they don't have
            var returnTurn = new Turn(new Dictionary<Token, int> { { Token.Sapphire, -1 } });
            var result = board.ExecuteTurn(returnTurn);

            // Assert - Should get an error
            result.Error.Should().NotBeNull("can't return tokens you don't have");
            result.Error!.Message.Should().Contain("enough", "error should mention not having enough tokens");
        }

        [Fact]
        public void ReservationGoldFlow_FullEndToEnd_WithDifferentTokenCombinations()
        {
            // Test Case 1: Player with evenly distributed tokens
            {
                var player = new PlayerBuilder()
                    .WithName("Player1")
                    .WithId(0)
                    .WithTokens(diamond: 2, sapphire: 2, emerald: 2, ruby: 2, onyx: 2, gold: 0)
                    .Build();

                var dummyPlayer = new Player("Dummy", 1);
                var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

                var cardToReserve = board.Level1Cards[0]!;

                // Step 1: Reserve card
                var reserveTurn = new Turn(cardToReserve, isReserve: true);
                var reserveResult = board.ExecuteTurn(reserveTurn);

                reserveResult.ContinueAction.Should().NotBeNull();
                player.NumberOfTokens().Should().Be(11);

                // Step 2: Return 1 token
                var returnTurn = new Turn(new Dictionary<Token, int> { { Token.Diamond, -1 } });
                var returnResult = board.ExecuteTurn(returnTurn);

                returnResult.Error.Should().BeNull();
                returnResult.ContinueAction.Should().BeNull();
                player.NumberOfTokens().Should().Be(10);
                board.CurrentPlayer.Should().Be(1, "turn should advance after successful return");
            }

            // Test Case 2: Player with unevenly distributed tokens
            {
                var player2 = new PlayerBuilder()
                    .WithName("Player2")
                    .WithId(0)
                    .WithTokens(diamond: 5, sapphire: 1, emerald: 1, ruby: 1, onyx: 2, gold: 0)
                    .Build();

                var dummyPlayer2 = new Player("Dummy", 1);
                var board2 = new GameBoard(new List<IPlayer> { player2, dummyPlayer2 }, TestHelpers.CreateMockGameDataService());

                var cardToReserve2 = board2.Level1Cards[0]!;

                // Reserve and return
                var reserveTurn2 = new Turn(cardToReserve2, isReserve: true);
                board2.ExecuteTurn(reserveTurn2);

                var returnTurn2 = new Turn(new Dictionary<Token, int> { { Token.Diamond, -1 } });
                var returnResult2 = board2.ExecuteTurn(returnTurn2);

                returnResult2.Error.Should().BeNull();
                player2.NumberOfTokens().Should().Be(10);
                player2.Tokens[Token.Diamond].Should().Be(4);
            }
        }

        [Fact]
        public void ReservationGoldFlow_NoGoldAvailable_ShouldStillWork()
        {
            // Arrange - Simulate 4 players (which gives 5 gold tokens)
            // Then reserve 5 times to consume all gold
            var player1 = new PlayerBuilder().WithName("P1").WithId(0).WithTokens(diamond: 2).Build();
            var player2 = new PlayerBuilder().WithName("P2").WithId(1).WithTokens(diamond: 2).Build();
            var player3 = new PlayerBuilder().WithName("P3").WithId(2).WithTokens(diamond: 2).Build();
            var player4 = new PlayerBuilder().WithName("P4").WithId(3).WithTokens(diamond: 2).Build();

            var board = new GameBoard(
                new List<IPlayer> { player1, player2, player3, player4 },
                TestHelpers.CreateMockGameDataService());

            // Reserve 5 cards to use up all gold (4 players = 5 gold tokens)
            for (int i = 0; i < 5 && board.TokenStacks[Token.Gold] > 0; i++)
            {
                int playerIndex = i % 4;
                var cardToReserve = board.Level1Cards.FirstOrDefault(c => c != null);
                if (cardToReserve != null)
                {
                    var reserveTurn = new Turn(cardToReserve, isReserve: true);
                    board.ExecuteTurn(reserveTurn);

                    // Advance to next player manually if needed
                    while (board.CurrentPlayer != (playerIndex + 1) % 4 && board.CurrentPlayer != playerIndex)
                    {
                        // Skip turn for other players
                        board.ExecuteTurn(new Turn(new Dictionary<Token, int> { { Token.Diamond, 0 } }));
                    }
                }
            }

            // Now gold should be depleted
            board.TokenStacks[Token.Gold].Should().Be(0, "all gold should be used up");

            // Set up test player with 10 tokens
            var testPlayer = new PlayerBuilder()
                .WithName("TestPlayer")
                .WithId(4)
                .WithTokens(diamond: 2, sapphire: 2, emerald: 2, ruby: 2, onyx: 2, gold: 0)
                .Build();

            // Create new board with test player and depleted gold
            var testBoard = new GameBoard(
                new List<IPlayer> { testPlayer, player2 },
                TestHelpers.CreateMockGameDataService());

            // Deplete gold tokens by using reflection
            var tokenStacksField = typeof(GameBoard).GetField("_tokenStacks",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tokenStacks = (Dictionary<Token, int>)tokenStacksField!.GetValue(testBoard)!;
            tokenStacks[Token.Gold] = 0;

            var cardToReserveTest = testBoard.Level1Cards[0]!;

            // Act - Reserve card when no gold available
            var testReserveTurn = new Turn(cardToReserveTest, isReserve: true);
            var result = testBoard.ExecuteTurn(testReserveTurn);

            // Assert - Should succeed without continue action (no gold added, still at 10 tokens)
            result.Error.Should().BeNull("reservation should succeed even without gold");
            result.ContinueAction.Should().BeNull("should not need to return tokens when no gold was added");
            testPlayer.ReservedCards.Should().Contain(cardToReserveTest);
            testPlayer.Tokens[Token.Gold].Should().Be(0, "player should not get gold when none available");
            testPlayer.NumberOfTokens().Should().Be(10, "player should still have 10 tokens");
        }

        [Fact]
        public void ReservationGoldFlow_PlayerHas9Tokens_ShouldNotTriggerContinueAction()
        {
            // Arrange - Player has 9 tokens
            var player = new PlayerBuilder()
                .WithName("Player1")
                .WithId(0)
                .WithTokens(diamond: 2, sapphire: 2, emerald: 2, ruby: 2, onyx: 1, gold: 0)
                .Build();

            var dummyPlayer = new Player("Dummy", 1);
            var board = new GameBoard(new List<IPlayer> { player, dummyPlayer }, TestHelpers.CreateMockGameDataService());

            player.NumberOfTokens().Should().Be(9);

            var cardToReserve = board.Level1Cards[0]!;

            // Act - Reserve card
            var reserveTurn = new Turn(cardToReserve, isReserve: true);
            var result = board.ExecuteTurn(reserveTurn);

            // Assert - Should succeed without continue action (9 + 1 = 10, which is the limit)
            result.Error.Should().BeNull();
            result.ContinueAction.Should().BeNull("should not need to return tokens when exactly at limit");
            player.NumberOfTokens().Should().Be(10);
            board.CurrentPlayer.Should().Be(1, "turn should advance");
        }

        [Fact]
        public void ReservationGoldFlow_MultiplePlayersSequence()
        {
            // Arrange - 3 players, player 1 reserves with 10 tokens
            var player1 = new PlayerBuilder()
                .WithName("Player1")
                .WithId(0)
                .WithTokens(diamond: 2, sapphire: 2, emerald: 2, ruby: 2, onyx: 2, gold: 0)
                .Build();

            var player2 = new PlayerBuilder()
                .WithName("Player2")
                .WithId(1)
                .WithTokens(diamond: 1, sapphire: 1, emerald: 1, ruby: 1, onyx: 1, gold: 0)
                .Build();

            var player3 = new PlayerBuilder()
                .WithName("Player3")
                .WithId(2)
                .WithTokens(diamond: 1, sapphire: 1, emerald: 1, ruby: 1, onyx: 1, gold: 0)
                .Build();

            var board = new GameBoard(new List<IPlayer> { player1, player2, player3 }, TestHelpers.CreateMockGameDataService());

            // Player 1 reserves
            var cardToReserve = board.Level1Cards[0]!;
            var reserveTurn = new Turn(cardToReserve, isReserve: true);
            board.ExecuteTurn(reserveTurn);

            board.CurrentPlayer.Should().Be(0, "should still be player 1's turn due to continue action");

            // Player 1 returns token
            var returnTurn = new Turn(new Dictionary<Token, int> { { Token.Emerald, -1 } });
            board.ExecuteTurn(returnTurn);

            board.CurrentPlayer.Should().Be(1, "should now be player 2's turn");

            // Verify player 1's final state
            player1.NumberOfTokens().Should().Be(10);
            player1.ReservedCards.Count.Should().Be(1);
            player1.Tokens[Token.Gold].Should().Be(1);
            player1.Tokens[Token.Emerald].Should().Be(1);
        }
    }
}
