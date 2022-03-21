

from typing import Tuple
import pygame


class Text:
    def __init__(self, pos: Tuple, font: str, fontSize: int, txt: str, color: Tuple):
        """
        initializes the text object

        PARAMETERS
        ----------
        pos : Tuple
            where the text should be placed
        font : str
            what the font of the text should be
        """
        self.font = pygame.font.Font(font, fontSize)
        self.txt = self.font.render(txt, True, color)
        self.rect = self.txt.get_rect(center=(pos[0], pos[1]))