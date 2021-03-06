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

        self.pos = pos
        """The position of the text"""

        self.color = color
        """The color of the text"""

        self.rect = self.display.get_rect(center=(pos[0], pos[1]))
        """The Rect used for positioning"""
        
        self.text = txt

    @property
    def text(self):
        """The text that gets displayed in the text"""
        return self._text

    @text.setter
    def text(self, value: str):
        self._text = value
        self.display = self.font.render(self._text, True, self.color)
        self.rect = self.display.get_rect(center=(self.pos[0], self.pos[1]))

    def addText(self, value: str):
        self._text += value
        self.display = self.font.render(self._text, True, self.color)
        self.rect.size = self.display.get_size()
    def changeText(self, value: str):
        self._text = value
        self.display = self.font.render(self._text, True, self.color)
        self.rect.size = self.display.get_size()
