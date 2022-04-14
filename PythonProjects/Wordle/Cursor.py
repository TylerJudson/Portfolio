

import time
from typing import Tuple
from pygame import Rect
import pygame
from Text import Text


class Cursor:
    def __init__(self, text: Text, color: Tuple[int, int, int], width: int=2):
        """Creates a cursor that blinks and keeps track of the index

        Args:
            text (Text): The text the cursor will be displayed on
            color (Tuple[int, int, int]): The color of the cursor
            width (int, optional): The width of the cursor. Defaults to 3.
        """
        self.text = text
        """The text the cursor will be displayed on"""

        self.width = width
        """The width of the cursor"""

        self.color = color
        """The color of the cursor"""

        self.rect = Rect(text.rect.topright, (width, text.rect.height))

    def move(self, text: Text):
        """Moves the cursor to the correct spot with the text

        Args:
            text (Text): The text to move the cursor on
        """
        self.text.changeText(text.text)
        self.rect.topleft = text.rect.topright

    def render(self, surface: pygame.Surface):
        """Renders the cursor on the surface

        Args:
            surface (pygame.Surface): The surface to render on
        """
        if (time.time() % 1 > 0.5):
            pygame.draw.rect(surface, self.color, self.rect)
