
class User:
    def __init__(self, username: str, password: str, gamesWon: int = 0, gamesLost: int = 0):
        """Creates a new User

        Args:
            username (str): The username for the user
            password (str): The password for the user
            gamesWon (int): The number of games the user has won
            gamesLost (int): The number of games the user has lost
        """
        self.username = username
        """The username the user has"""
        self.password = password
        """The password the user has"""

        self.gamesWon = gamesWon
        """The number of games the user has won"""
        self.gamesLost = gamesLost
        """The number of games the user has lost"""
        self.played = gamesWon + gamesLost
        """The number of games the user has played"""
        self.winPercent = gamesWon // 1 if self.played == 0 else self.played
        """The percentage of games the user has won"""

    def win(self):
        """Increments the number of times the player has won"""
        self.played += 1
        self.gamesWon += 1
        self.winPercent = self.gamesWon // self.played

    def lose(self):
        """Increments the number of times the player has lost"""
        self.played += 1
        self.gamesLost += 1
        self.winPercent = self.gamesWon // self.played
