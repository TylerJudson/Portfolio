from typing import Tuple
import pygame
from colors import BLACK


class Text:
    """Represents a text object in pygame
    """

    def __init__(self, pos: Tuple[int, int], font: str, fontSize: int, txt: str, color: Tuple[int, int, int]):
        """Initializes the text object

            Args:
                pos (Tuple[int, int]): Where the Text should be placed on the screen
                font (str): The font the text should be
                fontSize (int): The size of the font
                txt (str): The text that should be in the Text
                color (Tuple[int, int, int]): The color of the text
        """

        self.font = pygame.font.Font(font, fontSize)
        """The font object used to style the text"""

        self.display = self.font.render(txt, True, color)
        """The surface that gets displayed"""

        self.rect = self.display.get_rect(center=(pos[0], pos[1]))
        """The Rect used for positioning"""
